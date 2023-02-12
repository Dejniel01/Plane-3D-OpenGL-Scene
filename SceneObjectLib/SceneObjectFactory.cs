using FileParserLib;

namespace Plane3DOpenGLScene.Structures
{
    public static class SceneObjectFactory
    {
        private static readonly Dictionary<string, Model> parsedModelDictionary = new();

        public static SceneObject CreatePlaneObject()
        {
            string planeObj = "./../../../../../Assets/caravanazip/source/42cc9aa7d22b4afa917d6f84e6c5642f/single.obj";

            return CreateModelFromFile(planeObj);
        }

        public static SceneObject CreateCloudObject()
        {
            string cloudObj = "./../../../../../Assets/clouds/source/cloud.obj";

            return CreateModelFromFile(cloudObj);
        }

        public static SceneObject CreateSphereObject() 
        {
            string sphereObj = "./../../../../../Assets/sphere.obj";

            return CreateModelFromFile(sphereObj);
        }

        public static SceneObject CreateRingObject()
        {
            string ringObj = "./../../../../../Assets/ring.obj";

            return CreateModelFromFile(ringObj);
        }

        public static SceneObject CreateOceanObject() 
        {
            string oceanObj = "./../../../../../Assets/ocean_cube.obj";

            return CreateModelFromFile(oceanObj);
        }

        public static SceneObject CreateTestObject()
        {
            string testObj = "./../../../../../Assets/simple/cube.obj";

            return CreateModelFromFile(testObj);
        }

        private static SceneObject CreateModelFromFile(string fileName)
        {
            if (parsedModelDictionary.TryGetValue(fileName, out Model? model))
                return new SceneObject(model);

            var parser = new ObjParser();
            var obj = parser.Parse(fileName);

            model = new Model(obj.Vertices, obj.HasTextures ? 8 : 6);
            parsedModelDictionary.Add(fileName, model);
            return new SceneObject(model);
        }
    }
}

using OpenTK.Mathematics;

namespace Plane3DOpenGLScene.Structures
{
    public class SceneObject
    {
        public SceneObject(Model model) 
        {
            Model = model;
        }

        public Model Model { get; set; }
        public Vector3 Position { get; set; } = Vector3.Zero; 
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public float Scale { get; set; } = 1;

        public Matrix4 ModelMatrix
        {
            get
            {
                return Matrix4.CreateScale(Scale)
                    * Matrix4.CreateRotationX(Rotation.X)
                    * Matrix4.CreateRotationY(Rotation.Y)
                    * Matrix4.CreateRotationZ(Rotation.Z)
                    * Matrix4.CreateTranslation(Position);
            }
        }
    }
}

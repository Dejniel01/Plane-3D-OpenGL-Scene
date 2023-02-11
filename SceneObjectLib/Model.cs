using OpenTK.Mathematics;

namespace Plane3DOpenGLScene.Structures
{
    public class Model
    {
        internal Model(Vector3[] vertices, Vector3[] normals, Vector2[] textures, (uint V, uint T, uint N)[] indices)
        {
            if (textures.Any())
                Stride = 8;
            else
                Stride = 6;
            var verticesList = new List<float>();
            foreach(var face in indices)
            {
                verticesList.Add(vertices[face.V].X);
                verticesList.Add(vertices[face.V].Y);
                verticesList.Add(vertices[face.V].Z);
                verticesList.Add(normals[face.N].X);
                verticesList.Add(normals[face.N].Y);
                verticesList.Add(normals[face.N].Z);
                if (textures.Any())
                {
                    verticesList.Add(textures[face.T].X);
                    verticesList.Add(textures[face.T].Y);
                }
            }

            Vertices = verticesList.ToArray();
        }

        internal Model(float[] vertices, int stride)
        {
            Vertices = vertices;
            Stride = stride;
        }

        public float[] Vertices { get; set; }

        public int Stride { get; private set; }
    }
}

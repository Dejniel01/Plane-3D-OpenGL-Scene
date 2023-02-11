using OpenTK.Mathematics;

namespace Plane3DOpenGLScene.Interfaces
{
    internal interface IOpenGLModel
    {
        float[] GetOpenGLVertices();
        uint[] GetOpenGLIndices(uint offset);
        Matrix4 GetModelMatrix();
    }
}

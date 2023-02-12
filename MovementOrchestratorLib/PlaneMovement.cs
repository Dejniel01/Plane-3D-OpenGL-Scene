using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Plane3DOpenGLScene.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementOrchestratorLib
{
    internal abstract class PlaneMovement
    {
        public Func<SceneObject, Vector3, bool> NextCondition { get; set; } = (plane, planeDirection) => false;
        public PlaneMovement? Next { get; set; }

        public virtual PlaneMovement? Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Position += planeDirection * deltaTime;
            if (NextCondition(plane, planeDirection))
                return Next;
            return this;
        }
    }

    internal class GoingStraight : PlaneMovement
    {
        private readonly Random rand = new();
        public override PlaneMovement Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Rotation += new Vector3((float)rand.NextDouble() * 2 * deltaTime - deltaTime, 0, 0);

            if (Math.Abs(plane.Rotation.X) > Math.PI / 30)
                plane.Rotation = new Vector3(Math.Sign(plane.Rotation.X) * (float)Math.PI / 30, plane.Rotation.Y, plane.Rotation.Z);

            return base.Move(plane, ref planeDirection, deltaTime);
        }
    }

    internal class TurningRight : PlaneMovement
    {
        public override PlaneMovement Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Rotation += new Vector3(0, -deltaTime / 5, 0);
            planeDirection = (new Vector4(planeDirection, 1) * Matrix4.CreateRotationY(-deltaTime / 5)).Xyz;
            if (plane.Rotation.X > -Math.PI / 5)
                plane.Rotation += new Vector3(-deltaTime / 5, 0, 0);

            return base.Move(plane, ref planeDirection, deltaTime);
        }
    }

    internal class TurningLeft : PlaneMovement
    {
        public override PlaneMovement Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Rotation += new Vector3(0, deltaTime / 5, 0);
            planeDirection = (new Vector4(planeDirection, 1) * Matrix4.CreateRotationY(deltaTime / 5)).Xyz;
            if (plane.Rotation.X < Math.PI / 5)
                plane.Rotation += new Vector3(deltaTime / 5, 0, 0);

            return base.Move(plane, ref planeDirection, deltaTime);
        }
    }

    internal class StoppingTurningRight : PlaneMovement
    {
        public override PlaneMovement Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Rotation += new Vector3(deltaTime / 5, 0, 0);

            return base.Move(plane, ref planeDirection, deltaTime);
        }
    }

    internal class StoppingTurningLeft : PlaneMovement
    {
        public override PlaneMovement Move(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            plane.Rotation += new Vector3(-deltaTime / 5, 0, 0);

            return base.Move(plane, ref planeDirection, deltaTime);
        }
    }

}

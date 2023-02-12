using OpenTK.Mathematics;
using Plane3DOpenGLScene.Structures;

namespace MovementOrchestratorLib
{
    public class PlaneMovementOrchestrator
    {
        private PlaneMovement current;

        public PlaneMovementOrchestrator()
        {
            PlaneMovement tmp = new GoingStraight();
            PlaneMovement first = new GoingStraight();

            first = tmp = new StoppingTurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.X >= 0,
                Next = tmp
            };
            tmp = new TurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.Y <= MathHelper.DegreesToRadians(-360),
                Next = tmp
            };
            tmp = new GoingStraight()
            {
                NextCondition = (plane, planeDirection) => plane.Position.X >= 5,
                Next = tmp
            };
            tmp = new StoppingTurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.X >= 0,
                Next = tmp
            };
            tmp = new TurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.Y <= MathHelper.DegreesToRadians(-180),
                Next = tmp
            };
            tmp = new GoingStraight()
            {
                NextCondition = (plane, planeDirection) => plane.Position.Z <= -5,
                Next = tmp
            };
            tmp = new StoppingTurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.X >= 0,
                Next = tmp
            };
            tmp = new TurningRight()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.Y <= MathHelper.DegreesToRadians(-90),
                Next = tmp
            };
            tmp = new StoppingTurningLeft()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.X <= 0,
                Next = tmp
            };
            tmp = new TurningLeft()
            {
                NextCondition = (plane, planeDirection) => plane.Rotation.Y >= MathHelper.DegreesToRadians(135),
                Next = tmp
            };
            current = new GoingStraight()
            {
                NextCondition = (plane, planeDirection) => plane.Position.X <= -5,
                Next = tmp
            };
            first.Next = current;
        }

        public void UpdatePosition(SceneObject plane, ref Vector3 planeDirection, float deltaTime) 
        {
            current = current.Move(plane, ref planeDirection, deltaTime)!;
            if (plane.Rotation.Y <= MathHelper.DegreesToRadians(-360))
                plane.Rotation += new Vector3(0, MathHelper.DegreesToRadians(360), 0);
            if (plane.Rotation.Y >= MathHelper.DegreesToRadians(360))
                plane.Rotation += new Vector3(0, MathHelper.DegreesToRadians(-360), 0);
        }
    }
}
using OpenTK.Mathematics;
using Plane3DOpenGLScene.Structures;

namespace MovementOrchestratorLib
{
    public class PlaneMovementOrchestrator
    {
        private PlaneMovementState currentState = PlaneMovementState.GoingStraight;
        private Random rand = new Random();
        private bool first = true;

        public void UpdatePosition(SceneObject plane, ref Vector3 planeDirection, float deltaTime)
        {
            switch (currentState)
            {
                case PlaneMovementState.GoingStraight:
                    plane.Rotation += new Vector3((float)rand.NextDouble() * 10 * deltaTime - 5*deltaTime, 0, 0);

                    if (Math.Abs(plane.Rotation.X) > Math.PI / 30)
                        plane.Rotation = new Vector3(Math.Sign(plane.Rotation.X) * (float)Math.PI / 30, plane.Rotation.Y, plane.Rotation.Z);

                    break;
                case PlaneMovementState.TurningRight:
                    plane.Rotation += new Vector3(0, -deltaTime / 5, 0);
                    planeDirection = (new Vector4(planeDirection, 1) * Matrix4.CreateRotationY(-deltaTime / 5)).Xyz;
                    if (plane.Rotation.X > -Math.PI / 5)
                        plane.Rotation += new Vector3(-deltaTime / 5, 0, 0);
                    break;
                case PlaneMovementState.TurningLeft:
                    plane.Rotation += new Vector3(0, deltaTime / 5, 0);
                    planeDirection = (new Vector4(planeDirection, 1) * Matrix4.CreateRotationY(deltaTime / 5)).Xyz;
                    if (plane.Rotation.X < Math.PI / 5)
                        plane.Rotation += new Vector3(deltaTime / 5, 0, 0);
                    break;
                case PlaneMovementState.GoingDown:
                    break;
                case PlaneMovementState.GoingUp:
                    break;
                case PlaneMovementState.StoppingGoingRight:
                    plane.Rotation += new Vector3(deltaTime / 5, 0, 0);
                    if (plane.Rotation.X >= 0)
                        currentState = PlaneMovementState.GoingStraight;
                    break;
                case PlaneMovementState.StoppingGoingLeft:
                    plane.Rotation += new Vector3(-deltaTime / 5, 0, 0);
                    if (plane.Rotation.X <= 0)
                        currentState = PlaneMovementState.GoingStraight;
                    break;
            }

            plane.Position += planeDirection * deltaTime;

            if (first && plane.Position.X <= -5)
            {
                currentState = PlaneMovementState.TurningRight;
                first = false;
            }

            if (currentState == PlaneMovementState.TurningRight && plane.Rotation.Y <= -Math.PI / 1.5)
                currentState = PlaneMovementState.StoppingGoingRight;
        }
    }
}
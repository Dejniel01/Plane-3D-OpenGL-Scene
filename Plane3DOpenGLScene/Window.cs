using OpenTK.Windowing.Desktop;
using Plane3DOpenGLScene.Structures;
using OpenTK.Graphics.OpenGL4;
using CommonClassLib;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using MovementOrchestratorLib;
using Assimp.Unmanaged;

namespace Plane3D.Plane3DOpenGLScene
{
    internal class Window : GameWindow
    {
        //private readonly float[] vertices;

        private readonly SceneObject plane;

        //private int _elementBufferObject;

        private int planeVertexBufferObject;

        private int planeVertexArrayObject;

        private Shader planeShader;

        private Texture planeTexture;

        private readonly SceneObject[] clouds;

        private int cloudVertexBufferObject;

        private int cloudVertexArrayObject;

        private Shader cloudShader;

        private readonly SceneObject[] mainLights;

        private int mainLightVertexBufferObject;

        private int mainLightVertexArrayObject;

        private Shader mainLightShader;

        private readonly SceneObject[] rings;

        private int ringVertexBufferObject;

        private int ringVertexArrayObject;

        private Shader ringShader;

        // The view and projection matrices have been removed as we don't need them here anymore.
        // They can now be found in the new camera class.

        // We need an instance of the new camera class so it can manage the view and projection matrix code.
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time.
        // Finally, we add the last position of the mouse so we can calculate the mouse offset easily.
        private Camera camera;

        private bool isFirstMove = true;

        private Vector2 lastPos;

        private float[] cloudScales = new float[]
        {
            0.01f,
            0.015f,
            0.01f,
            0.005f,
            0.013f,
            0.015f,
            0.011f,
            0.02f,
            0.01f,
            0.013f
        };

        private Vector3[] cloudPos = new Vector3[]
        {
            new Vector3(-10, -10, -10),
            new Vector3(-10, 0, -5),
            new Vector3(-10, -10, -10),
            new Vector3(-10, 10, -10),
            new Vector3(3, -10, 2),
            new Vector3(5, -10, -10),
            new Vector3(-10, -10, -10),
            new Vector3(-10, 4, -10),
            new Vector3(10, -10, 5),
            new Vector3(10, 10, 5),
        };

        private float[] ringScales = new float[]
        {
            1.5f,
            1.5f,
            1.5f,
            1.5f,
            //0.01f,
            //0.015f,
            //0.01f,
            //0.005f
        };

        private Vector3[] ringPos = new Vector3[]
        {
            new Vector3(-5, -20, -10),
            new Vector3(0, 0, 0),
            new Vector3(0, 5, -10),
            new Vector3(-10, 10, -10),
        };

        private Vector3[] ringRotations = new Vector3[]
        {
            new Vector3((float)Math.PI/2, 0, 0),
            new Vector3(0, 0, (float)Math.PI/2),
            new Vector3((float)Math.PI/2, 0, 0),
            new Vector3(0, 0, (float)Math.PI/2),
        };

        private Vector3 planeDirection = new Vector3(-1, 0, 0);

        private PlaneMovementOrchestrator planeMovementOrchestrator = new PlaneMovementOrchestrator();

        //private double time;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            plane = SceneObjectFactory.CreatePlaneObject();
            plane.Scale = 0.25f;
            //plane.Position = new(10, 10, 10);
            //plane.Rotation = new Vector3(0, (float)Math.PI / 2, 0);

            clouds = new SceneObject[]
            {
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject()
            };

            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].Scale = cloudScales[i];
                clouds[i].Position = cloudPos[i];
            }

            mainLights = new SceneObject[]
            {
                SceneObjectFactory.CreateSphereObject(),
                SceneObjectFactory.CreateSphereObject()
            };

            mainLights[0].Position = new Vector3(75, 0, 0);
            mainLights[1].Position = new Vector3(-75, 0, 0);
            mainLights[0].Scale = 5;
            mainLights[1].Scale = 2;

            rings = new SceneObject[]
            {
                SceneObjectFactory.CreateRingObject(),
                SceneObjectFactory.CreateRingObject(),
                SceneObjectFactory.CreateRingObject(),
                SceneObjectFactory.CreateRingObject(),
            };

            for (int i = 0; i < rings.Length; i++)
            {
                rings[i].Scale = ringScales[i];
                rings[i].Position = ringPos[i];
                rings[i].Rotation = ringRotations[i];
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5294f, 0.7961f, 0.8706f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            SetupPlane();

            SetupClouds();

            SetupMainLights();

            SetupRings();

            // We initialize the camera so that it is 3 units back from where the rectangle is.
            // We also give it the proper aspect ratio.
            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            // We make the mouse cursor invisible and captured so we can have proper FPS-camera movement.
            CursorState = CursorState.Grabbed;
        }

        private void SetupPlane()
        {
            planeVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(planeVertexArrayObject);

            planeVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, planeVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, plane.Model.Vertices.Length * sizeof(float), plane.Model.Vertices, BufferUsageHint.StaticDraw);

            planeShader = new Shader("Shaders/TextureShader/shader.vert", "Shaders/TextureShader/shader.frag");
            planeShader.Use();

            var planeVertexLocation = planeShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(planeVertexLocation);
            GL.VertexAttribPointer(planeVertexLocation, 3, VertexAttribPointerType.Float, false, plane.Model.Stride * sizeof(float), 0);

            var planeNormalLocation = planeShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(planeNormalLocation);
            GL.VertexAttribPointer(planeNormalLocation, 3, VertexAttribPointerType.Float, false, plane.Model.Stride * sizeof(float), 3 * sizeof(float));

            var planeTexCoordLocation = planeShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(planeTexCoordLocation);
            GL.VertexAttribPointer(planeTexCoordLocation, 2, VertexAttribPointerType.Float, false, plane.Model.Stride * sizeof(float), 6 * sizeof(float));

            planeTexture = Texture.LoadFromFile("Resources/texture.png");
            planeTexture.Use(TextureUnit.Texture0);

            planeShader.SetInt("texture0", 0);
        }

        private void SetupClouds()
        {
            cloudVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(cloudVertexArrayObject);

            cloudVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, cloudVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, clouds[0].Model.Vertices.Length * sizeof(float), clouds[0].Model.Vertices, BufferUsageHint.StaticDraw);

            cloudShader = new Shader("Shaders/ColorShader/shader.vert", "Shaders/ColorShader/shader.frag");
            cloudShader.Use();

            var cloudVertexLocation = cloudShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(cloudVertexLocation);
            GL.VertexAttribPointer(cloudVertexLocation, 3, VertexAttribPointerType.Float, false, clouds[0].Model.Stride * sizeof(float), 0);

            var cloudNormalLocation = cloudShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(cloudNormalLocation);
            GL.VertexAttribPointer(cloudNormalLocation, 3, VertexAttribPointerType.Float, false, clouds[0].Model.Stride * sizeof(float), 3 * sizeof(float));
        }

        private void SetupMainLights()
        {
            mainLightVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(mainLightVertexArrayObject);

            mainLightVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, mainLightVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, mainLights[0].Model.Vertices.Length * sizeof(float), mainLights[0].Model.Vertices, BufferUsageHint.StaticDraw);

            mainLightShader = new Shader("Shaders/LightShader/shader.vert", "Shaders/LightShader/shader.frag");
            mainLightShader.Use();

            var mainLightVertexLocation = mainLightShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(mainLightVertexLocation);
            GL.VertexAttribPointer(mainLightVertexLocation, 3, VertexAttribPointerType.Float, false, mainLights[0].Model.Stride * sizeof(float), 0);
        }

        private void SetupRings()
        {
            ringVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(ringVertexArrayObject);

            ringVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ringVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, rings[0].Model.Vertices.Length * sizeof(float), rings[0].Model.Vertices, BufferUsageHint.StaticDraw);

            ringShader = new Shader("Shaders/ColorShader/shader.vert", "Shaders/ColorShader/shader.frag");
            ringShader.Use();

            var ringVertexLocation = ringShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(ringVertexLocation);
            GL.VertexAttribPointer(ringVertexLocation, 3, VertexAttribPointerType.Float, false, rings[0].Model.Stride * sizeof(float), 0);

            var ringNormalLocation = ringShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(ringNormalLocation);
            GL.VertexAttribPointer(ringNormalLocation, 3, VertexAttribPointerType.Float, false, rings[0].Model.Stride * sizeof(float), 3 * sizeof(float));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawPlane();

            DrawClouds();

            DrawMainLights();

            DrawRings();

            angle += (float)e.Time;

            planeMovementOrchestrator.UpdatePosition(plane, ref planeDirection, (float)e.Time);

            SwapBuffers();
        }

        private void DrawPlane()
        {
            GL.BindVertexArray(planeVertexArrayObject);

            planeTexture.Use(TextureUnit.Texture0);
            planeShader.Use();

            planeShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            planeShader.SetVector3("lightPos", new Vector3(new Vector4(mainLights[0].Position, 1) * Matrix4.CreateRotationZ(angle)));
            planeShader.SetVector3("viewPos", camera.Position);

            planeShader.SetMatrix4("model", plane.ModelMatrix);
            planeShader.SetMatrix4("view", camera.GetViewMatrix());
            planeShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            planeShader.SetMatrix4("transposedInversedModel", Matrix4.Transpose(Matrix4.Invert(plane.ModelMatrix)));

            GL.DrawArrays(PrimitiveType.Triangles, 0, plane.Model.Vertices.Length / plane.Model.Stride);
        }

        private void DrawClouds()
        {
            GL.BindVertexArray(cloudVertexArrayObject);

            cloudShader.Use();
            FillShaderArguments(cloudShader, new Vector3(1.0f));

            foreach (var cloud in clouds)
            {
                cloudShader.SetMatrix4("model", cloud.ModelMatrix);
                cloudShader.SetMatrix4("transposedInversedModel", Matrix4.Transpose(Matrix4.Invert(cloud.ModelMatrix)));
                GL.DrawArrays(PrimitiveType.Triangles, 0, cloud.Model.Vertices.Length / cloud.Model.Stride);
            }
        }

        private void FillShaderArguments(Shader shader, Vector3 objectColor)
        {
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            shader.SetVector3("viewPos", camera.Position);

            // Point lights
            shader.SetVector3($"pointLights[{0}].position", new Vector3(new Vector4(mainLights[0].Position, 1) * Matrix4.CreateRotationZ(angle)));
            shader.SetVector3($"pointLights[{0}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            shader.SetVector3($"pointLights[{0}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            shader.SetVector3($"pointLights[{0}].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat($"pointLights[{0}].constant", 1.0f);
            shader.SetFloat($"pointLights[{0}].linear", 0.0014f);
            shader.SetFloat($"pointLights[{0}].quadratic", 0.000007f);

            shader.SetVector3($"pointLights[{1}].position", new Vector3(new Vector4(mainLights[1].Position, 1) * Matrix4.CreateRotationZ(angle)));
            shader.SetVector3($"pointLights[{1}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            shader.SetVector3($"pointLights[{1}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            shader.SetVector3($"pointLights[{1}].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat($"pointLights[{1}].constant", 1.0f);
            shader.SetFloat($"pointLights[{1}].linear", 0.0045f);
            shader.SetFloat($"pointLights[{1}].quadratic", 0.000075f);

            //// Spot light
            //shader.SetVector3("spotLight.position", camera.Position);
            //shader.SetVector3("spotLight.direction", camera.Front);
            //shader.SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            //shader.SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            //shader.SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            //shader.SetFloat("spotLight.constant", 1.0f);
            //shader.SetFloat("spotLight.linear", 0.09f);
            //shader.SetFloat("spotLight.quadratic", 0.032f);
            //shader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            //shader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

            shader.SetVector3("objectColor", objectColor);
        }

        float angle = 0.0f;
        private void DrawMainLights()
        {
            GL.BindVertexArray(mainLightVertexArrayObject);

            mainLightShader.Use();

            mainLightShader.SetMatrix4("view", camera.GetViewMatrix());
            mainLightShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            foreach (var light in mainLights)
            {
                mainLightShader.SetMatrix4("model", light.ModelMatrix * Matrix4.CreateRotationZ(angle / 10));
                GL.DrawArrays(PrimitiveType.Triangles, 0, light.Model.Vertices.Length / light.Model.Stride);
            }
        }

        private void DrawRings()
        {
            GL.BindVertexArray(ringVertexArrayObject);

            ringShader.Use();

            FillShaderArguments(ringShader, new Vector3(0.2f, 1.0f, 0.2f));

            foreach (var ring in rings)
            {
                ringShader.SetMatrix4("model", ring.ModelMatrix);
                ringShader.SetMatrix4("transposedInversedModel", Matrix4.Transpose(Matrix4.Invert(ring.ModelMatrix)));
                GL.DrawArrays(PrimitiveType.Triangles, 0, ring.Model.Vertices.Length / ring.Model.Stride);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            var mouse = MouseState;

            if (isFirstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                isFirstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
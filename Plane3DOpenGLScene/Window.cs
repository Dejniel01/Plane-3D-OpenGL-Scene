using OpenTK.Windowing.Desktop;
using Plane3DOpenGLScene.Structures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.InteropServices;
using MovementOrchestratorLib;
using Assimp.Unmanaged;
using OpenGLHelperClassLib;

namespace Plane3D.Plane3DOpenGLScene
{
    internal class Window : GameWindow
    {
        #region Private fields

        #region Scene objects fields

        private readonly SceneObject plane;

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

        private readonly SceneObject ocean;

        private int oceanVertexBufferObject;

        private int oceanVertexArrayObject;

        private Shader oceanShader;

        private Texture oceanTexture;

        #endregion

        #region Camera fields

        private Camera activeCamera;

        private readonly Camera[] cameras;

        private bool isFirstMove = true;

        private Vector2 lastPos;

        #endregion

        #region Layout arrays

        private readonly float[] cloudScales = new float[]
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
            0.013f,
            0.01f,
            0.015f,
            0.01f,
            0.005f,
            0.013f,
            0.015f,
            0.011f,
            0.02f,
            0.01f,
            0.013f,
            0.01f,
            0.015f,
            0.01f,
            0.005f,
            0.013f,
            0.015f,
            0.011f,
            0.02f,
            0.01f,
            0.013f,
        };

        private readonly Vector3[] cloudPos = new Vector3[]
        {
            new Vector3(-10, -10, -10),
            new Vector3(-1.5f, 2, 17),
            new Vector3(-10, -10, -10),
            new Vector3(-20, 2, 4),
            new Vector3(3, -10, 2),
            new Vector3(5, -10, -10),
            new Vector3(-10, -10, -10),
            new Vector3(-10, 4, -10),
            new Vector3(10, -10, 5),
            new Vector3(10, 10, 5),
            new Vector3(16, -10, 27),
            new Vector3(17, 2, 27),
            new Vector3(18, -10, 29),
            new Vector3(19, 2, 30),
            new Vector3(20, -10, -28),
            new Vector3(16, -10, -20),
            new Vector3(17, -10, -25),
            new Vector3(18, 4, -19),
            new Vector3(19, -10, -28),
            new Vector3(20, 10, 27),
            new Vector3(-24, -10, 30),
            new Vector3(-25, 2, 28),
            new Vector3(-26, -10, 32),
            new Vector3(-27, 2, 27),
            new Vector3(-28, -10, 29),
            new Vector3(-24, -10, -20),
            new Vector3(-25, -10, -30),
            new Vector3(-26, 4, -25),
            new Vector3(-27, -10, -22),
            new Vector3(-28, 10, -29),
        };

        private readonly Vector3[] ringPos = new Vector3[]
        {
            new Vector3(-14.85f, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(-7.76f, 0, 9.31f),
            new Vector3(0, 0, -10),
        };

        private readonly Vector3[] ringRotations = new Vector3[]
        {
            new Vector3(MathHelper.DegreesToRadians(90), 0, 0),
            new Vector3(0, 0, MathHelper.DegreesToRadians(90)),
            new Vector3(MathHelper.DegreesToRadians(90), MathHelper.DegreesToRadians(45), 0),
            new Vector3(0, 0, MathHelper.DegreesToRadians(90)),
        };

        #endregion

        private Vector3 planeDirection = new(-1, 0, 0);

        private readonly PlaneMovementOrchestrator planeMovementOrchestrator = new();

        private float angle = 0;
        private bool isSunVisible = true;
        private bool isMoonVisible = false;

        private float spotlightsOffsetHorizontal = 0;
        private float spotlightsOffsetVertical = 0;

        #endregion

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            plane = SceneObjectFactory.CreatePlaneObject();
            plane.Scale = 0.25f;

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
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
                SceneObjectFactory.CreateCloudObject(),
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
                rings[i].Scale = 1.5f;
                rings[i].Position = ringPos[i];
                rings[i].Rotation = ringRotations[i];
            }

            ocean = SceneObjectFactory.CreateOceanObject();
            ocean.Position = new Vector3(0, -20, 0);
            ocean.Scale = 2;

            cameras = new Camera[]
            {
                new Camera(new Vector3(-15, 15, 15), Size.X / (float)Size.Y, new Vector3(0)),
                new Camera(new Vector3(-15, 15, 15), Size.X / (float)Size.Y, new Vector3(0)),
                new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y),
                new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y),
            };
            activeCamera = cameras[0];
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

            SetupOcean();
        }

        #region OnLoad methods

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

        private void SetupOcean()
        {
            oceanVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(oceanVertexArrayObject);

            oceanVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, oceanVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, ocean.Model.Vertices.Length * sizeof(float), ocean.Model.Vertices, BufferUsageHint.StaticDraw);

            oceanShader = new Shader("Shaders/TextureShader/shader.vert", "Shaders/TextureShader/shader.frag");
            oceanShader.Use();

            var oceanVertexLocation = oceanShader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(oceanVertexLocation);
            GL.VertexAttribPointer(oceanVertexLocation, 3, VertexAttribPointerType.Float, false, ocean.Model.Stride * sizeof(float), 0);

            var oceanNormalLocation = oceanShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(oceanNormalLocation);
            GL.VertexAttribPointer(oceanNormalLocation, 3, VertexAttribPointerType.Float, false, ocean.Model.Stride * sizeof(float), 3 * sizeof(float));

            var oceanTexCoordLocation = oceanShader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(oceanTexCoordLocation);
            GL.VertexAttribPointer(oceanTexCoordLocation, 2, VertexAttribPointerType.Float, false, ocean.Model.Stride * sizeof(float), 6 * sizeof(float));

            oceanTexture = Texture.LoadFromFile("Resources/ocean.png");
            oceanTexture.Use(TextureUnit.Texture0);

            oceanShader.SetInt("texture0", 0);
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

        #endregion

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawPlane();

            DrawClouds();

            DrawMainLights();

            DrawRings();

            DrawOcean();

            isSunVisible = new Vector3(new Vector4(mainLights[0].Position, 1) * Matrix4.CreateRotationZ(angle / 10)).Y >= ocean.Position.Y;
            isMoonVisible = new Vector3(new Vector4(mainLights[1].Position, 1) * Matrix4.CreateRotationZ(angle / 10)).Y >= ocean.Position.Y;

            if (isSunVisible && isMoonVisible)
                GL.ClearColor(0.2078f, 0.298f, 0.5255f, 1.0f);
            else if (isSunVisible)
                GL.ClearColor(0.5294f, 0.7961f, 0.8706f, 1.0f);
            else if (isMoonVisible)
                GL.ClearColor(0.03529f, 0.04705f, 0.2431f, 1.0f);

            angle += (float)e.Time;

            planeMovementOrchestrator.UpdatePosition(plane, ref planeDirection, (float)e.Time);

            cameras[1].LookAt = (new Vector4(0, 0, 0, 1) * plane.ModelMatrix).Xyz;
            cameras[2].Position = (new Vector4(5, 5, 0, 1) * plane.ModelMatrix).Xyz;
            cameras[2].LookAt = (new Vector4(-5, 0, 0, 1) * plane.ModelMatrix).Xyz;

            SwapBuffers();
        }

        #region OnRenderFrame methods

        private void DrawPlane()
        {
            GL.BindVertexArray(planeVertexArrayObject);

            planeTexture.Use(TextureUnit.Texture0);
            planeShader.Use();

            FillShaderArguments(planeShader);

            planeShader.SetMatrix4("model", plane.ModelMatrix);
            planeShader.SetMatrix4("transposedInversedModel", Matrix4.Transpose(Matrix4.Invert(plane.ModelMatrix)));

            GL.DrawArrays(PrimitiveType.Triangles, 0, plane.Model.Vertices.Length / plane.Model.Stride);
        }

        private void DrawOcean()
        {
            GL.BindVertexArray(oceanVertexArrayObject);

            oceanTexture.Use(TextureUnit.Texture0);
            oceanShader.Use();

            FillShaderArguments(oceanShader);

            oceanShader.SetMatrix4("model", ocean.ModelMatrix);
            oceanShader.SetMatrix4("transposedInversedModel", Matrix4.Transpose(Matrix4.Invert(ocean.ModelMatrix)));

            GL.DrawArrays(PrimitiveType.Triangles, 0, ocean.Model.Vertices.Length / ocean.Model.Stride);
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

        private void DrawMainLights()
        {
            GL.BindVertexArray(mainLightVertexArrayObject);

            mainLightShader.Use();

            mainLightShader.SetMatrix4("view", activeCamera.GetViewMatrix());
            mainLightShader.SetMatrix4("projection", activeCamera.GetProjectionMatrix());

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

        private void FillShaderArguments(Shader shader, Vector3? objectColor = null)
        {
            shader.SetMatrix4("view", activeCamera.GetViewMatrix());
            shader.SetMatrix4("projection", activeCamera.GetProjectionMatrix());

            shader.SetVector3("viewPos", activeCamera.Position);

            shader.SetVector3($"pointLights[0].position", new Vector3(new Vector4(mainLights[0].Position, 1) * Matrix4.CreateRotationZ(angle / 10)));
            shader.SetVector3($"pointLights[0].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            shader.SetVector3($"pointLights[0].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            shader.SetVector3($"pointLights[0].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat($"pointLights[0].constant", 1.0f);
            shader.SetFloat($"pointLights[0].linear", 0.0014f);
            shader.SetFloat($"pointLights[0].quadratic", 0.000007f);
            shader.SetInt($"pointLights[0].visible", isSunVisible ? 1 : 0);

            shader.SetVector3($"pointLights[1].position", new Vector3(new Vector4(mainLights[1].Position, 1) * Matrix4.CreateRotationZ(angle / 10)));
            shader.SetVector3($"pointLights[1].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            shader.SetVector3($"pointLights[1].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            shader.SetVector3($"pointLights[1].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat($"pointLights[1].constant", 1.0f);
            shader.SetFloat($"pointLights[1].linear", 0.0045f);
            shader.SetFloat($"pointLights[1].quadratic", 0.000075f);
            shader.SetInt($"pointLights[1].visible", isMoonVisible ? 1 : 0);

            shader.SetVector3("spotLights[0].position", (new Vector4(0, 0, 4, 1) * plane.ModelMatrix).Xyz);
            shader.SetVector3("spotLights[0].direction", planeDirection + Vector3.Normalize(Vector3.Cross(planeDirection, Vector3.UnitY)) * spotlightsOffsetHorizontal + Vector3.UnitY * spotlightsOffsetVertical);
            shader.SetVector3("spotLights[0].ambient", new Vector3(0.0f, 0.0f, 0.0f));
            shader.SetVector3("spotLights[0].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetVector3("spotLights[0].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat("spotLights[0].constant", 1.0f);
            shader.SetFloat("spotLights[0].linear", 0.09f);
            shader.SetFloat("spotLights[0].quadratic", 0.032f);
            shader.SetFloat("spotLights[0].cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            shader.SetFloat("spotLights[0].outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

            shader.SetVector3("spotLights[1].position", (new Vector4(0, 0, -4, 1) * plane.ModelMatrix).Xyz);
            shader.SetVector3("spotLights[1].direction", planeDirection - Vector3.Normalize(Vector3.Cross(planeDirection, Vector3.UnitY)) * spotlightsOffsetHorizontal + Vector3.UnitY * spotlightsOffsetVertical);
            shader.SetVector3("spotLights[1].ambient", new Vector3(0.0f, 0.0f, 0.0f));
            shader.SetVector3("spotLights[1].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetVector3("spotLights[1].specular", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetFloat("spotLights[1].constant", 1.0f);
            shader.SetFloat("spotLights[1].linear", 0.09f);
            shader.SetFloat("spotLights[1].quadratic", 0.032f);
            shader.SetFloat("spotLights[1].cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            shader.SetFloat("spotLights[1].outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));

            if (objectColor != null)
                shader.SetVector3("objectColor", objectColor.Value);
        }

        #endregion

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

            if (input.IsKeyDown(Keys.Up))
            {
                spotlightsOffsetVertical += cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                spotlightsOffsetVertical -= cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Left))
            {
                spotlightsOffsetHorizontal -= cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                spotlightsOffsetHorizontal += cameraSpeed * (float)e.Time;
            }

            if (activeCamera == cameras[3])
            {
                if (input.IsKeyDown(Keys.W))
                {
                    cameras[3].Position += cameras[3].Front * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.S))
                {
                    cameras[3].Position -= cameras[3].Front * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.A))
                {
                    cameras[3].Position -= cameras[3].Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.D))
                {
                    cameras[3].Position += cameras[3].Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.Space))
                {
                    cameras[3].Position += cameras[3].Up * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    cameras[3].Position -= cameras[3].Up * cameraSpeed * (float)e.Time;
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

                    cameras[3].Yaw += deltaX * sensitivity;
                    cameras[3].Pitch -= deltaY * sensitivity;
                }
            }

            if (input.IsKeyDown(Keys.D1))
            {
                activeCamera = cameras[0];
                CursorState = CursorState.Normal;
            }
            if (input.IsKeyDown(Keys.D2))
            {
                activeCamera = cameras[1];
                CursorState = CursorState.Normal;
            }
            if (input.IsKeyDown(Keys.D3))
            {
                activeCamera = cameras[2];
                CursorState = CursorState.Normal;
            }
            if (input.IsKeyDown(Keys.D4))
            {
                activeCamera = cameras[3];
                CursorState = CursorState.Grabbed;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            cameras[3].Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            foreach(var camera in cameras)
                camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
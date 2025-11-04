using System;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTKExperiment;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private Shader shader;

        private Texture texture;

        private Camera camera;

        private CubeMesh cube;

        private PointLight light;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Console.WriteLine("Press E to create a light where you are of a random colour");

            GL.ClearColor(0.2f, 0.5f, 0.9f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            cube = new CubeMesh(new Vector3(0, -1, 0), 1);

            shader = Shader.FromFiles("Shaders/shader.vert", "Shaders/lighting.frag");
            shader.Use();

            texture = Texture.LoadFromFile("Assets/wall.jpg");
            texture.Use(TextureUnit.Texture0);


            shader.SetInt("ourTexture", 0);

            camera = new Camera(new Vector3(0, 0.5f, 4), Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;

            Random random = new Random();
            Vector3 lightCol = new Vector3(
                (float)random.NextDouble() * (float)random.NextDouble(),
                (float)random.NextDouble() * (float)random.NextDouble(),
                (float)random.NextDouble() * (float)random.NextDouble()
            );

            PointLight newLight = new PointLight();
            newLight.lightColor = lightCol;
            newLight.lightPos = camera.Position;

            light = newLight;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            texture.Use(TextureUnit.Texture0);
            shader.Use();

            var model = Matrix4.Identity;
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            string access = "pointLight.";
            shader.SetVector3($"{access}objectColor", new Vector3(1.0f, 1.0f, 1.0f));
            shader.SetVector3($"{access}lightColor", light.lightColor);
            shader.SetVector3($"{access}lightPos", light.lightPos);
            shader.SetVector3($"{access}viewPos", camera.Position);

            GL.BindVertexArray(cube.vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, cube.indices.Length, DrawElementsType.UnsignedInt, 0);  

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyPressed(Keys.E))
            { 
                Random random = new Random();
                Vector3 lightCol = new Vector3(
                    (float)random.NextDouble() * (float)random.NextDouble(),
                    (float)random.NextDouble() * (float)random.NextDouble(),
                    (float)random.NextDouble() * (float)random.NextDouble()
                );

                PointLight newLight = new PointLight();
                newLight.lightColor = lightCol;
                newLight.lightPos = camera.Position;

                light = newLight;
            }

            camera.Update((float)e.Time, KeyboardState, MouseState);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            // We need to update the aspect ratio once the window has been resized.
            camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
using System;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTKExperiment;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace WindowEngine
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;
        private Cube cube;
        private Vector3 cameraPosition;
        private Vector3 lookDirection;
        private int frameNumber;
        private bool wireframe;


        // Constructor
        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            // Set window size to 1280x768
            this.Size = new Vector2i(768, 768);

            this.cube = new Cube(new Vector3(-0.25f, -0.25f, -0.25f), 0.5f);
            this.cameraPosition = new Vector3(0, 0, 1);
            this.lookDirection = new Vector3(0, 0, 0);
            this.frameNumber = 0;
            this.wireframe = false;


            // Center the window on the screen
            this.CenterWindow(this.Size);
        }

        // Called automatically whenever the window is resized
        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        // Called once when the game starts, ideal for loading resources
        protected override void OnLoad()
        {
            base.OnLoad();

            Console.WriteLine("CONTROLS:");
            Console.WriteLine("Key E: Toggle wireframe");

            // Set the background color (RGBA)
            GL.ClearColor(new Color4(0.5f, 0.7f, 0.8f, 1f));
            // backface culling
            GL.Enable(EnableCap.CullFace);
            // Definitely need to re-order the vertices
            GL.CullFace(CullFaceMode.Front);

            // Define a simple triangle in normalized device coordinates (NDC)
            Vertex[] vertices = new Vertex[] // first three vertices are the position, next 3 are colour
            {
                cube.v000, cube.v001, cube.v101, // bottom
                cube.v000, cube.v101, cube.v100,

                cube.v000, cube.v100, cube.v110, // north
                cube.v000, cube.v110, cube.v010,

                cube.v001, cube.v000, cube.v010, // east
                cube.v001, cube.v010, cube.v011,

                cube.v101, cube.v001, cube.v011, // south
                cube.v101, cube.v011, cube.v111,

                cube.v100, cube.v101, cube.v111, // west
                cube.v100, cube.v111, cube.v110,

                cube.v010, cube.v110, cube.v111, // top
                cube.v010, cube.v111, cube.v011,
/*
                -0.5f, -0.5f, 0.0f, bottomLeftCol.X, bottomLeftCol.Y, bottomLeftCol.Z,  // Bottom-left vertex
                0.5f, -0.5f, 0.0f, bottomRightCol.X, bottomRightCol.Y, bottomRightCol.Z,    // Bottom-right vertex
                // without using index buffers, we just duplicate the connected vertices
                0.5f,  0.5f, 0.0f, topRightCol.X, topRightCol.Y, topRightCol.Z,   // Top-right vertex
                -0.5f, 0.5f, 0.0f, topLeftCol.X, topLeftCol.Y, topLeftCol.Z,  // Top-left vertex
                -0.5f, -0.5f, 0.0f, bottomLeftCol.X, bottomLeftCol.Y, bottomLeftCol.Z,   // Bottom-left vertex*/

            };

            // Generate a Vertex Buffer Object (VBO) to store vertex data on GPU
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float) * 6, vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind to prevent accidental modifications

            // Generate a Vertex Array Object (VAO) to store the VBO configuration
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            // Bind the VBO and define the layout of vertex data for shaders
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            const int totalStride = 6 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, totalStride, 0); // vertex shader layout location 0
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, totalStride, 12); // vertex shader layout location 1
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Vertex shader: positions each vertex
            string vertexShaderCode = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition; // Vertex position input
                layout(location = 1) in vec3 aColour; // Vertex position input

                uniform mat4 u_cameraMVP;

                out vec3 colour;

                void main()
                {
                    gl_Position = u_cameraMVP * vec4(aPosition, 1.0);
                    colour = aColour;
                }
            ";

            // Fragment shader: outputs a single color
            string fragmentShaderCode = @"
                #version 330 core
                out vec4 FragColor;
                in vec3 colour;

                void main()
                {
                    FragColor = vec4(colour.r, colour.g, colour.b, 1.0f); // Orange-red color
                }
            ";

            // Compile shaders
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            // Create shader program and link shaders
            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            // Cleanup shaders after linking (no longer needed individually)
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
        }

        // Called every frame to update game logic
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // Handle input, animations, physics, AI, etc.
        }

        // Called every frame to render graphics
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            //Matrix4 projectMatrix = Matrix4.CreatePerspectiveFieldOfView(1.5708f, 1280.0f / 768.0f, 0.01f, 1000);
            // just use view matrix for now

            cameraPosition.X = (float)Math.Sin(((double)frameNumber) / 10000);
            cameraPosition.Y = (float)Math.Sin(((double)frameNumber + 10000) / 10000);
            cameraPosition.Z = (float)Math.Sin(((double)frameNumber + 20000) / 10000);
            Vector3 origin = new Vector3(0, 0, -1);
            Matrix4 viewMatrix = Matrix4.LookAt(origin, origin + cameraPosition, new Vector3(0, 1, 0));
            Matrix4 mvp = viewMatrix;

            base.OnRenderFrame(args);

            // Clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use our shader program
            GL.UseProgram(shaderProgramHandle);

            int location = GL.GetUniformLocation(shaderProgramHandle, "u_cameraMVP");
            GL.UniformMatrix4(location, true, ref mvp);

            // Bind the VAO and draw the triangle
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36); // 36 vertices for 12 triangles to make a cube-ish
            GL.BindVertexArray(0);

            // Display the rendered frame
            SwapBuffers();

            frameNumber += 1;
        }

        // Called when the game is closing or resources need to be released
        protected override void OnUnload()
        {
            // Unbind and delete buffers and shader program
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }

        // Helper function to check for shader compilation errors
        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.E)
            {
                if(this.wireframe){
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    this.wireframe = false;
                }
                else {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    this.wireframe = true;
                }
            }
            base.OnKeyDown(e);
        }
    }
}
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
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;
        private int textureHandle;
        private Cube cube;
        private Vector3 cameraPosition;
        private Vector3 lookDirection;
        private int frameNumber;
        private bool wireframe;
        private int vertexCount;

        // Default wrapping and filtering
        private TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        private TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear;
        private TextureMagFilter magFilter = TextureMagFilter.Linear;

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

            Square bottom = new Square(new Vector3(-0.25f, -0.25f, -0.25f), 0.5f, Vector3.UnitY);
            Square north = new Square(new Vector3(-0.25f, -0.25f, -0.25f), 0.5f, Vector3.UnitX);
            Square east = new Square(new Vector3(-0.25f, -0.25f, -0.25f), 0.5f, Vector3.UnitZ);
            Square south = new Square(new Vector3(0.25f, -0.25f, -0.25f), 0.5f, Vector3.UnitX);
            Square west = new Square(new Vector3(-0.25f, -0.25f, 0.25f), 0.5f, Vector3.UnitZ);
            Square top = new Square(new Vector3(-0.25f, 0.25f, -0.25f), 0.5f, Vector3.UnitY);


            // Define a simple triangle in normalized device coordinates (NDC)
            Vertex[] vertices = new Vertex[] // first three vertices are the position, next 3 are colour
            {
/*                cube.v000, cube.v001, cube.v101, // bottom
                cube.v000, cube.v101, cube.v100,*/
                bottom.v00, bottom.v01, bottom.v11,
                bottom.v00, bottom.v11, bottom.v10,
                
/*                cube.v000, cube.v100, cube.v110, // north
                cube.v000, cube.v110, cube.v010,*/
                north.v00, north.v10, north.v11,
                north.v00, north.v11, north.v01,

/*                cube.v001, cube.v000, cube.v010, // east
                cube.v001, cube.v010, cube.v011,*/
                east.v01, east.v00, east.v10,
                east.v01, east.v10, east.v11,

/*                cube.v101, cube.v001, cube.v011, // south
                cube.v101, cube.v011, cube.v111,*/
                south.v10, south.v00, south.v01,
                south.v10, south.v01, south.v11,

/*                cube.v100, cube.v101, cube.v111, // west
                cube.v100, cube.v111, cube.v110,*/
                west.v00, west.v01, west.v11,
                west.v00, west.v11, west.v10,

/*                cube.v010, cube.v110, cube.v111, // top
                cube.v010, cube.v111, cube.v011,*/
                top.v00, top.v10, top.v11,
                top.v00, top.v11, top.v01
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
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float) * (3 + 3 + 3 + 2), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // Unbind to prevent accidental modifications
            vertexCount = vertices.Length;

            // Generate a Vertex Array Object (VAO) to store the VBO configuration
            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            // Bind the VBO and define the layout of vertex data for shaders
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            const int totalStride = (3 + 3 + 3 + 2) * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, totalStride, 0); // vertex shader layout location 0 position
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, totalStride, 12); // vertex shader layout location 1 normal
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, totalStride, 24); // vertex shader layout location 2 colour
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, totalStride, 36); // vertex shader layout location 2 texture
            GL.EnableVertexAttribArray(3);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Vertex shader: positions each vertex
            string vertexShaderCode = @"
                #version 330 core
                layout(location = 0) in vec3 aPosition; // Vertex position input
                layout(location = 1) in vec3 aNormal; // Vertex normal input
                layout(location = 2) in vec3 aColour; // Vertex colour input
                layout(location = 3) in vec2 aTexCoord; // Vertex texture input

                uniform mat4 u_cameraMVP;

                out vec3 colour;
                out vec2 texCoord;

                void main()
                {
                    gl_Position = u_cameraMVP * vec4(aPosition, 1.0);
                    colour = aColour;
                    texCoord = aTexCoord;
                }
            ";

            // Fragment shader: outputs a single color
            string fragmentShaderCode = @"
                #version 330 core
                out vec4 FragColor;
                in vec3 colour;
                in vec2 texCoord;

                uniform sampler2D ourTexture;

                void main()
                {
                    FragColor = vec4(colour.r, colour.g, colour.b, 1.0f) * texture(ourTexture, texCoord);
                    // FragColor = vec4(colour.r, colour.g, colour.b, 1.0f);
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

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;
            Console.WriteLine(projectDirectory);
            string texturePath = Path.Combine(projectDirectory, "Assets", "wall.jpg");
            Console.WriteLine(texturePath);
            textureHandle = LoadTexture(texturePath);

            Console.WriteLine("X");
            Square sX = new Square(Vector3.Zero, 2, Vector3.UnitX);
            Console.WriteLine("Y");
            Square sY = new Square(Vector3.Zero, 2, Vector3.UnitY);
            Console.WriteLine("Z");
            Square sZ = new Square(Vector3.Zero, 2, Vector3.UnitZ);
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
            cameraPosition.Z = (float)Math.Sin(((double)frameNumber + 10000) / 5000);
            Vector3 origin = new Vector3(0, 0, -1);
            Matrix4 viewMatrix = Matrix4.LookAt(origin, origin + cameraPosition, new Vector3(0, 1, 0));
            Matrix4 mvp = viewMatrix;

            base.OnRenderFrame(args);

            // Clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use our shader program
            GL.UseProgram(shaderProgramHandle);
            // And texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            int location = GL.GetUniformLocation(shaderProgramHandle, "u_cameraMVP");
            GL.UniformMatrix4(location, true, ref mvp);

            // Bind the VAO and draw the triangle
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
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

        private int LoadTexture(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException($"Could not find texture file: {path}");

            int texId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texId);

            // Initial wrap and filter
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            using (Bitmap bmp = new Bitmap(path))
            {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                var data = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb); // fully qualified

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                              OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            return texId;
        }
    }
}
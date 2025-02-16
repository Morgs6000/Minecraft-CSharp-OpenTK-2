using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection.Metadata;

namespace RubyDung.src;

public class Window : GameWindow {
    private int width;
    private int height;

    private Shader shader = new Shader();
    private Shader shader2 = new Shader();
    private Tesselator t = new Tesselator();
    private Tesselator t2 = new Tesselator();

    public Window(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();

        width = ClientSize.X;
        height = ClientSize.Y;
    }

    protected override void OnLoad() {
        base.OnLoad();

        GL.ClearColor(0.5f, 0.8f, 1.0f, 0.0f);

        shader.Load();
        Tile.tile.Load(t, 0, 0, 0);
        t.Load();

        shader2.Load();
        LoadSquare();
        t2.Load();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        shader.Render();
        t.Render();
        SetupCamera();
        Matrix4 view = Matrix4.Identity;
        view *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians((float)GLFW.GetTime() * 100));
        view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(30.0f));
        view *= Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        shader.SetMatrix4("view", view);

        shader2.Render();
        t2.Render();
        SetupOrthoCamera();
        Matrix4 view2 = Matrix4.Identity;
        view2 *= Matrix4.CreateTranslation(1.0f, 1.0f, -3.0f);
        view2 *= Matrix4.CreateScale(48.0f);
        shader2.SetMatrix4("view", view2);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        width = e.Width;
        height = e.Height;
    }

    /* ..:: CAMERA ::.. */

    private void SetupCamera() {
        Matrix4 view = Matrix4.Identity;
        shader.SetMatrix4("view", view);

        Matrix4 projection = Matrix4.Identity;
        projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)width / (float)height, 0.05f, 1000.0f);
        shader.SetMatrix4("projection", projection);
    }

    private void SetupOrthoCamera() {
        Matrix4 view = Matrix4.Identity;
        shader2.SetMatrix4("view", view);

        Matrix4 projection = Matrix4.Identity;
        projection *= Matrix4.CreateOrthographicOffCenter(0.0f, (float)width, 0.0f, (float)height, 100.0f, 300.0f);
        shader2.SetMatrix4("projection", projection);
    }

    /* ..:: QUADRADO ::.. */

    private void LoadSquare() {
        float size = 0.5f;

        // Vértices do quadrado
        t2.Vertex(-size, -size, 0.0f);
        t2.Vertex(size, -size, 0.0f);
        t2.Vertex(size, size, 0.0f);
        t2.Vertex(-size, size, 0.0f);
        t2.Indice();
    }
}

public class Shader() {
    private readonly string vertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec3 aPos;

        uniform mat4 view;
        uniform mat4 projection;

        void main() {
            gl_Position = vec4(aPos, 1.0);
            gl_Position *= view;
            gl_Position *= projection;
        }
    ";

    private readonly string fragmentShaderSource = @"
        #version 330 core
        out vec4 color;

        void main() {
            color = vec4(1.0f, 0.5f, 0.2f, 1.0f); // Laranja
        }
    ";

    private int shaderProgram;

    public void Load() {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompile(vertexShader);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompile(fragmentShader);

        shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int status);
        if(status == 0) {
            var infoLog = GL.GetProgramInfoLog(shaderProgram);
            throw new Exception($"Error linking shader program: {infoLog}");
        }

        GL.DetachShader(shaderProgram, vertexShader);
        GL.DetachShader(shaderProgram, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    private static void CheckShaderCompile(int shader) {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if(success == 0) {
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling shader: {infoLog}");
        }
    }

    public void Render() {
        GL.UseProgram(shaderProgram);
    }

    public void SetMatrix4(string name, Matrix4 matrix) {
        int location = GL.GetUniformLocation(shaderProgram, name);
        GL.UniformMatrix4(location, true, ref matrix);
    }
}

public class Tesselator() {
    private List<float> vertexBuffer = new List<float>();
    private List<int> indiceBuffer = new List<int>();

    private int vertices;

    private int vertexArrayObject;
    private int vertexBufferObject;
    private int elementBufferObject;

    public void Load() {
        /* ..:: Vertex Array Object ::.. */
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        /* ..:: Vertex Buffer Object ::.. */
        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Count * sizeof(float), vertexBuffer.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        /* ..:: Element Buffer Object ::.. */
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indiceBuffer.Count * sizeof(int), indiceBuffer.ToArray(), BufferUsageHint.StaticDraw);
    }

    public void Render() {
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indiceBuffer.Count, DrawElementsType.UnsignedInt, 0);
    }

    public void Vertex(float x, float y, float z) {
        vertexBuffer.Add(x);
        vertexBuffer.Add(y);
        vertexBuffer.Add(z);
    }

    public void Indice() {
        indiceBuffer.Add(0 + vertices);
        indiceBuffer.Add(1 + vertices);
        indiceBuffer.Add(2 + vertices);

        indiceBuffer.Add(0 + vertices);
        indiceBuffer.Add(2 + vertices);
        indiceBuffer.Add(3 + vertices);

        vertices += 4;
    }
}

public class Tile() {
    public static Tile tile = new Tile();

    public void Load(Tesselator t, int x, int y, int z) {
        float x0 = (float)x + 0.0f;
        float y0 = (float)y + 0.0f;
        float z0 = (float)z + 0.0f;

        float x1 = (float)x + 1.0f;
        float y1 = (float)y + 1.0f;
        float z1 = (float)z + 1.0f;

        // x0
        t.Vertex(x0, y0, z0);
        t.Vertex(x0, y0, z1);
        t.Vertex(x0, y1, z1);
        t.Vertex(x0, y1, z0);
        t.Indice();

        // x1
        t.Vertex(x1, y0, z1);
        t.Vertex(x1, y0, z0);
        t.Vertex(x1, y1, z0);
        t.Vertex(x1, y1, z1);
        t.Indice();

        // y0
        t.Vertex(x0, y0, z0);
        t.Vertex(x1, y0, z0);
        t.Vertex(x1, y0, z1);
        t.Vertex(x0, y0, z1);
        t.Indice();

        // y1
        t.Vertex(x0, y1, z1);
        t.Vertex(x1, y1, z1);
        t.Vertex(x1, y1, z0);
        t.Vertex(x0, y1, z0);
        t.Indice();

        // z0
        t.Vertex(x1, y0, z0);
        t.Vertex(x0, y0, z0);
        t.Vertex(x0, y1, z0);
        t.Vertex(x1, y1, z0);
        t.Indice();

        // z1
        t.Vertex(x0, y0, z1);
        t.Vertex(x1, y0, z1);
        t.Vertex(x1, y1, z1);
        t.Vertex(x0, y1, z1);
        t.Indice();
    }
}

public class Program() {
    private static void Main(string[] args) {
        GameWindowSettings gws = GameWindowSettings.Default;

        NativeWindowSettings nws = NativeWindowSettings.Default;
        nws.ClientSize = (1024, 768);

        new Window(gws, nws).Run();
    }
}

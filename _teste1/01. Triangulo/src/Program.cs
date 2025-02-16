using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RubyDung.src;

public class Program : GameWindow {
    public Program(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();
    }

    protected override void OnLoad() {
        base.OnLoad();

        GL.ClearColor(0.5f, 0.8f, 1.0f, 0.0f);

        LoadShader();
        LoadTesselator();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        RenderShader();
        RenderTesselator();

        SwapBuffers();
    }

    /* ..:: SHADER ::.. */

    private readonly string vertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec3 aPos;

        void main() {
            gl_Position = vec4(aPos, 1.0);
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

    private void LoadShader() {
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

    private void RenderShader() {
        GL.UseProgram(shaderProgram);
    }

    /* ..:: TESSELATOR ::.. */

    private readonly float[] Vertices = {
        // Posições
        -0.5f, -0.5f, 0.0f,
         0.5f, -0.5f, 0.0f,
         0.0f,  0.5f, 0.0f
    };

    private int vertexArrayObject;
    private int vertexBufferObject;

    private void LoadTesselator() {
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    private void RenderTesselator() {
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    /* ..:: MAIN ::.. */

    private static void Main(string[] args) {
        GameWindowSettings gws = GameWindowSettings.Default;

        NativeWindowSettings nws = NativeWindowSettings.Default;
        nws.ClientSize = (1024, 768);

        new Program(gws, nws).Run();
    }
}

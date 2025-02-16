using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RubyDung.src.level;
using RubyDung.src.level.tile;

namespace RubyDung.src;

public class Window : GameWindow {
    private int width;
    private int height;

    private Shader shader;
    private Shader shaderGUI;
    private Texture texture;
    private LevelRenderer levelRenderer;

    private bool wireframeMode = false;

    public Window(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) {
        CenterWindow();

        width = ClientSize.X;
        height = ClientSize.Y;
    }

    protected override void OnLoad() {
        base.OnLoad();

        GL.ClearColor(0.5f, 0.8f, 1.0f, 0.0f);

        shader = new Shader("../../../src/shaders/Vertex.glsl", "../../../src/shaders/Fragment.glsl");
        shaderGUI = new Shader("../../../src/shaders/Vertex.glsl", "../../../src/shaders/Fragment.glsl");

        texture = new Texture("../../../src/textures/terrain.png");

        levelRenderer = new LevelRenderer();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        LoadDrawGUI();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        if(KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }

        if(KeyboardState.IsKeyDown(Keys.F3) && KeyboardState.IsKeyPressed(Keys.W)) {
            wireframeMode = !wireframeMode;

            shader.GetBool("wireframeMode", wireframeMode);

            GL.PolygonMode(TriangleFace.FrontAndBack, wireframeMode ? PolygonMode.Line : PolygonMode.Fill);

            Console.WriteLine($"O modo Wireframe {(wireframeMode ? "está ligado." : "está desligado.")}");
        }

        ProcessInputDrawGUI();
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Render();
        texture.Render();

        levelRenderer.Render();

        SetupCamera();

        Matrix4 view = Matrix4.Identity;
        view *= Matrix4.CreateTranslation(-0.5f, -0.5f, -0.5f);
        view *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians((float)GLFW.GetTime() * 100));
        view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(30.0f));
        view *= Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        shader.SetMatrix4("view", view);

        RenderDrawGUI();

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e) {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        width = e.Width;
        height = e.Height;
    }

    private void SetupCamera() {
        Matrix4 view = Matrix4.Identity;
        shader.SetMatrix4("view", view);

        Matrix4 projection = Matrix4.Identity;
        projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), (float)width / (float)height, 0.05f, 1000.0f);
        shader.SetMatrix4("projection", projection);
    }

    private void SetupOrthoCamera() {
        Matrix4 view = Matrix4.Identity;
        shaderGUI.SetMatrix4("view", view);

        Matrix4 projection = Matrix4.Identity;
        projection *= Matrix4.CreateOrthographicOffCenter(0.0f, (float)width, 0.0f, (float)height, 100.0f, 300.0f);
        shaderGUI.SetMatrix4("projection", projection);
    }

    private Tesselator tesselator;
    private int paintTexture = 1;

    private void LoadDrawGUI() {
        shaderGUI.Render();

        SetupOrthoCamera();

        Matrix4 view = Matrix4.Identity;
        view *= Matrix4.CreateTranslation(1.5f, -0.5f, -0.5f);
        view *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(45.0f));
        view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(30.0f));
        view *= Matrix4.CreateScale(48.0f, 48.0f, 48.0f);
        view *= Matrix4.CreateTranslation((float)(width - 48.0f), (float)(height - 48.0f), 0.0f);
        view *= Matrix4.CreateTranslation(0.0f, 0.0f, -200.0f);
        shaderGUI.SetMatrix4("view", view);

        tesselator = new Tesselator();
        tesselator.Init();
        Tile.tiles[paintTexture].Load(tesselator, -2, 0, 0);

        tesselator.Vertex(0.0f, 0.0f, 0.0f);
        tesselator.Vertex(1.0f, 0.0f, 0.0f);
        tesselator.Vertex(1.0f, 1.0f, 0.0f);
        tesselator.Vertex(0.0f, 1.0f, 0.0f);
        tesselator.Indice();

        tesselator.Load();
    }

    private void RenderDrawGUI() {
        shaderGUI.Render();
        texture.Render();
        tesselator.Render();
    }

    private void UpdateDrawGUI() {
        tesselator.Init();
        Tile.tiles[paintTexture].Load(tesselator, -2, 0, 0);
        tesselator.Load();
    }

    private void ProcessInputDrawGUI() {
        if(KeyboardState.IsKeyPressed(Keys.D1)) {
            paintTexture = 1;
            UpdateDrawGUI();
        }
        if(KeyboardState.IsKeyPressed(Keys.D2)) {
            paintTexture = 3;
            UpdateDrawGUI();
        }
        if(KeyboardState.IsKeyPressed(Keys.D3)) {
            paintTexture = 4;
            UpdateDrawGUI();
        }
        if(KeyboardState.IsKeyPressed(Keys.D4)) {
            paintTexture = 5;
            UpdateDrawGUI();
        }
    }
}

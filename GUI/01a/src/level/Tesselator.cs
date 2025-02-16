using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace RubyDung.src.level;

public class Tesselator {
    private List<float> vertexBuffer = new List<float>();
    private List<int> indiceBuffer = new List<int>();
    private List<float> texCoordBuffer = new List<float>();
    private List<float> colorBuffer = new List<float>();

    private int vertices = 0;

    private float u;
    private float v;

    private float r;
    private float g;
    private float b;

    private bool hasTexture = false;
    private bool hasColor = false;

    private int VertexArrayObject;
    private int VertexBufferObject;
    private int ElementBufferObject;
    private int TextureBufferObject;
    private int ColorBufferObject;

    public void Load() {
        /* ..:: Vertex Array Object ::.. */
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        /* ..:: Vertex Buffer Object ::.. */
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Count * sizeof(float), vertexBuffer.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);

        /* ..:: Element Buffer Object ::.. */
        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indiceBuffer.Count * sizeof(int), indiceBuffer.ToArray(), BufferUsageHint.StaticDraw);

        /* ..:: Texture Buffer Object ::.. */
        if(hasTexture) {
            TextureBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TextureBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoordBuffer.Count * sizeof(float), texCoordBuffer.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
        }

        /* ..:: Color Buffer Object ::.. */
        if(hasColor) {
            ColorBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, colorBuffer.Count * sizeof(float), colorBuffer.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);
        }
    }

    public void Render() {
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indiceBuffer.Count, DrawElementsType.UnsignedInt, 0);
    }

    private void Clear() {
        vertexBuffer.Clear();
        indiceBuffer.Clear();
        texCoordBuffer.Clear();
        colorBuffer.Clear();

        vertices = 0;
    }

    public void Init() {
        Clear();

        hasTexture = false;
        hasColor = false;
    }

    public void Vertex(float x, float y, float z) {
        vertexBuffer.Add(x);
        vertexBuffer.Add(y);
        vertexBuffer.Add(z);

        if(hasTexture) {
            texCoordBuffer.Add(u);
            texCoordBuffer.Add(v);
        }
        if(hasColor) {
            colorBuffer.Add(r);
            colorBuffer.Add(g);
            colorBuffer.Add(b);
        }
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

    public void Tex(float u, float v) {
        hasTexture = true;

        this.u = u;
        this.v = v;
    }

    public void VertexUV(float x, float y, float z, float u, float v) {
        Tex(u, v);
        Vertex(x, y, z);
    }

    public void Color(float r, float g, float b) {
        hasColor = true;

        this.r = r;
        this.g = g;
        this.b = b;
    }
}

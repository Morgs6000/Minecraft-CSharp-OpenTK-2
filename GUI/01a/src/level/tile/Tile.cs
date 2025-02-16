using System.Numerics;

namespace RubyDung.src.level.tile;

public class Tile {
    public static readonly Tile[] tiles = new Tile[256];

    public static readonly Tile empty = null;
    public static readonly Tile rock = new Tile(1, 1);
    public static readonly Tile grass = new GrassTile(2);
    public static readonly Tile dirt = new Tile(3, 2);
    public static readonly Tile stoneBrick = new Tile(4, 16);
    public static readonly Tile wood = new Tile(5, 4);

    public int tex;
    public Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
    public readonly int id;

    protected Tile(int id) {
        tiles[id] = this;
        this.id = id;
    }

    protected Tile(int id, int tex) : this(id) {
        this.tex = tex;
    }

    public void Load(Tesselator tesselator, int x, int y, int z) {
        // x0
        RenderFace(tesselator, x, y, z, 0);

        // x1
        RenderFace(tesselator, x, y, z, 1);

        // y0
        RenderFace(tesselator, x, y, z, 2);

        // y1
        RenderFace(tesselator, x, y, z, 3);

        // z0
        RenderFace(tesselator, x, y, z, 4);

        // z1
        RenderFace(tesselator, x, y, z, 5);
    }

    protected virtual int GetTexture(int face) {
        return tex;
    }

    protected virtual Vector3 GetColor(int face) {
        return color;
    }

    public void RenderFace(Tesselator tesselator, int x, int y, int z, int face) {
        float x0 = x + 0.0f;
        float y0 = y + 0.0f;
        float z0 = z + 0.0f;

        float x1 = x + 1.0f;
        float y1 = y + 1.0f;
        float z1 = z + 1.0f;

        int tex = GetTexture(face);

        float u0 = tex % 16 / 16.0f;
        float v0 = (float)(16.0f - 1.0f - tex / 16) / 16.0f;

        float u1 = u0 + 1.0f / 16.0f;
        float v1 = v0 + 1.0f / 16.0f;

        Vector3 color = GetColor(face);

        float c1 = 1.0f;
        float c2 = 0.8f;
        float c3 = 0.6f;

        // x0
        if(face == 0) {
            tesselator.Color(c3 * color.X, c3 * color.Y, c3 * color.Z);

            tesselator.VertexUV(x0, y0, z0, u0, v0);
            tesselator.VertexUV(x0, y0, z1, u1, v0);
            tesselator.VertexUV(x0, y1, z1, u1, v1);
            tesselator.VertexUV(x0, y1, z0, u0, v1);

            tesselator.Indice();
        }

        // x1
        if(face == 1) {
            tesselator.Color(c3 * color.X, c3 * color.Y, c3 * color.Z);

            tesselator.VertexUV(x1, y0, z1, u0, v0);
            tesselator.VertexUV(x1, y0, z0, u1, v0);
            tesselator.VertexUV(x1, y1, z0, u1, v1);
            tesselator.VertexUV(x1, y1, z1, u0, v1);

            tesselator.Indice();
        }

        // y0
        if(face == 2) {
            tesselator.Color(c1 * color.X, c1 * color.Y, c1 * color.Z);

            tesselator.VertexUV(x0, y0, z0, u0, v0);
            tesselator.VertexUV(x1, y0, z0, u1, v0);
            tesselator.VertexUV(x1, y0, z1, u1, v1);
            tesselator.VertexUV(x0, y0, z1, u0, v1);

            tesselator.Indice();
        }

        // y1
        if(face == 3) {
            tesselator.Color(c1 * color.X, c1 * color.Y, c1 * color.Z);

            tesselator.VertexUV(x0, y1, z1, u0, v0);
            tesselator.VertexUV(x1, y1, z1, u1, v0);
            tesselator.VertexUV(x1, y1, z0, u1, v1);
            tesselator.VertexUV(x0, y1, z0, u0, v1);

            tesselator.Indice();
        }

        // z0
        if(face == 4) {
            tesselator.Color(c2 * color.X, c2 * color.Y, c2 * color.Z);

            tesselator.VertexUV(x1, y0, z0, u0, v0);
            tesselator.VertexUV(x0, y0, z0, u1, v0);
            tesselator.VertexUV(x0, y1, z0, u1, v1);
            tesselator.VertexUV(x1, y1, z0, u0, v1);

            tesselator.Indice();
        }

        // z1
        if(face == 5) {
            tesselator.Color(c2 * color.X, c2 * color.Y, c2 * color.Z);

            tesselator.VertexUV(x0, y0, z1, u0, v0);
            tesselator.VertexUV(x1, y0, z1, u1, v0);
            tesselator.VertexUV(x1, y1, z1, u1, v1);
            tesselator.VertexUV(x0, y1, z1, u0, v1);

            tesselator.Indice();
        }
    }
}

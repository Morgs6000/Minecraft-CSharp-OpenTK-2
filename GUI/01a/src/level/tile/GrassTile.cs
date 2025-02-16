using System.Numerics;

namespace RubyDung.src.level.tile;

public class GrassTile : Tile {
    public GrassTile(int id) : base(id) {
        tex = 3;
    }

    protected override int GetTexture(int face) {
        if(face == 3) {
            return 0;
        }
        else {
            return face == 2 ? 2 : 3;
        }
    }

    protected override Vector3 GetColor(int face) {
        if(face == 3) {
            return new Vector3(0.0f, 1.0f, 0.0f);
        }
        else {
            return new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}

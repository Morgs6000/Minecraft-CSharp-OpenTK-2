using RubyDung.src.level.tile;

namespace RubyDung.src.level;

public class Chunk {
    private Tesselator tesselator = new Tesselator();

    public void Load() {
        Tile.tiles[2].Load(tesselator, 0, 0, 0);
        tesselator.Load();
    }

    public void Render() {
        tesselator.Render();
    }
}

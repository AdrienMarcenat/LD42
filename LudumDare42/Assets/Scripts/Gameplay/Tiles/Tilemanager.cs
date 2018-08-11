
using UnityEngine;

public class TileManager
{
    public Tile GetTile (int x, int y)
    {
        return ms_InvalidTile;
    }

    public Tile GetTile (Vector2 coordinates)
    {
        return ms_InvalidTile;
    }

    public static Tile ms_InvalidTile = new Tile();
}

public class TileManagerProxy : UniqueProxy<TileManager>
{ }
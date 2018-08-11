
using UnityEngine;
using System.Collections.Generic;

public class TileManager
{
    private Dictionary<TileCoordinates, Tile> m_Tiles;

    public TileManager()
    {
        m_Tiles = new Dictionary<TileCoordinates, Tile> ();
    }

    public Tile GetTile (int x, int y)
    {
        return ms_InvalidTile;
    }

    public Tile GetTile (Vector2 coordinates)
    {
        return ms_InvalidTile;
    }
}

public class TileManagerProxy : UniqueProxy<TileManager>
{ }
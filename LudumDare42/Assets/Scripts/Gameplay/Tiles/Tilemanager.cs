
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
        return null;
    }

    public Tile GetTile (Vector2 coordinates)
    {
        return null;
    }

    public void AddTile (Tile tile)
    {
        this.m_Tiles.Add (tile.GetCoordinates(), tile);
    }
}

public class TileManagerProxy : UniqueProxy<TileManager>
{ }
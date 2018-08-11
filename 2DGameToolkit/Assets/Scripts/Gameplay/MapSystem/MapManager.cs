using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TileType { Normal, LeftOnly, RightOnly, UpOnly, DownOnly };
public enum ObjectType { Tile, Truck, Bin, Goal };
public enum Orientation { Left, Right, Up, Down };

public class MapManager
{
    List<Tile> tiles { get; set; }
    Truck truck { get; set; }
    List<Bin> bins { get; set; }
    List<Goal> goals { get; set; }

    public MapManager()
    {

    }

    public void AddTile(Tile tile)
    {
        this.tiles.Add(tile);
    }

    public void SetTruck(Truck truck)
    {
        this.truck = truck;
    }

    public void AddBin(Bin bin)
    {
        this.bins.Add(bin);
    }

    public void AddGoal(Goal goal)
    {
        this.goals.Add(goal);
    }
}

public class Tile
{
    int[,] position;
    TileType tileType;
    TileObject tileObject;

    public Tile (int x, int y, TileType tileType)
    {
        this.position.SetValue(x, 0);
        this.position.SetValue(y, 1);
        this.tileType = tileType;
    }
}

public class TileObject { }

public class Truck : TileObject
{
    int[,] position;
    Orientation orientation;

    public Truck (int x, int y, Orientation orientation)
    {
        this.position.SetValue(x, 0);
        this.position.SetValue(y, 1);
        this.orientation = orientation;
    }
}

public class Bin : TileObject
{
    int[,] position;
    int order;

    public Bin (int x, int y, int order)
    {
        this.position.SetValue(x, 0);
        this.position.SetValue(y, 1);
        this.order = order;
    }
}

public class Goal : TileObject
{
    int[,] position;
    int order;

    public Goal (int x, int y, int order)
    {
        this.position.SetValue(x, 0);
        this.position.SetValue(y, 1);
        this.order = order;
    }
}

public class MapManagerProxy : UniqueProxy<MapManager>
{ }
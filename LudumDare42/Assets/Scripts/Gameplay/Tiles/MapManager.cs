using UnityEngine;
using System.Collections;

enum TileType { Normal, LeftOnly, RightOnly, UpOnly, DownOnly };
enum ObjectType { Truck, Bin, Goal };
enum Orientation { Left, Right, Up, Down };

public class MapManager
{
    Tile[] tiles;
    Truck truck;
    Bin bin;
    Goal[] goals;
}

public class Tile
{
    int[,] position;
    TileType tileType;
}

public class Truck
{
    int[,] position;
    Orientation orientation;
}

public class Bin
{
    int[,] position;
    int order;
}

public class Goal
{
    int[,] position;
    int order;
}

public class MapManagerProxy : UniqueProxy<MapManager>
{ }
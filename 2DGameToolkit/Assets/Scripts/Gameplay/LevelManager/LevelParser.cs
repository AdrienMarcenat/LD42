using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;


public class LevelParser
{

    protected void ReadFile(string path, string filename)
    {

        string line;

        // Read the file and display it line by line.  
        System.IO.StreamReader file =
            new System.IO.StreamReader(AppendPathSeparator(path) + filename);
        while ((line = file.ReadLine()) != null)
        {
            string[] words = line.Split(' ');

            ObjectType objectType = (ObjectType)Enum.Parse(typeof(ObjectType), (String)words.GetValue(0), true);
            int x = Int32.Parse((String)words.GetValue(1));
            int y = Int32.Parse((String)words.GetValue(2));

            switch (objectType)
            {
                case ObjectType.Tile:
                    TileType tileType = (TileType)Enum.Parse(typeof(TileType), (String)words.GetValue(3), true);
                    Tile tile = new Tile(x, y, tileType);
                    MapManagerProxy.Get().AddTile(tile);
                    break;
                case ObjectType.Truck:
                    Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), (String)words.GetValue(3), true);
                    Truck truck = new Truck(x, y, orientation);
                    MapManagerProxy.Get().SetTruck(truck);
                    break;
                case ObjectType.Goal:
                    int order = Int32.Parse((String)words.GetValue(3));
                    Goal goal = new Goal(x, y, order);
                    MapManagerProxy.Get().AddGoal(goal);
                    break;
                case ObjectType.Bin:
                    int orderBin = Int32.Parse((String)words.GetValue(3));
                    Bin bin = new Bin(x, y, orderBin);
                    MapManagerProxy.Get().AddBin(bin);
                    break;
            }
        } 

        file.Close();
    }

    public string AppendPathSeparator(string filepath)
    {
        if (!filepath.EndsWith(@"\"))
            filepath += @"\";

        return filepath;
    }
}

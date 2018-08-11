using System.Collections;
using System;
using System.IO;

public enum EObjectType { Tile, Truck, Bin, Goal };

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

            EObjectType EObjectType = (EObjectType)Enum.Parse(typeof(EObjectType), (String)words.GetValue(0), true);
            int x = Int32.Parse((String)words.GetValue(1));
            int y = Int32.Parse((String)words.GetValue(2));

            switch (EObjectType)
            {
                case EObjectType.Tile:
                    ETileType ETileType = (ETileType)Enum.Parse(typeof(ETileType), (String)words.GetValue(3), true);
                    Tile tile = new Tile(x, y, ETileType);
                    TileManagerProxy.Get().AddTile(tile);
                    break;
                case EObjectType.Truck:
                    EFacingDirection EFacingDirection = (EFacingDirection)Enum.Parse(typeof(EFacingDirection), (String)words.GetValue(3), true);
                    Truck truck = new Truck(x, y, EFacingDirection);
                    TileManagerProxy.Get().SetTruck(truck);
                    break;
                case EObjectType.Goal:
                    int order = Int32.Parse((String)words.GetValue(3));
                    Goal goal = new Goal(x, y, order);
                    TileManagerProxy.Get().AddGoal(goal);
                    break;
                case EObjectType.Bin:
                    int orderBin = Int32.Parse((String)words.GetValue(3));
                    Bin bin = new Bin(x, y, orderBin);
                    TileManagerProxy.Get().AddBin(bin);
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

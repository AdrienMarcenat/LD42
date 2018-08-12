
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelParser
{
    private static Dictionary<string, ETileType> ms_CharToTileType = new Dictionary<string, ETileType> ()
    {
        { "X", ETileType.None },
        { "N", ETileType.Normal },
        { "C", ETileType.QuarterCircle },
    };

    public static TileCoordinates GenLevel (string filename)
    {
#if UNITY_EDITOR
        filename = "Assets/" + filename;
#endif
        string[] lines = File.ReadAllLines (filename);
        int x = 0;
        int y = 0;
        foreach (string line in lines)
        {
            string[] lienOfTile = line.Split (new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            x = 0;
            foreach (string tileInfo in lienOfTile)
            {
                string[] words = tileInfo.Split (',');

                ETileType tileType = ms_CharToTileType[words[0]];
                if (tileType == ETileType.None)
                {
                    x++;
                    continue;
                }
                GameObject tileGameObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("Tile"));
                tileGameObject.transform.position = new Vector3 (x.ToWorldUnit (), y.ToWorldUnit (), 0);
                Tile tile = tileGameObject.AddComponent<Tile> ();
                tile.SetCoordinates (new TileCoordinates (x, y));
                tile.SetType (tileType);

                TileManagerProxy.Get ().AddTile (tile);

                if (words.Length > 1)
                {
                    ETileObjectType tileObjectType = (ETileObjectType)Enum.Parse (typeof (ETileObjectType), (String)words.GetValue (1), true);
                    GameObject tileObjectGameObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("TileObject_" + words[1]));
                    tileObjectGameObject.transform.position = new Vector3 (x.ToWorldUnit (), y.ToWorldUnit (), 0);
                    TileObject tileObject = tileObjectGameObject.GetComponent<TileObject> ();
                    tileObject.Init (tileObjectType, x, y, words.SubArray (2, -1));
                    tile.SetTileObject (tileObject);
                }

                x++;
            }
            y--;
        }

        return new TileCoordinates (x, y);
    }
}

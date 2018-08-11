
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
        { "W", ETileType.Wall },
        { "S", ETileType.Start },
        { "A", ETileType.Acid },
    };
    
    public static void GenLevel (string filename)
    {
#if UNITY_EDITOR
        filename = "Assets/" + filename;
#endif
        string[] lines = File.ReadAllLines (filename);
        int x = 0;
        int y = 0;
        int xPlayer = 0;
        int yPlayer = 0;
        EFacingDirection playerFacingDirection = EFacingDirection.Right;
        foreach (string line in lines)
        {
            string[] lienOfTile = line.Split (' ');
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
                if (tileType == ETileType.Start)
                {
                    xPlayer = x;
                    yPlayer = y;
                    playerFacingDirection = (EFacingDirection)Enum.Parse (typeof (EFacingDirection), (String)words.GetValue (1), true);
                }
                GameObject tileGameObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("Tile"));
                tileGameObject.transform.position = new Vector3 (x.ToWorldUnit(), y.ToWorldUnit(), 0);
                Tile tile = tileGameObject.AddComponent<Tile> ();
                tile.SetCoordinates (new TileCoordinates (x, y));
                tile.SetType (tileType);
                TileManagerProxy.Get ().AddTile (tile);

                if (words.Length > 1 && tileType != ETileType.Start)
                {
                    ETileObjectType tileObjectType = (ETileObjectType)Enum.Parse (typeof (ETileObjectType), (String)words.GetValue (1), true);
                    GameObject tileObjectGameObject = GameObject.Instantiate (RessourceManager.LoadPrefab("TileObject_" + words[1]));
                    tileObjectGameObject.transform.position = new Vector3 (x.ToWorldUnit (), y.ToWorldUnit (), 0);
                    TileObject tileObject = tileObjectGameObject.GetComponent<TileObject> ();
                    tileObject.Init (tileObjectType, x, y, words.SubArray(2, -1));
                    if (tileObject.CanBeGrabed ())
                    {
                        tile.SetTileObject (tileObject);
                    }
                }

                x++;
            }
            y--;
        }
        
        {
            GameObject playerObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("Player"));
            playerObject.transform.position = new Vector3 (xPlayer.ToWorldUnit(), yPlayer.ToWorldUnit(), 0);
            PlayerController controller = playerObject.AddComponent<PlayerController> ();
            controller.SetFacingDirection (playerFacingDirection);
        }
    }
}

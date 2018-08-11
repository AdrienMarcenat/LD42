
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelParser : MonoBehaviour
{
    [SerializeField] private GameObject m_TilePrefab;
    [SerializeField] private GameObject m_PlayerPrefab;
    [SerializeField] private GameObject m_TileObjectPrefab;

    private static Dictionary<string, ETileType> ms_CharToTileType = new Dictionary<string, ETileType> ()
    {
        { "X", ETileType.None },
        { "N", ETileType.Normal },
        { "W", ETileType.Wall },
        { "G", ETileType.Goal },
        { "S", ETileType.Start },
        { "A", ETileType.Acid },
    };

    private void Awake ()
    {
        TileManagerProxy.Get ().Reset ();
        ReadFile ("Datas/Level1.txt");
    }

    public void ReadFile (string filename)
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
                    continue;
                }
                if (tileType == ETileType.Start)
                {
                    xPlayer = x;
                    yPlayer = y;
                    playerFacingDirection = (EFacingDirection)Enum.Parse (typeof (EFacingDirection), (String)words.GetValue (1), true);
                }
                GameObject tileGameObject = GameObject.Instantiate (m_TilePrefab);
                tileGameObject.transform.position = new Vector3 (x, y, 0);
                Tile tile = tileGameObject.AddComponent<Tile> ();
                tile.SetCoordinates (new TileCoordinates (x, y));
                tile.SetType (tileType);
                TileManagerProxy.Get ().AddTile (tile);

                if (words.Length > 1 && tileType != ETileType.Start)
                {
                    ETileObjectType tileObjectType = (ETileObjectType)Enum.Parse (typeof (ETileObjectType), (String)words.GetValue (1), true);
                    int objectNumber = Int32.Parse ((String)words.GetValue (2));
                    GameObject tileObjectGameObject = GameObject.Instantiate (m_TileObjectPrefab);
                    tileObjectGameObject.transform.position = new Vector3 (x, y, 0);
                    TileObject tileObject = tileObjectGameObject.AddComponent<TileObject> ();
                    tileObject.SetNumber (objectNumber);
                    tileObject.SetType (tileObjectType);
                    tile.SetTileObject (tileObject);
                }

                x++;
            }
            y--;
        }
        
        {
            GameObject playerObject = GameObject.Instantiate (m_PlayerPrefab);
            playerObject.transform.position = new Vector3 (xPlayer, yPlayer, 0);
            PlayerController controller = playerObject.AddComponent<PlayerController> ();
            controller.SetFacingDirection (playerFacingDirection);
        }
    }
}

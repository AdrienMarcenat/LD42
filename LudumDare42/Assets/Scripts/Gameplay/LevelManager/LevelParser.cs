
using System;
using System.IO;
using UnityEngine;

public enum EObjectType { Tile, Player, TileObject };

public class LevelParser : MonoBehaviour
{
    [SerializeField] private GameObject m_TilePrefab;
    [SerializeField] private GameObject m_PlayerPrefab;
    [SerializeField] private GameObject m_TileObjectPrefab;

    private void Awake()
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
        foreach (string line in lines)
        {
            string[] words = line.Split (' ');

            EObjectType EObjectType = (EObjectType)Enum.Parse (typeof (EObjectType), (String)words.GetValue (0), true);
            int x = Int32.Parse ((String)words.GetValue (1));
            int y = Int32.Parse ((String)words.GetValue (2));

            switch (EObjectType)
            {
                case EObjectType.Tile:
                {
                    ETileType tileType = (ETileType)Enum.Parse (typeof (ETileType), (String)words.GetValue (3), true);
                    GameObject tileObject = GameObject.Instantiate (m_TilePrefab);
                    tileObject.transform.position = new Vector3 (x, y, 0);
                    Tile tile = tileObject.AddComponent<Tile> ();
                    tile.SetCoordinates (new TileCoordinates (x, y));
                    tile.SetType (tileType);
                    TileManagerProxy.Get ().AddTile (tile);
                }
                break;
                case EObjectType.Player:
                {
                    EFacingDirection facingDirection = (EFacingDirection)Enum.Parse (typeof (EFacingDirection), (String)words.GetValue (3), true);
                    GameObject playerObject = GameObject.Instantiate (m_PlayerPrefab);
                    playerObject.transform.position = new Vector3 (x, y, 0);
                    PlayerController controller = playerObject.AddComponent<PlayerController> ();
                    controller.SetFacingDirection (facingDirection);
                }
                break;
                case EObjectType.TileObject:
                {
                    ETileObjectType tileObjectType = (ETileObjectType)Enum.Parse (typeof (ETileObjectType), (String)words.GetValue (3), true);
                    int number = Int32.Parse ((String)words.GetValue (4));
                    GameObject tileGameObject = GameObject.Instantiate (m_TileObjectPrefab);
                    tileGameObject.transform.position = new Vector3 (x, y, 0);
                    TileObject tileObject = tileGameObject.AddComponent<TileObject> ();
                    tileObject.SetNumber (number);
                    tileObject.SetType (tileObjectType);
                    TileManagerProxy.Get ().SetTileObject (new TileCoordinates (x, y), tileObject);
                }
                break;
            }
        }
    }
}

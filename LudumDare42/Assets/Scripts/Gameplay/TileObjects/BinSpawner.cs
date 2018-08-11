
using UnityEngine;

public class BinSpawnCommand : Command
{
    public BinSpawnCommand (GameObject actor) : base (actor)
    {
    }

    public override void Execute () { m_Actor.GetComponent<BinSpawner> ().SpawnBin (); }
    public override void Undo () { m_Actor.GetComponent<BinSpawner> ().UnSpawnbin (); }
}

public class BinSpawner : TileObject
{
    private GameObject m_BinPrefab;
    public static int ms_BinNumber;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        this.RegisterAsListener ("Player", typeof (PlayerInputGameEvent));
        m_BinPrefab = RessourceManager.LoadPrefab ("TileObject_Bin");
        ms_BinNumber = 0;
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("Player");
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        string input = inputEvent.GetInput ();
        EInputState state = inputEvent.GetInputState ();
        if (inputEvent.GetInputState () == EInputState.Down)
        {
            switch (input)
            {
                case "BinSpawnRequest":
                    AddBinSpawnCommand ();
                    break;
                default:
                    break;
            }
        }
    }

    private void AddBinSpawnCommand ()
    {
        if (TileManagerProxy.Get ().GetTile (GetCoordinates ()).GetTileObject () == null)
        {
            BinSpawnCommand command = new BinSpawnCommand (gameObject);
            command.Execute ();
            CommandStackProxy.Get ().PushCommand (command);
        }
    }

    public void SpawnBin ()
    {
        GameObject binGameObject = GameObject.Instantiate (m_BinPrefab);
        TileCoordinates coordinates = GetCoordinates ();
        binGameObject.transform.position = new Vector3 (coordinates.x.ToWorldUnit(), coordinates.y.ToWorldUnit (), 0);
        Bin bin = binGameObject.GetComponent<Bin> ();
        bin.Init (ETileObjectType.Bin, coordinates.x, coordinates.y, new string[] { "0" });
        TileManagerProxy.Get ().SetTileObject (coordinates, bin);
        ms_BinNumber++;
    }

    public void UnSpawnbin ()
    {
        Destroy (TileManagerProxy.Get ().GetTile (GetCoordinates ()).GetTileObject ().gameObject);
        ms_BinNumber--;
    }

    public override bool IsObstacle ()
    {
        return false;
    }

    public override bool CanBeGrabed ()
    {
        return false;
    }
}

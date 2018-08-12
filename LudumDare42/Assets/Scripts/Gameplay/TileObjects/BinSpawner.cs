
using UnityEngine;

public class BinSpawnEvent : GameEvent
{
    public BinSpawnEvent (bool shouldSpawn, int binNumber) : base ("BinSpawner")
    {
        m_ShouldSpawn = shouldSpawn;
        m_BinNumber = binNumber;
    }

    public bool ShouldSpawn ()
    {
        return m_ShouldSpawn;
    }

    public int GetBinNumber ()
    {
        return m_BinNumber;

    }
    private bool m_ShouldSpawn;
    private int m_BinNumber;
}

public class BinSpawnCommand : Command
{
    public BinSpawnCommand (GameObject actor) : base (actor)
    {
    }

    public override void Execute () { new BinSpawnEvent (true, 0).Push (); }
    public override void Undo () { new BinSpawnEvent (false, 0).Push (); }
}

public class BinSpawner : TileObject
{
    private GameObject m_BinPrefab;
    public static int ms_BinNumber;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        this.RegisterAsListener ("BinSpawner", typeof (BinSpawnEvent));
        m_BinPrefab = RessourceManager.LoadPrefab ("TileObject_Bin");
        ms_BinNumber = 0;
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("BinSpawner");
    }

    public void OnGameEvent (BinSpawnEvent spawnEvent)
    {
        if (spawnEvent.ShouldSpawn ())
        {
            SpawnBin (spawnEvent.GetBinNumber ());
        }
        else
        {
            UnSpawnbin ();
        }
    }

    public void SpawnBin (int binNumber)
    {
        if (!TileManagerProxy.Get ().GetTile (GetCoordinates ()).IsEmpty ())
        {
            return;
        }

        GameObject binGameObject = GameObject.Instantiate (m_BinPrefab);
        TileCoordinates coordinates = GetCoordinates ();
        binGameObject.transform.position = new Vector3 (coordinates.x.ToWorldUnit (), coordinates.y.ToWorldUnit (), 0);
        Bin bin = binGameObject.GetComponent<Bin> ();
        bin.Init (ETileObjectType.Bin, coordinates.x, coordinates.y, new string[] { binNumber.ToString() });
        bin.SetIsSpawned (true);
        TileManagerProxy.Get ().SetTileObject (coordinates, bin);
        ms_BinNumber++;
    }

    public void UnSpawnbin ()
    {
        if (TileManagerProxy.Get ().GetTile (GetCoordinates ()).GetTileObject () == null)
        {
            return;
        }

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


using UnityEngine;

public class BinSpawner : TileObject
{
    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        this.RegisterAsListener ("BinGoal", typeof (BinGoalEvent));
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("BinGoal");
    }

    public void OnGameEvent (BinGoalEvent binGoalEvent)
    {
        GameObject binGameObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("TileObject_Bin"));
        TileCoordinates coordinates = GetCoordinates ();
        binGameObject.transform.position = new Vector3 (coordinates.x, coordinates.y, 0);
        Bin bin = binGameObject.GetComponent<Bin> ();
        bin.Init (ETileObjectType.Bin, coordinates.x, coordinates.y, new string[] { "0" });
        TileManagerProxy.Get().SetTileObject (coordinates, bin);
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

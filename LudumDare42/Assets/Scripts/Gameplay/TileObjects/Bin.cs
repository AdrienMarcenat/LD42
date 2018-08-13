
using System;
using UnityEngine;

public class Bin : TileObject
{
    [SerializeField] private int m_Number;
    private int m_SpawnedAtCommandNumber = -1;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        m_Number = Int32.Parse ((String)args.GetValue (0));
        GameObject numberObject = GameObject.Instantiate (RessourceManager.LoadPrefab ("number"));
        numberObject.transform.SetParent (transform, false);
        numberObject.GetComponent<SpriteRenderer> ().sprite = RessourceManager.LoadSprite ("number-" + (m_Number + 1), 0);
        GoalManagerProxy.Get ().RegisterBin (this);
    }

    private void OnDestroy ()
    {
        GoalManagerProxy.Get ().UnegisterBin (this);
    }

    public override bool IsObstacle ()
    {
        return true;
    }

    public override bool CanBeGrabed ()
    {
        return true;
    }

    public int GetNumber ()
    {
        return m_Number;
    }

    public bool IsSpawnedAtCommandNumber ()
    {
        return m_SpawnedAtCommandNumber == CommandStackProxy.Get ().GetNumberOfCommand () + 1;
    }

    public void SetSpawnedCommandNumber ()
    {
        m_SpawnedAtCommandNumber = CommandStackProxy.Get ().GetNumberOfCommand (); ;
    }
}


using System;
using UnityEngine;

public class Bin : TileObject
{
    [SerializeField] private int m_Number;
    private bool m_IsSpawned = false;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        m_Number = Int32.Parse ((String)args.GetValue (0));
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

    public int GetNumber()
    {
        return m_Number;
    }

    public bool IsSpawned()
    {
        return m_IsSpawned;
    }

    public void SetIsSpawned(bool isSpawned)
    {
        m_IsSpawned = isSpawned;
    }
}

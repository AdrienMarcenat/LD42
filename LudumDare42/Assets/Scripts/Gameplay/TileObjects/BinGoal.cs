
using System;
using UnityEngine;

public class BinGoal : TileObject
{
    [SerializeField] private int m_Number;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        m_Number = Int32.Parse ((String)args.GetValue (0));
        GoalManagerProxy.Get ().RegisterBinGoal (this);
    }

    public override bool IsObstacle ()
    {
        return false;
    }

    public override bool CanBeGrabed ()
    {
        return false;
    }

    public int GetNumber ()
    {
        return m_Number;
    }
}


using System;

public class BinGoalEvent : GameEvent
{
    public BinGoalEvent () : base("BinGoal")
    {
    }
}

public class BinGoal : TileObject
{
    private int m_Number;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        m_Number = Int32.Parse ((String)args.GetValue (0));
        this.RegisterAsListener ("Bin", typeof (BinEvent));
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("Bin");
    }

    public void OnGameEvent(BinEvent binEvent)
    {
        Bin bin = binEvent.GetBin ();
        if (bin.GetNumber() == m_Number && bin.GetCoordinates() == GetCoordinates())
        {
            new BinGoalEvent ().Push ();
            Destroy (this);
        }
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

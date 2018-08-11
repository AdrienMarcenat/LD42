
using System;

public class BinEvent : GameEvent
{
    public BinEvent (Bin bin) : base ("Bin")
    {
        m_Bin = bin;
    }
    public Bin GetBin ()
    {
        return m_Bin;
    }

    private Bin m_Bin;
}

public class Bin : TileObject
{
    private int m_Number;

    public override void Init (ETileObjectType type, int x, int y, string[] args)
    {
        base.Init (type, x, y, args);
        m_Number = Int32.Parse ((String)args.GetValue (0));
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
}


using UnityEngine;

public class MoveCommand : Command
{
    public override void Execute () { m_Actor.GetComponent<PlayerController> ().Move (m_XDir, m_YDir); }
    public override void Undo () { m_Actor.GetComponent<PlayerController> ().MoveInstant (-m_XDir, -m_YDir); }

    public MoveCommand (GameObject actor, int xDir, int yDir) : base (actor)
    {
        m_XDir = xDir;
        m_YDir = yDir;
    }

    private int m_XDir;
    private int m_YDir;
}

public class TurnCommand : Command
{
    public override void Execute () { m_Actor.GetComponent<PlayerController> ().Turn (m_IsTurningRight); }
    public override void Undo () { m_Actor.GetComponent<PlayerController> ().TurnInstant (!m_IsTurningRight); }

    public TurnCommand (GameObject actor, bool isTurningRight) : base (actor)
    {
        m_IsTurningRight = isTurningRight;
    }

    private bool m_IsTurningRight;
}

public class ToggleGrabCommand : Command
{
    public ToggleGrabCommand (GameObject actor) : base (actor)
    {
    }

    public override void Execute () { m_Actor.GetComponent<PlayerController> ().ToggleGrab (); }
    public override void Undo () { m_Actor.GetComponent<PlayerController> ().ToggleGrab (); }
}
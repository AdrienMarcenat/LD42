﻿
using UnityEngine;

public class MoveCommand : Command
{
    public override void Execute ()
    {
        PlayerController controller = m_Actor.GetComponent<PlayerController> ();
        if(controller.IsHolding())
        {
            LevelManagerProxy.Get ().IncreaseNumberOfMove ();
        }
        controller.Move (m_XDir, m_YDir);
    }

    public override void Undo ()
    {
        PlayerController controller = m_Actor.GetComponent<PlayerController> ();
        if (controller.IsHolding ())
        {
            LevelManagerProxy.Get ().DecreaseNumberOfMove ();
        }
        controller.MoveInstant (-m_XDir, -m_YDir);
    }

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
    public override void Execute ()
    {
        PlayerController controller = m_Actor.GetComponent<PlayerController> ();
        if (controller.IsHolding ())
        {
            LevelManagerProxy.Get ().IncreaseNumberOfMove ();
        }
        controller.Turn (m_NewFacingDirection);
    }
    public override void Undo ()
    {
        PlayerController controller = m_Actor.GetComponent<PlayerController> ();
        if (controller.IsHolding ())
        {
            LevelManagerProxy.Get ().DecreaseNumberOfMove ();
        }
        controller.TurnInstant (m_OldFacingDirection);
    }

    public TurnCommand (GameObject actor, int newFacingDirection, int oldFacingDirection) : base (actor)
    {
        m_NewFacingDirection = newFacingDirection;
        m_OldFacingDirection = oldFacingDirection;
    }

    private int m_NewFacingDirection;
    private int m_OldFacingDirection;
}

public class ToggleGrabCommand : Command
{
    public ToggleGrabCommand (GameObject actor) : base (actor)
    {
    }

    public override void Execute () { m_Actor.GetComponent<PlayerController> ().ToggleGrab (); }
    public override void Undo () { m_Actor.GetComponent<PlayerController> ().ToggleGrab (true); }
}
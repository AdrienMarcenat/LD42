using System.Collections.Generic;
using UnityEngine;

public enum EFacingDirection
{
    // Keep that order !!!
    Right,
    Up,
    Left,
    Down
}

public class Command
{
    public virtual void Execute (PlayerController actor) { }
    public virtual void Undo (PlayerController actor) { }
};

public class MoveCommand : Command
{
    public override void Execute (PlayerController actor) { actor.Move (m_XDir, m_YDir); }
    public override void Undo (PlayerController actor) { actor.Move (-m_XDir, -m_YDir); }

    public MoveCommand(int xDir, int yDir)
    {
        m_XDir = xDir;
        m_YDir = yDir;
    }

    private int m_XDir;
    private int m_YDir;
}

public class TurnCommand : Command
{
    public override void Execute (PlayerController actor) { actor.Turn (m_NewFacingDirection); }
    public override void Undo (PlayerController actor) { actor.Turn (m_OldFacingDirection); }

    public TurnCommand (int oldFacingDirection, int newFacingDirection)
    {
        m_OldFacingDirection = oldFacingDirection;
        m_NewFacingDirection = newFacingDirection;
    }

    private int m_OldFacingDirection;
    private int m_NewFacingDirection;
}

[RequireComponent (typeof (Collider2D))]
public class PlayerController : MonoBehaviour
{
    private EFacingDirection m_FacingDirection;
    private bool m_IsHoldingSomething;
    private Stack<Command> m_CommandStack;
    
    void Awake ()
    {
        m_FacingDirection = EFacingDirection.Right;
        m_IsHoldingSomething = false;
        m_CommandStack = new Stack<Command> ();
        this.RegisterAsListener ("Player", typeof (PlayerInputGameEvent));
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("Player");
    }

    private void AddMoveCommand(int XDir, int YDir)
    {
        MoveCommand command = new MoveCommand (XDir, YDir);
        command.Execute (this);
        m_CommandStack.Push (command);
    }

    private void AddTurnCommand (int newFacingDirection)
    {
        TurnCommand command = new TurnCommand ((int)m_FacingDirection, newFacingDirection);
        command.Execute (this);
        m_CommandStack.Push (command);
    }

    private void UndoCommand ()
    {
        if (m_CommandStack.Count != 0)
        {
            m_CommandStack.Pop ().Undo (this);
        }
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        string input = inputEvent.GetInput ();
        EInputState state = inputEvent.GetInputState ();
        if (inputEvent.GetInputState () == EInputState.Down)
        {
            switch (input)
            {
                case "Right":
                    AddMoveCommand (1, 0);
                    break;
                case "Left":
                    AddMoveCommand (-1, 0);
                    break;
                case "Up":
                    AddMoveCommand (0, 1);
                    break;
                case "Down":
                    AddMoveCommand (0, -1);
                    break;
                case "TurnRight":
                    AddTurnCommand ((int)EFacingDirection.Right);
                    break;
                case "TurnLeft":
                    AddTurnCommand ((int)EFacingDirection.Left);
                    break;
                case "TurnUp":
                    AddTurnCommand ((int)EFacingDirection.Up);
                    break;
                case "TurnDown":
                    AddTurnCommand ((int)EFacingDirection.Down);
                    break;
                case "Undo":
                    UndoCommand();
                    break;
                default:
                    break;
            }
        }
    }

    public void Move (int xDir, int yDir)
    {
        transform.position = new Vector3 (transform.position.x + xDir, transform.position.y + yDir, transform.position.z);
    }

    public void Turn (int directionToFace)
    {
        if ((int)m_FacingDirection == Modulo (directionToFace + 1, 4))
        {
            transform.Rotate (Vector3.forward, -90);
            m_FacingDirection = (EFacingDirection)directionToFace;
        }
        else if ((int)m_FacingDirection == Modulo (directionToFace - 1, 4))
        {
            transform.Rotate (Vector3.forward, 90);
            m_FacingDirection = (EFacingDirection)directionToFace;
        }
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
        }
    }

    private int Modulo (int a, int b)
    {
        int res = a % b;
        if (res < 0)
        {
            return res + b;
        }
        else
        {
            return res;
        }
    }
}

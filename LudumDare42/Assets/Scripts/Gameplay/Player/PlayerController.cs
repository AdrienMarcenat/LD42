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

    public MoveCommand (int xDir, int yDir)
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

public class ToggleGrabCommand : Command
{
    public override void Execute (PlayerController actor) { actor.ToggleGrab (); }
    public override void Undo (PlayerController actor) { actor.ToggleGrab (); }
}

public class PlayerController : MonoBehaviour
{
    private EFacingDirection m_FacingDirection;
    private bool m_IsHoldingSomething;
    private TileObject m_Bin;
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

    private void AddMoveCommand (int xDir, int yDir)
    {
        if (CanMoveTo (xDir, yDir))
        {
            MoveCommand command = new MoveCommand (xDir, yDir);
            command.Execute (this);
            m_CommandStack.Push (command);
        }
    }

    private void AddTurnCommand (int newFacingDirection)
    {
        if (CanTurn (newFacingDirection))
        {
            TurnCommand command = new TurnCommand ((int)m_FacingDirection, newFacingDirection);
            command.Execute (this);
            m_CommandStack.Push (command);
        }
    }

    private void AddToggleGrabCommand ()
    {
        if (CanToggleGrab ())
        {
            ToggleGrabCommand command = new ToggleGrabCommand ();
            command.Execute (this);
            m_CommandStack.Push (command);
        }
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
                    UndoCommand ();
                    break;
                case "Grab / Ungrab":
                    AddToggleGrabCommand ();
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

    private bool CanMoveTo (int xDir, int yDir)
    {
        Tile nextTruckTile = TileManagerProxy.Get ().GetTile ((int)transform.position.x + xDir, (int)transform.position.y + yDir);
        if (m_Bin == null)
        {
            return nextTruckTile.IsEmpty ();
        }
        else
        {
            TileCoordinates facingCoordinate = GetFacingTileCoordinates ();
            Tile nextBarrelTile = TileManagerProxy.Get ().GetTile (facingCoordinate);
            return nextTruckTile.IsEmpty () && nextBarrelTile.IsEmpty ();
        }
    }

    public void ToggleGrab ()
    {
        Tile facingTile = GetFacingTile ();
        if (m_Bin != null)
        {
            if (facingTile.IsEmpty ())
            {
                m_Bin.transform.SetParent (null, true);
                m_Bin = null;
            }
        }
        else
        {
            TileObject bin = facingTile.GetTileObject ();
            if (bin != null && bin.GetObjectType() == ETileObjectType.Bin)
            {
                bin.transform.SetParent (transform, true);
                m_Bin = bin;
            }
        }
    }

    private bool CanToggleGrab ()
    {
        Tile facingTile = GetFacingTile ();
        return (m_Bin != null && facingTile.IsEmpty ()) || (m_Bin == null && facingTile.HasBin ());
    }

    private Tile GetFacingTile ()
    {
        return TileManagerProxy.Get ().GetTile (GetFacingTileCoordinates ());
    }

    private TileCoordinates GetFacingTileCoordinates ()
    {
        TileCoordinates currentTileCoordinates = transform.position;
        TileCoordinates facingTileOffset = ms_NeighboorTiles[m_FacingDirection];

        return currentTileCoordinates + facingTileOffset;
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

    private bool CanTurn (int newFacingDirection)
    {
        if (m_Bin == null)
        {
            return true;
        }
        else
        {
            TileCoordinates currentTileCoordinates = transform.position;
            TileCoordinates oldFacingTileOffset = ms_NeighboorTiles[m_FacingDirection];
            TileCoordinates newFacingOffset = ms_NeighboorTiles[(EFacingDirection)newFacingDirection];
            TileCoordinates passingTileOffset = oldFacingTileOffset + newFacingOffset;

            return TileManagerProxy.Get ().GetTile (currentTileCoordinates + newFacingOffset).IsEmpty ()
                && TileManagerProxy.Get ().GetTile (currentTileCoordinates + passingTileOffset).IsEmpty ();
        }
    }

    public void SetFacingDirection (EFacingDirection newFacingOrientation)
    {
        m_FacingDirection = newFacingOrientation;
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

    private static Dictionary<EFacingDirection, TileCoordinates> ms_NeighboorTiles = new Dictionary<EFacingDirection, TileCoordinates> ()
    {
        { EFacingDirection.Right, new TileCoordinates(1, 0) },
        { EFacingDirection.Left,  new TileCoordinates(-1, 0) },
        { EFacingDirection.Up,    new TileCoordinates(0, 1) },
        { EFacingDirection.Down,  new TileCoordinates(0, -1) },
    };
}

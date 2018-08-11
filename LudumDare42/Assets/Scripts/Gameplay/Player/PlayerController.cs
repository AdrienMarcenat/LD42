using System.Collections.Generic;
using UnityEngine;

public enum EFacingDirection
{
    // Keep that order !!! (it has to be circular)
    Right,
    Up,
    Left,
    Down
}

public class PlayerController : MonoBehaviour
{
    private EFacingDirection m_FacingDirection;
    private TileObject m_Bin;

    void Awake ()
    {
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
            MoveCommand command = new MoveCommand (gameObject, xDir, yDir);
            command.Execute ();
            CommandStackProxy.Get ().PushCommand (command);
        }
    }

    private void AddTurnCommand (bool isTurningRight)
    {
        int direction = isTurningRight ? -1 : 1;
        if (CanTurn (Modulo ((int)m_FacingDirection + direction, 4)))
        {
            TurnCommand command = new TurnCommand (gameObject, isTurningRight);
            command.Execute ();
            CommandStackProxy.Get ().PushCommand (command);
        }
    }

    private void AddToggleGrabCommand ()
    {
        if (CanToggleGrab ())
        {
            ToggleGrabCommand command = new ToggleGrabCommand (gameObject);
            command.Execute ();
            CommandStackProxy.Get().PushCommand (command);
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
                    AddTurnCommand (true);
                    break;
                case "TurnLeft":
                    AddTurnCommand (false);
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
            return nextTruckTile != null && nextTruckTile.IsEmpty ();
        }
        else
        {
            TileCoordinates facingCoordinate = GetFacingTileCoordinates ();
            Tile nextBarrelTile = TileManagerProxy.Get ().GetTile (new TileCoordinates(facingCoordinate.x + xDir, facingCoordinate.y + yDir));
            return nextBarrelTile != null && nextTruckTile != null && nextTruckTile.IsEmpty () && nextBarrelTile.IsEmpty ();
        }
    }

    public void ToggleGrab ()
    {
        Tile facingTile = GetFacingTile ();
        if (facingTile == null)
        {
            return;
        }
        if (m_Bin != null)
        {
            if (facingTile.IsEmpty ())
            {
                //new BinEvent ((Bin)m_Bin).Push ();
                facingTile.SetTileObject (m_Bin);
                m_Bin.transform.SetParent (null, true);
                m_Bin = null;
            }
        }
        else
        {
            TileObject bin = facingTile.GetTileObject ();
            if (bin != null && bin.GetObjectType () == ETileObjectType.Bin)
            {
                facingTile.SetTileObject (null);
                bin.transform.SetParent (transform, true);
                m_Bin = bin;
            }
        }
    }

    private bool CanToggleGrab ()
    {
        Tile facingTile = GetFacingTile ();
        return facingTile != null && ((m_Bin != null && facingTile.IsEmpty ()) || (m_Bin == null && facingTile.HasBin ()));
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

    public void Turn (bool isTurningRight)
    {
        if (isTurningRight)
        {
            transform.Rotate (Vector3.forward, -90);
        }
        else
        {
            transform.Rotate (Vector3.forward, 90);
        }
        int direction = isTurningRight ? -1 : 1;
        m_FacingDirection = (EFacingDirection)Modulo ((int)m_FacingDirection + direction, 4);
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
            Tile nextTile = TileManagerProxy.Get ().GetTile (currentTileCoordinates + newFacingOffset);
            Tile passingTile = TileManagerProxy.Get ().GetTile (currentTileCoordinates + passingTileOffset);
            return passingTile != null && passingTile.IsEmpty ()
                   && nextTile != null && nextTile.IsEmpty ();
        }
    }

    public void SetFacingDirection (EFacingDirection newFacingOrientation)
    {
        m_FacingDirection = newFacingOrientation;
        transform.rotation = Quaternion.Euler (0, 0, ms_FacingAngles[newFacingOrientation]);
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

    private static Dictionary<EFacingDirection, float> ms_FacingAngles = new Dictionary<EFacingDirection, float> ()
    {
        { EFacingDirection.Right, -90 },
        { EFacingDirection.Left, 90 },
        { EFacingDirection.Up, 0 },
        { EFacingDirection.Down, 180 },
    };
}

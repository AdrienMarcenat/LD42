using System.Collections;
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
    [SerializeField] private float m_MoveSpeed = 150f;
    [SerializeField] private float m_TurnSpeed = 400f;

    private EFacingDirection m_FacingDirection;
    private TileObject m_Bin;
    private bool m_IsMoving = false;
    private bool m_IsTurning = false;
    private float m_TurnAmount = 0f;
    private Vector3 m_TargetPos;
    private Vector3 m_OldAngle;
    private Vector3 m_TargetAngle;
    private Animator m_Animator;

    void Awake ()
    {
        m_TargetPos = transform.position;
        m_OldAngle = transform.rotation.eulerAngles;
        m_Animator = GetComponent<Animator> ();
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
        int direction = isTurningRight ? 1 : -1;
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
            CommandStackProxy.Get ().PushCommand (command);
        }
    }

    private void AddBinSpawnCommand ()
    {
        if (LevelManagerProxy.Get ().GetMode () == ELevelMode.MaxBin)
        {
            BinSpawnCommand command = new BinSpawnCommand (gameObject);
            command.Execute ();
            CommandStackProxy.Get ().PushCommand (command);
        }
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        if(UpdaterProxy.Get().IsPaused())
        {
            return;
        }

        string input = inputEvent.GetInput ();
        EInputState state = inputEvent.GetInputState ();
        if (state == EInputState.Down)
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
                case "BinSpawnRequest":
                    AddBinSpawnCommand ();
                    break;
                default:
                    break;
            }
        }
    }

    public void Move (int xDir, int yDir)
    {
        m_TargetPos = new Vector3 (transform.position.x + xDir.ToWorldUnit (), transform.position.y + yDir.ToWorldUnit (), transform.position.z);
        StartCoroutine (MoveRoutine ());
    }

    public void MoveInstant (int xDir, int yDir)
    {
        StopMovement ();
        m_TargetPos = new Vector3 (transform.position.x + xDir.ToWorldUnit (), transform.position.y + yDir.ToWorldUnit (), transform.position.z);
        transform.position = m_TargetPos;
    }

    IEnumerator MoveRoutine ()
    {
        SetIsMoving (true);
        while (transform.position != m_TargetPos)
        {
            transform.position = Vector3.MoveTowards (transform.position, m_TargetPos, Time.deltaTime * m_MoveSpeed);
            yield return null;
        }
        SetIsMoving (false);
    }

    private void SetIsMoving (bool isMoving)
    {
        m_IsMoving = isMoving;
        m_Animator.SetBool ("IsMoving", m_IsMoving);
    }

    private bool CanMoveTo (int xDir, int yDir)
    {
        if (m_IsMoving || m_IsTurning)
        {
            return false;
        }

        Tile nextTruckTile = TileManagerProxy.Get ().GetTile (((int)transform.position.x).ToTileUnit () + xDir, ((int)transform.position.y).ToTileUnit () + yDir);
        if (m_Bin == null)
        {
            return nextTruckTile != null && nextTruckTile.IsEmpty ();
        }
        else
        {
            TileCoordinates facingCoordinate = GetFacingTileCoordinates ();
            Tile nextBarrelTile = TileManagerProxy.Get ().GetTile (new TileCoordinates (facingCoordinate.x + xDir, facingCoordinate.y + yDir));
            return nextBarrelTile != null && nextTruckTile != null && nextTruckTile.IsEmpty () && nextBarrelTile.IsEmpty ();
        }
    }

    private void SetBinCoordinates()
    {
        if(m_Bin != null)
        {
            m_Bin.SetCoordinates (GetFacingTileCoordinates ());
        }
    }

    public void ToggleGrab (bool undo = false)
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
                SetBinCoordinates ();
                facingTile.SetTileObject (m_Bin);
                m_Bin.transform.SetParent (null, true);
                GoalManagerProxy.Get ().OnBinPlaced ((Bin)m_Bin);
                m_Bin = null;
                m_Animator.SetBool ("IsGrabbing", false);
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
                GoalManagerProxy.Get ().OnBinRemoved ((Bin)m_Bin, undo);
                m_Animator.SetBool ("IsGrabbing", true);
            }
        }
    }

    private bool CanToggleGrab ()
    {
        if (m_IsMoving || m_IsTurning)
        {
            return false;
        }
        Tile facingTile = GetFacingTile ();
        return facingTile != null && ((m_Bin != null && facingTile.IsEmpty ()) || (m_Bin == null && facingTile.HasBin ()));
    }

    private Tile GetFacingTile ()
    {
        return TileManagerProxy.Get ().GetTile (GetFacingTileCoordinates ());
    }

    private TileCoordinates GetFacingTileCoordinates ()
    {
        TileCoordinates currentTileCoordinates = new TileCoordinates (((int)transform.position.x).ToTileUnit (), ((int)transform.position.y).ToTileUnit ());
        TileCoordinates facingTileOffset = ms_NeighboorTiles[m_FacingDirection];

        return currentTileCoordinates + facingTileOffset;
    }

    public void Turn (bool isTurningRight)
    {
        TurnInternal (isTurningRight);
        StartCoroutine (TurnRoutine (isTurningRight ? 1 : -1));
    }

    public void TurnInstant (bool isTurningRight)
    {
        StopMovement ();
        TurnInternal (isTurningRight);
        transform.rotation = Quaternion.Euler (m_TargetAngle);
    }

    private void TurnInternal (bool isTurningRight)
    {
        m_OldAngle = transform.rotation.eulerAngles;
        int direction = isTurningRight ? 1 : -1;
        m_FacingDirection = (EFacingDirection)Modulo ((int)m_FacingDirection + direction, 4);
        if (isTurningRight)
        {
            m_TargetAngle = transform.rotation.eulerAngles + new Vector3 (0, 0, 90);
        }
        else
        {
            m_TargetAngle = transform.rotation.eulerAngles + new Vector3 (0, 0, -90);
        }
    }

    private void SetIsTurning (bool isMoving)
    {
        m_IsTurning = isMoving;
    }

    IEnumerator TurnRoutine (int direction)
    {
        m_TurnAmount = 0;
        SetIsTurning (true);
        while (m_TurnAmount != 90)
        {
            m_TurnAmount = Mathf.Clamp (m_TurnAmount + Time.deltaTime * m_TurnSpeed, 0f, 90f);
            transform.rotation = Quaternion.Euler (m_OldAngle + new Vector3 (0, 0, direction * m_TurnAmount));
            yield return null;
        }
        SetIsTurning (false);
    }

    private void StopMovement ()
    {
        if (m_IsTurning || m_IsMoving)
        {
            StopAllCoroutines ();
            SetIsMoving (false);
            SetIsTurning (false);
            transform.position = m_TargetPos;
            transform.rotation = Quaternion.Euler (m_TargetAngle);
        }
    }

    private bool CanTurn (int newFacingDirection)
    {
        if (m_IsMoving || m_IsTurning)
        {
            return false;
        }

        if (m_Bin == null)
        {
            return true;
        }

        else
        {
            TileCoordinates currentTileCoordinates = new TileCoordinates (((int)transform.position.x).ToTileUnit (), ((int)transform.position.y).ToTileUnit ());
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

    public static Dictionary<EFacingDirection, float> ms_FacingAngles = new Dictionary<EFacingDirection, float> ()
    {
        { EFacingDirection.Right, 90 },
        { EFacingDirection.Left, -90 },
        { EFacingDirection.Up, 180 },
        { EFacingDirection.Down, 0 },
    };
}

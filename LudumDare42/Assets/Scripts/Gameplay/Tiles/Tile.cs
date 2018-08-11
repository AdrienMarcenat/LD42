
using UnityEngine;

public enum ETileType
{
    None,

    Normal,
    Wall,
    Acid,
    Goal,
    Start,
}

public struct TileCoordinates
{
    public TileCoordinates(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

    public static TileCoordinates operator + (TileCoordinates t1, TileCoordinates t2)
    {
        TileCoordinates res = new TileCoordinates ();
        res.x = t1.x + t2.x;
        res.y = t1.y + t2.y;
        return res;
    }

    public static implicit operator TileCoordinates (Vector3 vector)
    {
        TileCoordinates res = new TileCoordinates ();
        res.x = (int)vector.x;
        res.y = (int)vector.y;
        return res;
    }

    public int x;
    public int y;
}

public class Tile : MonoBehaviour
{
    [SerializeField]  private ETileType m_Type;
    [SerializeField] private TileObject m_Object;
    private TileCoordinates m_Coordinates;
    private bool m_HasBarrel;

    public bool HasBin ()
    {
        return m_Object != null && m_Object.GetObjectType() == ETileObjectType.Bin;
    }

    public TileObject GetTileObject ()
    {
        return m_Object;
    }

    public bool IsEmpty ()
    {
        return m_Object == null;
    }

    public TileCoordinates GetCoordinates ()
    {
        return m_Coordinates;
    }

    public void SetTileObject(TileObject tileObject)
    {
        m_Object = tileObject;
    }
}

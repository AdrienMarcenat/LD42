
using UnityEngine;

public enum ETileType
{
    None,

    Normal,

    Count,
}

public class Tile
{
    private ETileType m_Type;
    private GameObject m_Barrel;
    private bool m_HasBarrel;
    private Vector2 m_Coordinates;

    public bool HasBarrel()
    {
        return m_Barrel != null;
    }

    public GameObject GetBarrel ()
    {
        return m_Barrel;
    }

    public bool IsEmpty()
    {
        return m_Barrel == null;
    }

    public Vector2 GetCoordinates ()
    {
        return m_Coordinates;
    }
}

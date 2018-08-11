using UnityEngine;

public enum ETileObjectType
{
    Bin,
}

public class TileObject : MonoBehaviour
{
    [SerializeField] private ETileObjectType m_Type;

    public TileObject (ETileObjectType type)
    {
        m_Type = type;
    }

    public ETileObjectType GetObjectType ()
    {
        return m_Type;
    }
}

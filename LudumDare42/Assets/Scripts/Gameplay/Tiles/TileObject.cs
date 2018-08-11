using UnityEngine;

public enum ETileObjectType
{
    Bin,
}

public class TileObject : MonoBehaviour
{
    [SerializeField] private ETileObjectType m_Type;
    [SerializeField] private int m_Number;

    public TileObject (ETileObjectType type)
    {
        m_Type = type;
    }

    public ETileObjectType GetObjectType ()
    {
        return m_Type;
    }

    public void SetType (ETileObjectType type)
    {
        m_Type = type;
    }

    public int GetNumber ()
    {
        return m_Number;
    }

    public void SetNumber (int number)
    {
        m_Number = number;
    }
}

using UnityEngine;

public class CameraLevelSelection : MonoBehaviour
{
    private Transform m_Target;

    void Start ()
    {
        m_Target = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    void Update ()
    {
        transform.position = new Vector3 (transform.position.x, m_Target.position.y, transform.position.z);
    }
}

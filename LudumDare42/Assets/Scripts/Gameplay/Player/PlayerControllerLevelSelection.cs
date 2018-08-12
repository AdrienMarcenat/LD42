using System.Collections;
using UnityEngine;

public class PlayerControllerLevelSelection : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed = 50f;

    private bool m_IsMoving = false;
    private Vector3 m_TargetPos;
    private Animator m_Animator;
    private Transform[] m_LevelPositions;
    private int m_CurrentLevel;

    void Awake ()
    {
        m_LevelPositions = GameObject.Find("LevelPositions").GetComponentsInChildren<Transform> ().SubArray (1);
        m_CurrentLevel = 0;
        m_TargetPos = transform.position;
        m_Animator = GetComponent<Animator> ();
        this.RegisterAsListener ("Player", typeof (PlayerInputGameEvent));
    }

    private void OnDestroy ()
    {
        this.UnregisterAsListener ("Player");
    }

    public void OnGameEvent (PlayerInputGameEvent inputEvent)
    {
        string input = inputEvent.GetInput ();
        EInputState state = inputEvent.GetInputState ();
        if (inputEvent.GetInputState () == EInputState.Down)
        {
            switch (input)
            {
                case "Up":
                    Move (true);
                    break;
                case "Down":
                    Move (false);
                    break;
                default:
                    break;
            }
        }
    }

    public void Move (bool isGoingUp)
    {
        int nextLevel = m_CurrentLevel + (isGoingUp ? 1 : -1);
        if (!m_IsMoving && nextLevel >= 0 && nextLevel < m_LevelPositions.Length)
        {
            m_CurrentLevel = nextLevel;
            LevelManagerProxy.Get ().SetLevelIndex (m_CurrentLevel);
            m_TargetPos = m_LevelPositions[m_CurrentLevel].position;
            StartCoroutine (MoveRoutine ());
        }
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
}

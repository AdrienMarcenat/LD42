using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    [SerializeField] private Text m_BestNumber;
    [SerializeField] private Text m_MoveNumber;
    [SerializeField] private Text m_LevelName;

    private void Start ()
    {
        int best = LevelManagerProxy.Get ().GetCurrentLevelScore ();
        m_BestNumber.text = best > 0 ? best.ToString() : "";
        m_LevelName.text = LevelManagerProxy.Get ().GetCurrentLevelName ();
    }

    private void Update ()
    {
        m_MoveNumber.text = CommandStackProxy.Get ().GetNumberOfCommand ().ToString();
    }

}

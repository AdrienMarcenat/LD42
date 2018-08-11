using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    [SerializeField] private Text m_CommandNumber;

    private void Update ()
    {
        m_CommandNumber.text = "Command Number: " + CommandStackProxy.Get ().GetNumberOfCommand ();
    }

}

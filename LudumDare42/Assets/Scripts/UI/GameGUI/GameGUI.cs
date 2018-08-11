using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    [SerializeField] private Text m_CommandNumber;
    [SerializeField] private Text m_BinNumber;

    private void Update ()
    {
        m_CommandNumber.text = "Command Number: " + CommandStackProxy.Get ().GetNumberOfCommand ();
        m_BinNumber.text = "Bin Number: " + BinSpawner.ms_BinNumber;
    }

}

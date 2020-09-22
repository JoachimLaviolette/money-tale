using UnityEngine;
using UnityEngine.UI;

public class ActionPopup : MonoBehaviour
{
    [SerializeField]
    private Transform m_backgroundPanel;
    [SerializeField]
    private Text m_message;

    public void Setup(string sampleText, string objectName)
    {
        m_message.text = string.Format(sampleText, objectName);
    }
}

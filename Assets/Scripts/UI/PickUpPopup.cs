using UnityEngine;
using UnityEngine.UI;

public class PickUpPopup : MonoBehaviour
{
    private string m_sampleText = "Pick up {0} [E]";
    [SerializeField]
    private Transform m_backgroundPanel;
    [SerializeField]
    private Text m_text;

    public void Setup(string objectName)
    {
        m_text.text = string.Format(m_sampleText, objectName);
    }
}

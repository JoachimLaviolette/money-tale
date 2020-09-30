using UnityEngine;
using UnityEngine.UI;

public class ActionPopup : MonoBehaviour
{
    [SerializeField]
    private Transform m_backgroundPanel;
    private Image m_backgroundPanelImage;
    [SerializeField]
    private Text m_message;

    private void Awake()
    {
        m_backgroundPanelImage = m_backgroundPanel.GetComponent<Image>();
    }

    public void Setup(string sampleText, string textFormatArgs)
    {
        m_message.text = string.Format(sampleText, textFormatArgs);
    }

    public void Setup(Color textColor, Color backgroundColor)
    {
        m_message.color = textColor;
        m_backgroundPanelImage.color = backgroundColor;
    }
}

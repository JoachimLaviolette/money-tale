using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfoMessage : MonoBehaviour
{
    public class Option
    {
        public string m_text;
        public KeyCode m_cmd;
        public UnityAction m_callback;
    }

    [SerializeField]
    private Text m_message;
    [SerializeField]
    private Text m_option1Text;
    [SerializeField]
    private Text m_option2Text;
    private Option m_option1;
    private Option m_option2;

    private void Update()
    {
        if (!Input.GetKeyDown(m_option1.m_cmd) && !Input.GetKeyDown(m_option2.m_cmd)) return;

        if (Input.GetKeyDown(m_option1.m_cmd)) m_option1.m_callback?.Invoke();
        else m_option2.m_callback?.Invoke();

        Destroy(gameObject);
    }

    /**
     * Setup the information message
     */
    public void Setup(string message, Option option1, Option option2)
    {
        m_message.text = message;
        m_option1Text.text = option1.m_text;
        m_option2Text.text = option2.m_text;
        m_option1 = option1;
        m_option2 = option2;
    } 
}

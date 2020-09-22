using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerController m_playerController;
    private int m_index;
    [SerializeField]
    private Image m_icon;
    [SerializeField]
    private Text m_name;
    private bool m_isSelected;
    [SerializeField]
    private Image m_stroke;
    [SerializeField]
    private Image m_background;
    [SerializeField]
    private Sprite m_strokeRegular;
    [SerializeField]
    private Sprite m_strokeSelected;
    private bool m_isEmpty;

    private void Start()
    {
        m_isEmpty = true;
        Disable();
    }

    public void Setup(bool isEmpty, PlayerController playerController, int index, Sprite icon, string name, bool isSelected)
    {
        m_isEmpty = isEmpty;

        if (m_isEmpty)
        {
            m_playerController = null;
            m_index = -1;
            m_icon.sprite = null;
            m_name.text = "";
            m_isSelected = false;
            Disable();
        } 
        else
        {
            m_playerController = playerController;
            m_index = index;
            m_icon.sprite = icon;
            m_name.text = name.ToUpper();
            m_isSelected = isSelected;
            Enable();
        }

        SetSelected(m_isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        m_isSelected = isSelected;
        m_stroke.sprite = m_isSelected ? m_strokeSelected : m_strokeRegular;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_isEmpty) return;

        if (!m_isSelected) m_playerController.Equip(m_index);
        else m_playerController.UnEquip(m_index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_isEmpty) return;
        if (m_isSelected) return; 

        m_stroke.sprite = m_strokeSelected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_isEmpty) return;
        if (m_isSelected) return;

        m_stroke.sprite = m_strokeRegular;
    }

    private void Disable()
    {
        Color strokeColor = m_stroke.color;
        strokeColor.a = 0.2f;
        m_stroke.color = strokeColor;

        Color backgroundColor = m_background.color;
        backgroundColor.a = 0.2f;
        m_background.color = backgroundColor;

        Color iconColor = m_icon.color;
        iconColor.a = 0f;
        m_icon.color = iconColor;
    }

    private void Enable()
    {
        Color strokeColor = m_stroke.color;
        strokeColor.a = 1f;
        m_stroke.color = strokeColor;

        Color backgroundColor = m_background.color;
        backgroundColor.a = 1f;
        m_background.color = backgroundColor;

        m_icon.color = Color.white;
    }
}

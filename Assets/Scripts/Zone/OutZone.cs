using UnityEngine;

public class OutZone : MonoBehaviour
{
    [SerializeField]
    private bool m_isEnabled;
    [SerializeField]
    private Material m_enabledMaterial;
    [SerializeField]
    private Material m_disabledMaterial;
    private MeshRenderer m_meshRenderer;
    [SerializeField]
    private LayerMask m_playerLayerMask;

    private void Start()
    {
        m_isEnabled = false;
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    public bool isEnabled()
    {
        return m_isEnabled;
    }

    public void SetEnabled(bool isEnabled)
    {
        m_isEnabled = isEnabled;
        m_meshRenderer.sharedMaterial = m_enabledMaterial;
    }
}

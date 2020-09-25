using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private float m_hp;

    private void Start()
    {
        m_hp = 100f;
    }

    /**
     * Damage the player
     */
    public void Damage(float damages, Transform damagerTransform)
    {
        m_hp -= damages;

        if (m_hp < 0f) m_hp = 0f;

        // Debug.Log(string.Format("Player has lost {0} HP, falling back to {1} HP.", damages, m_hp));
    }
}

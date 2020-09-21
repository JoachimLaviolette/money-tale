using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private float m_hp;

    private void Start()
    {
        m_hp = 100f;
    }

    /**
     * Damage the enemy
     */
    public void Damage(float damages)
    {
        m_hp -= damages;

        if (m_hp < 0f) m_hp = 0f;

        Debug.Log(string.Format("Enemy {0} has lost {1} HP, falling back to {2} HP.", gameObject.name, damages, m_hp));
    }
}

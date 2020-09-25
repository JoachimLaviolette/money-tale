using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private float m_hp;
    public enum State
    {
        Damaged,
        Dead,
    }
    public class OnEnemyDamagedArgs : EventArgs
    {
        public State m_enemyState;
        public Transform m_damagerTransform;
    }
    public EventHandler<OnEnemyDamagedArgs> m_onEnemyDamaged;

    private void Start()
    {
        m_hp = 100f;
    }

    /**
     * Damage the enemy
     */
    public void Damage(float damages, Transform damagerTransform)
    {
        m_hp -= damages;

        if (m_hp < 0f) m_hp = 0f;

        m_onEnemyDamaged?.Invoke(this, new OnEnemyDamagedArgs { m_enemyState = m_hp == 0f ? State.Dead : State.Damaged, m_damagerTransform = damagerTransform });
        
        // Debug.Log(string.Format("Enemy {0} has lost {1} HP, falling back to {2} HP.", gameObject.name, damages, m_hp));
    }
}

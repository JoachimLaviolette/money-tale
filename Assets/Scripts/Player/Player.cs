using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private float m_hp;
    public enum State
    {
        Damaged,
        Dead,
    }
    public class OnPlayerDamagedArgs : EventArgs
    {
        public State m_playerState;
        public Transform m_damagerTransform;
        public DamageType m_damageType;
    }
    public EventHandler<OnPlayerDamagedArgs> m_onPlayerDamaged;

    private void Start()
    {
        m_hp = 100f;
    }

    /**
     * Damage the player
     */
    public void Damage(float damages, Transform damagerTransform, DamageType damageType)
    {
        m_hp -= damages;

        if (m_hp < 0f) m_hp = 0f;

        m_onPlayerDamaged?.Invoke(this, new OnPlayerDamagedArgs { 
            m_playerState = m_hp == 0f ? State.Dead : State.Damaged, 
            m_damagerTransform = damagerTransform,
            m_damageType = damageType,
        });

        // Debug.Log(string.Format("Player {0} has lost {1} HP, falling back to {2} HP.", gameObject.name, damages, m_hp));
    }
}

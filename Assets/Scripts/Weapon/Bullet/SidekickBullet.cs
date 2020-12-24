using UnityEngine;

public class SidekickBullet : Bullet
{
    [SerializeField]
    private Transform m_anchor;
    private readonly float m_length = 100f;

    /**
     * Setup the bullet
     */
    override public void Setup(bool isStatic, Vector3 direction, float speed, float damages, DamageType damageType, Transform shooterTransform, LayerMask damageableLayerMask, bool destroyAfterDamage = true)
    {
        base.Setup(isStatic, direction, speed, damages, damageType, shooterTransform, damageableLayerMask, false);
        m_anchor.localScale = new Vector3(
            m_length,
            m_anchor.localScale.y,
            m_anchor.localScale.z);
    }
}

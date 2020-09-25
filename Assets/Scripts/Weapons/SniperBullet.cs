using UnityEngine;

public class SniperBullet : Bullet
{
    [SerializeField]
    private Transform m_anchor;
    [SerializeField]
    private LayerMask m_outerLimitLayerMask;

    /**
     * Setup the bullet
     */
    override public void Setup(bool isStatic, Vector3 direction, float speed, float damages, Transform shooterTransform, LayerMask damageableLayerMask, bool destroyAfterDamage = true)
    {
        base.Setup(isStatic, direction, speed, damages, shooterTransform, damageableLayerMask, destroyAfterDamage);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out RaycastHit hit, Mathf.Infinity, m_outerLimitLayerMask))
        {
            m_anchor.localScale = new Vector3(
            hit.distance,
            m_anchor.localScale.y,
            m_anchor.localScale.z);
        }       
    }
}

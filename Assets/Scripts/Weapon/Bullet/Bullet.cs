using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected bool m_isStatic;
    protected Vector3 m_direction;
    protected float m_moveSpeed;
    protected float m_damages;
    protected DamageType m_damageType;
    protected Transform m_shooterTransform;
    protected LayerMask m_damageableLayerMask;
    protected bool m_destroyAfterDamage;
    [SerializeField]
    protected LayerMask[] m_onDestroyLayerMasks;

    protected void FixedUpdate()
    {
        if (!m_isStatic) transform.position += m_direction * m_moveSpeed * Time.deltaTime;
    }

    /**
     * Setup the bullet
     */
    virtual public void Setup(bool isStatic, Vector3 direction, float speed, float damages, DamageType damageType, Transform shooterTransform, LayerMask damageableLayerMask, bool destroyAfterDamage = true)
    {
        m_isStatic = isStatic;
        m_direction = new Vector3(direction.x, 0f, direction.z).normalized;
        m_moveSpeed = speed;
        m_damages = damages;
        m_damageType = damageType;
        m_shooterTransform = shooterTransform;
        m_damageableLayerMask = damageableLayerMask;
        m_destroyAfterDamage = destroyAfterDamage;
    }

    /**
     * Handle the collision
     */
    protected void OnTriggerEnter(Collider collider)
    {
        IDamageable collidedObject = collider.GetComponent<IDamageable>();

        if (collider.gameObject.layer == Mathf.Log(m_damageableLayerMask, 2))
        {
            if (collidedObject != null)
            {
                collidedObject.Damage(m_damages, m_shooterTransform, m_damageType);
                if (m_destroyAfterDamage) Destroy(gameObject);
            }
        }
        else
        {
            foreach (LayerMask layer in m_onDestroyLayerMasks)
                if (collider.gameObject.layer == Mathf.Log(layer, 2))
                    Destroy(gameObject);    
        }
    }
}

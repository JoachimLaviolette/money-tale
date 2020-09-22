using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected bool m_isStatic;
    protected Vector3 m_direction;
    protected float m_moveSpeed;
    protected float m_damages;
    protected LayerMask m_damageableLayer;
    protected bool m_destroyAfterDamage;
    [SerializeField]
    protected LayerMask[] m_onDestroyLayers;

    protected void FixedUpdate()
    {
        if (!m_isStatic) transform.position += m_direction * m_moveSpeed * Time.deltaTime;
    }

    /**
     * Setup the bullet
     */
    virtual public void Setup(bool isStatic, Vector3 direction, float speed, float damages, LayerMask damageableLayer, bool destroyAfterDamage = true)
    {
        m_isStatic = isStatic;
        m_direction = new Vector3(direction.x, 0f, direction.z).normalized;
        m_moveSpeed = speed;
        m_damages = damages;
        m_damageableLayer = damageableLayer;
        m_destroyAfterDamage = destroyAfterDamage;
    }

    /**
     * Handle the collision
     */
    protected void OnTriggerEnter(Collider collider)
    {
        IDamageable collidedObject = collider.GetComponent<IDamageable>();

        if (Mathf.Pow(2, collider.gameObject.layer) == m_damageableLayer.value)
        {
            if (collidedObject != null)
            {
                collidedObject.Damage(m_damages);
                if (m_destroyAfterDamage) Destroy(gameObject);
            }
        }
        else
        {
            foreach (LayerMask layer in m_onDestroyLayers)
                if (Mathf.Pow(2, collider.gameObject.layer) == layer.value)
                    Destroy(gameObject);    
        }
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 m_direction;
    private float m_moveSpeed;
    [SerializeField]
    private float m_damages;
    private LayerMask m_damageableLayer;
    private bool m_destroyAfterDamage;
    [SerializeField] 
    private LayerMask[] m_environmentLayers;

    public void FixedUpdate()
    {
        transform.position += m_direction * m_moveSpeed * Time.deltaTime;
    }

    /**
     * Setup the bullet
     */
    public void Setup(Vector3 direction, float speed, LayerMask damageableLayer, bool destroyAfterDamage = true)
    {
        m_direction = new Vector3(direction.x, 0f, direction.z).normalized;
        m_moveSpeed = speed;
        m_damageableLayer = damageableLayer;
        m_destroyAfterDamage = destroyAfterDamage;
    }

    /**
     * Handle the collision
     */
    private void OnTriggerEnter(Collider collider)
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
            /*foreach (LayerMask layer in m_environmentLayers)
                if (Mathf.Pow(2, collider.gameObject.layer) == layer.value) Destroy(gameObject);*/
        }

    }

    /**
     * Destroy the bullet when out of the viewport
     */
    /*private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }*/
}

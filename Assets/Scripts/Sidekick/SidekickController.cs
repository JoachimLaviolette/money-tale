using UnityEngine;

public class SidekickController : MonoBehaviour, IShooter
{
    private Rifle m_weapon;
    private Transform m_currentTarget;
    private bool m_isShooting;
    [SerializeField]
    private PlayerController m_playerController;
    private TargetScanner m_targetScanner;
    [SerializeField]
    private Transform m_bulletFirePosition;
    [SerializeField]
    private Transform m_bulletPosition;
    [SerializeField]
    private LayerMask m_enemyLayerMask;
    [SerializeField]
    private LayerMask m_damageableLayerMask;
    [SerializeField]
    [Range(1.5f, 10f)]
    private float m_shootMaxDetectionDistance;

    private void Start()
    {
        Rotate(m_playerController.transform);
        m_weapon = GetComponentInChildren<Rifle>();
        m_targetScanner = GetComponentInChildren<TargetScanner>();
        m_currentTarget = null;
        m_isShooting = false;
        m_targetScanner.m_onTargetDetected += OnEnemyDetectedCallback;
    }

    private void Update()
    {
        Rotate();
        Shoot();
    }

    /**
     * Rotate the enemy towards the given target if any, otherwise towards the current target if any
     */
    private void Rotate(Transform target = null)
    {
        Transform newTarget = target;

        if (newTarget == null)
            newTarget = m_currentTarget;
        if (newTarget == null) return;

        Vector3 newRotation = newTarget.position;
        newRotation.y = transform.position.y;
        transform.LookAt(newRotation);
    }

    /**
     * Shoot
     */
    public void Shoot()
    {
        if (m_isShooting) return;
        if (!m_weapon.CanFire()) return;
        if (m_currentTarget == null) return;
        if (1 << m_currentTarget.gameObject.layer != m_enemyLayerMask) return;

        Vector3 direction = m_currentTarget.position - transform.position;

        Debug.DrawRay(transform.position, direction.normalized * m_shootMaxDetectionDistance);
        if (!Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, m_shootMaxDetectionDistance, m_enemyLayerMask)) return;

        m_isShooting = true;
        m_weapon.Fire(
                transform,
                m_bulletFirePosition.position,
                Quaternion.Euler(90f, 0f, -transform.rotation.eulerAngles.y),
                m_bulletPosition.position,
                Quaternion.Euler(90f, 0f, 90f - transform.rotation.eulerAngles.y),
                direction,
                m_damageableLayerMask,
                () => m_isShooting = false
            );
    }

    /**
     * Called when the player is detected by the enemy scan
     */
    private void OnEnemyDetectedCallback(object sender, TargetScanner.OnTargetDetectedArgs args)
    {
        m_currentTarget = args.m_targetTransform;
    }

    /**
     * Draw Gizmos in scene view
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * 0.7f);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.back) * 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.left) * 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.right) * 0.3f);

        // Draw gizmo line to show the shooting distance radius
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * m_shootMaxDetectionDistance);
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_playerTransform;
    [SerializeField]
    [Range(0f, 30f)]
    private float m_moveSpeed = 2f;

    private void LateUpdate()
    {
        Vector3 targetPosition = m_playerTransform.position;

        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(targetPosition, transform.position);

        if (distance > 0f)
        {
            Vector3 newPosition = transform.position + moveDir * distance * m_moveSpeed * Time.deltaTime;
            float distanceAfterMoving = Vector3.Distance(newPosition, targetPosition);

            // Overshoot
            if (distanceAfterMoving > distance) newPosition = targetPosition;

            // Conserve the height
            newPosition.y = transform.position.y;

            transform.position = newPosition;
        }
    }
}

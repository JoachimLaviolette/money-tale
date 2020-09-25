using System;
using UnityEngine;

public class EnemyScan : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_playerLayerMask; 
    public EventHandler<OnPlayerDetectedArgs> m_onPlayerDetected;
    public class OnPlayerDetectedArgs : EventArgs
    {
        public Transform m_playerTransform;
    }

    /**
     * Check the enemy's surroundings
     */
    private void OnTriggerEnter(Collider collider)
    {
        if (Mathf.Pow(2, collider.gameObject.layer) == m_playerLayerMask)
        {
            // Debug.Log("Player in sight!");
            m_onPlayerDetected?.Invoke(this, new OnPlayerDetectedArgs { m_playerTransform = collider.transform });
        }
    }

    /**
     * Check the enemy's surroundings
     */
    private void OnTriggerExit(Collider collider)
    {
        if (Mathf.Pow(2, collider.gameObject.layer) == m_playerLayerMask)
        {
            // Debug.Log("Player out of sight...");
            m_onPlayerDetected?.Invoke(this, new OnPlayerDetectedArgs { m_playerTransform = null });
        }
    }
}

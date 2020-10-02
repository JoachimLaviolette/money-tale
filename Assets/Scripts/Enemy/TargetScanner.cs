using System;
using UnityEngine;

public class TargetScanner : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_targetLayerMask; 
    public EventHandler<OnTargetDetectedArgs> m_onTargetDetected;
    public class OnTargetDetectedArgs : EventArgs
    {
        public Transform m_targetTransform;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == Mathf.Log(m_targetLayerMask, 2))
            m_onTargetDetected?.Invoke(this, new OnTargetDetectedArgs { m_targetTransform = collider.transform });
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == Mathf.Log(m_targetLayerMask, 2))
            m_onTargetDetected?.Invoke(this, new OnTargetDetectedArgs { m_targetTransform = null });
    }
}

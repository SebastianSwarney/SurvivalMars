using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DetectionEvent: UnityEngine.Events.UnityEvent { }

public class EnemyDetection : MonoBehaviour
{
    public DetectionEvent m_playerInRadiusEvent;
    public DetectionEvent m_playerExitRadiusEvent;
    public DetectionEvent m_playerLostEvent;
    private bool m_playerLost;

    public float m_chaseDetectionRadius;

    [Header("Gizmos")]
    public bool m_debugGizmos;
    public Color m_chaseDetectionColor;
    public LayerMask m_playerDetectionMask;
    private void OnTriggerStay(Collider other)
    {
        m_playerInRadiusEvent.Invoke();
        m_playerLost = false;
    }
    private void OnTriggerExit(Collider other)
    {
        m_playerExitRadiusEvent.Invoke();
    }

    public bool PlayerInRadius()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, m_chaseDetectionRadius, transform.forward, out hit, 0, m_playerDetectionMask))
        {
            return true;
        }
        if (!m_playerLost)
        {
            m_playerLost = true;
            m_playerLostEvent.Invoke();
        }
        
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!m_debugGizmos) return;
        Gizmos.color = m_chaseDetectionColor;
        Gizmos.DrawWireSphere(transform.position, m_chaseDetectionRadius);
    }
}

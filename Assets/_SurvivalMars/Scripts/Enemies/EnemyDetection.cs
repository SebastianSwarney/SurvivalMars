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
    public LayerMask m_playerDetectionMask;
    public LayerMask m_nonBlockingTerrain;

    [Header("Gizmos")]
    public bool m_debugGizmos;
    public Color m_chaseDetectionColor;
    

    Collider[] m_colliders;
    private void Awake()
    {
        m_colliders = GetComponents<Collider>();
    }

    public void SetCollidersState(bool p_collide)
    {
        foreach (Collider col in m_colliders)
        {
            col.enabled = p_collide;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!Physics.Linecast(transform.position, other.transform.position, ~m_nonBlockingTerrain))
        {
            m_playerInRadiusEvent.Invoke();
            m_playerLost = false;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        m_playerExitRadiusEvent.Invoke();
    }

    public bool PlayerInRadius()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_chaseDetectionRadius, m_playerDetectionMask);
        if (cols.Length > 0)
        {
            if (!Physics.Linecast(transform.position, cols[0].transform.position, ~m_nonBlockingTerrain))
            {
                return true;
            }
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

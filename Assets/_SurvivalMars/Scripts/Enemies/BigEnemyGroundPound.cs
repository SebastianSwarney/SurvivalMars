using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundPound : UnityEngine.Events.UnityEvent { }
public class BigEnemyGroundPound : MonoBehaviour
{
    public GroundPound m_groundHit;
    public float m_radius;
    public LayerMask m_fxLayer;

    [Header("Debugging")]
    public bool m_debugging;
    public Color m_gizmosColor1;

    private void OnTriggerEnter(Collider other)
    {
        m_groundHit.Invoke();
        Collider[] cols = Physics.OverlapSphere(transform.position, m_radius, m_fxLayer);
        foreach(Collider col in cols)
        {
            IAreaFx[] fx = col.GetComponents<IAreaFx>();
            foreach (IAreaFx newFx in fx)
            {
                newFx.PerformEffect(transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}

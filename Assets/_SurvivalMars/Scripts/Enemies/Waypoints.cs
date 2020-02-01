using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public Waypoints m_nextWaypoint;

    [Header("DEbugging")]
    public bool m_debugging = true;
    public Color m_nextWaypointColor;

    private void OnDrawGizmos()
    {
        if (!m_debugging || m_nextWaypoint == null) return;
        Gizmos.DrawLine(transform.position, m_nextWaypoint.transform.position);
        Gizmos.DrawWireSphere(transform.position + (m_nextWaypoint.transform.position - transform.position).normalized * (m_nextWaypoint.transform.position - transform.position).magnitude * .25f, .5f);
    }

    public Waypoints GiveNextWaypoint()
    {
        return m_nextWaypoint;
    }
}

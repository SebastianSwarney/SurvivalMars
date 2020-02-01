using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Radius : MonoBehaviour
{
    public float m_radius;

    [Header("Debugging")]
    public bool m_debug;
    public Color m_debuggingColor;
    public Vector3 GiveNewPosition()
    {
        Vector2 position = Random.insideUnitCircle * m_radius;
        return new Vector3(position.x, 0f, position.y) + new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        Gizmos.color = m_debuggingColor;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement_SmallEnemy : EnemyMovement_Base
{
    private Waypoints m_currentWaypoint;
    public int m_sectorIndex;
    private Waypoint_Manager m_waypointManager;
    public float m_stoppingDistance, m_brakingDistance;
    public float m_chaseSpeed;
    public float m_idleSpeed;

    private Rigidbody m_rb;
    private float m_currentSpeed;

    private void Start()
    {
        m_waypointManager = Waypoint_Manager.Instance;
    }
    public override void IdleMovement()
    {
        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(m_currentWaypoint.transform.position.x, 0, m_currentWaypoint.transform.position.z));
        if (distance > m_brakingDistance)
        {
            m_currentSpeed = 1 * m_idleSpeed;
        }
        else
        {
            m_currentSpeed = (distance / m_brakingDistance) * m_idleSpeed;
        }
        
    }

    public override void MoveToLastKnownPosition()
    {

    }

    public override void MoveToPlayer()
    {
    }

    public override void RunAway()
    {
    }
}

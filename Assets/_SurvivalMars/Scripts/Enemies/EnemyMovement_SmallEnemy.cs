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
    [Header("Rotation")]
    public float m_rotateSpeed;
    public float m_rotationSensitivity = 5;

    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        m_waypointManager = Waypoint_Manager.Instance;
        m_currentWaypoint = m_waypointManager.GetClosestWaypoint(m_sectorIndex, transform.position);
    }
    public override void IdleMovement()
    {
        if (NearPosition(m_idleSpeed, m_brakingDistance, m_stoppingDistance, m_currentWaypoint.transform.position))
        {
            m_currentWaypoint = m_currentWaypoint.m_nextWaypoint;
        }
    }

    /// <summary>
    /// Moves the Character accordingly, and brakes depending on how close to the target position
    /// </summary>
    /// <param name="p_stateSpeed"></param>
    /// <param name="p_stateBrakingDistance"></param>
    /// <param name="p_stateStoppingDistance"></param>
    /// <param name="p_target"></param>
    /// <returns></returns>
    private bool NearPosition(float p_stateSpeed, float p_stateBrakingDistance, float p_stateStoppingDistance, Vector3 p_target)
    {
        bool nearPos = false;
        float currentSpeed;
        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(p_target.x, 0, p_target.z));
        if (distance > p_stateBrakingDistance)
        {
            currentSpeed = 1 * p_stateSpeed;
        }
        else
        {
            if (distance < p_stateStoppingDistance)
            {

                nearPos = true;
                currentSpeed = 1 * p_stateSpeed;
            }
            else
            {
                currentSpeed = (distance / p_stateBrakingDistance) * p_stateSpeed;
            }
        }
        Vector3 dir = transform.position - p_target;
        dir = -new Vector3(dir.x, 0, dir.z).normalized;
        RotateToPoint(new Vector3(p_target.x, transform.position.y, p_target.z));
        m_rb.velocity = new Vector3(transform.forward.x, m_rb.velocity.y, transform.forward.z) * currentSpeed;
        return nearPos;
    }

    public override void MoveToLastKnownPosition()
    {
        NearPosition(m_chaseSpeed, m_brakingDistance, m_stoppingDistance, m_lastPlayerPostion);
    }

    public override void MoveToPlayer()
    {
        NearPosition(m_chaseSpeed, 0, 0, m_playerObject.transform.position);
    }

    public override void RotateToPoint(Vector3 p_point)
    {


        Vector3 dirToTarget = (new Vector3(p_point.x, 0f, p_point.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized * m_rotationSensitivity;
        float rotationAmount = Vector3.Cross(transform.forward, dirToTarget - transform.forward).magnitude;

        Debug.DrawLine(transform.position, transform.position + Vector3.Cross(transform.forward, dirToTarget - transform.forward));
        Debug.DrawLine(transform.position, p_point);
        float rotationSide = Vector3.Dot(transform.right, dirToTarget - transform.forward);
        print("Rotate Side: " + rotationSide);

        if (Vector3.Angle(transform.forward, dirToTarget) > 150)
        {
            rotationSide = 1;
            rotationAmount = 1.5f;
        }
        transform.Rotate(transform.up, ((rotationAmount > 1 ) ? m_rotateSpeed : rotationAmount * m_rotateSpeed) * Mathf.Sign(rotationSide));
    }
}

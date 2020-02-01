using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement_BigEnemy : EnemyMovement_Base
{
    public int m_sectorIndex;
    private Waypoint_Manager m_waypointManager;
    public float m_stoppingDistance, m_brakingDistance;
    public float m_chaseSpeed;
    public float m_idleSpeed;
    private Vector3 m_targetPosition;

    [Header("Rotation")]
    public float m_rotateSpeed;
    public float m_rotationSensitivity = 5;

    [Header("Floating Values")]
    public LayerMask m_floatingLayer;
    public float m_targetHeightAboveGround;
    public float m_maxRaycastDistance;
    [Tooltip("The amount of influence the height above the ground has.")]
    public float m_targetHeightWeight;
    public float m_maxBoost;
    public float m_heightStoppingDistance;
    public float m_heightBrakingDistance;

    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        m_waypointManager = Waypoint_Manager.Instance;
    }
    public override void IdleMovement()
    {
        if (NearPosition(m_idleSpeed, m_brakingDistance, m_stoppingDistance, new Vector3(m_targetPosition.x, transform.position.y, m_targetPosition.z)))
        {
            m_targetPosition = m_waypointManager.GiveNewPointInRadius(m_sectorIndex);
        }
    }

    
    public override void MoveToLastKnownPosition()
    {
        NearPosition(m_chaseSpeed, m_brakingDistance, m_stoppingDistance, new Vector3(m_lastPlayerPostion.x, transform.position.y, m_lastPlayerPostion.z));
    }

    public override void MoveToPlayer()
    {
        NearPosition(m_chaseSpeed, 0, 0, new Vector3(m_playerObject.transform.position.x, transform.position.y, m_playerObject.transform.position.z));
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
                print("CurrentSpeed");
            }
        }
        Vector3 dir = transform.position - p_target;
        dir = -new Vector3(dir.x, 0, dir.z).normalized;
        RotateToPoint(new Vector3(p_target.x, transform.position.y, p_target.z));
        if (currentSpeed > 30)
        {
            print("Current Speed : " + currentSpeed);
        }
        m_rb.velocity = new Vector3(transform.forward.x * currentSpeed, m_rb.velocity.y, transform.forward.z * currentSpeed) ;


        FloatAboveGround();
        return nearPos;
    }

    private void FloatAboveGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit, m_maxRaycastDistance, m_floatingLayer))
        {

            float distance = Vector3.Distance(transform.position, hit.point) - m_targetHeightAboveGround;
            Distance = distance;
            float newVelocity = 0;


            if (Mathf.Abs(distance) > m_brakingDistance )
            {
                newVelocity = -m_maxBoost * Mathf.Sign(distance);
                print("Max Boost");
            }
            else
            {
                if (Mathf.Abs(distance) > m_stoppingDistance)
                {
                    newVelocity = -m_maxBoost * (Mathf.Abs(distance) / m_brakingDistance) * Mathf.Sign(distance);
                    print("Adjust Boost");
                }
                print("No Boost");
                
            }


            m_rb.velocity = new Vector3(m_rb.velocity.x, newVelocity, m_rb.velocity.z);
        }

    }
    public float Distance;

    public override void RotateToPoint(Vector3 p_point)
    {


        Vector3 dirToTarget = (new Vector3(p_point.x, 0f, p_point.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized * m_rotationSensitivity;
        float rotationAmount = Vector3.Cross(transform.forward, dirToTarget - transform.forward).magnitude;

        Debug.DrawLine(transform.position, transform.position + Vector3.Cross(transform.forward, dirToTarget - transform.forward));
        Debug.DrawLine(transform.position, p_point);
        float rotationSide = Vector3.Dot(transform.right, dirToTarget - transform.forward);

        if (Vector3.Angle(transform.forward, dirToTarget) > 150)
        {
            rotationSide = 1;
            rotationAmount = 1.5f;
        }
        transform.Rotate(transform.up, ((rotationAmount > 1) ? m_rotateSpeed : rotationAmount * m_rotateSpeed) * Mathf.Sign(rotationSide));

    }


    public override void StopMovement()
    {
        m_rb.velocity = Vector3.zero;
    }
}

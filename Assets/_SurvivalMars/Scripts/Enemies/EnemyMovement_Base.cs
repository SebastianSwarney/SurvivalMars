using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement_Base : MonoBehaviour
{

    public GameObject m_playerObject;
    public Vector3 m_lastPlayerPostion;


    [Header("Floating Values")]
    public LayerMask m_floatingLayer;
    public float m_targetHeightAboveGround;
    public float m_maxRaycastDistance;
    public float m_maxBoost;
    public float m_heightStoppingDistance;
    public float m_heightBrakingDistance;
    [HideInInspector]
    public Rigidbody m_rb;
    public abstract void IdleMovement();

    public abstract void MoveToPlayer();

    public abstract void MoveToLastKnownPosition();

    public abstract void RotateToPoint(Vector3 p_point);

    public void PlayerLost()
    {
        m_lastPlayerPostion = m_playerObject.transform.position;
    }

    public abstract void StopMovement();

    public void FloatAboveGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_maxRaycastDistance, m_floatingLayer))
        {

            float distance = Vector3.Distance(transform.position, hit.point) - m_targetHeightAboveGround;
            float newVelocity = 0;
            if (Mathf.Abs(distance) > m_heightBrakingDistance)
            {
                newVelocity = -m_maxBoost * Mathf.Sign(distance);
            }
            else
            {
                if (Mathf.Abs(distance) > m_heightStoppingDistance)
                {
                    newVelocity = -m_maxBoost * (Mathf.Abs(distance) / m_heightBrakingDistance) * Mathf.Sign(distance);
                }
            }


            m_rb.velocity = new Vector3(m_rb.velocity.x, newVelocity, m_rb.velocity.z);
        }

    }
}

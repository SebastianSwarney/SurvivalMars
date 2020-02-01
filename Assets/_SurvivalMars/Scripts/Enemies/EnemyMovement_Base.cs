using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement_Base : MonoBehaviour
{

    public GameObject m_playerObject;
    public Vector3 m_lastPlayerPostion;
    public abstract void IdleMovement();

    public abstract void MoveToPlayer();

    public abstract void MoveToLastKnownPosition();

    public abstract void RotateToPoint(Vector3 p_point);

    public void PlayerLost()
    {
        m_lastPlayerPostion = m_playerObject.transform.position;
    }

    public abstract void StopMovement();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement_Base : MonoBehaviour
{

    public GameObject m_playerObject;
    private Vector3 m_lastPlayerPostion;
    public abstract void IdleMovement();

    public abstract void MoveToPlayer();

    public abstract void MoveToLastKnownPosition();

    public abstract void RunAway();

    public void PlayerLost()
    {
        m_lastPlayerPostion = m_playerObject.transform.position;
    }
}

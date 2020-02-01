using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    public EnemyController m_enemyToKill;
    public KeyCode m_killCode;

    private void Update()
    {
        if (Input.GetKeyDown(m_killCode))
        {
            m_enemyToKill.KillMe();
        }
    }
}

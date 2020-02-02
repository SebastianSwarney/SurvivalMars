using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundPound : UnityEngine.Events.UnityEvent { }
public class BigEnemyGroundPound : MonoBehaviour
{
    public GroundPound m_groundHit;

    private void OnTriggerEnter(Collider other)
    {
        m_groundHit.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    public GameObject m_particlePrefab;
    private ObjectPooler m_pooler;

    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }

    public void SpawnParticleFunction()
    {
        m_pooler.NewObject(m_particlePrefab, transform.position, Quaternion.identity);
    }
}

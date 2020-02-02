using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_ChangeSize : MonoBehaviour
{

    public float m_startSize, m_targetSize;
    private ParticleSystem m_particle;
    public AnimationCurve m_animCurve;
    public float m_effectTime;
    private ParticleSystem.ShapeModule m_particleShape;
    // Start is called before the first frame update
    void Awake()
    {
        m_particle = GetComponent<ParticleSystem>();
        m_particleShape = m_particle.shape;
    }

    private void Start()
    {
        StartCoroutine(ChangeRadius());
    }
    private IEnumerator ChangeRadius()
    {
        float timer = 0;
        while (timer < m_effectTime)
        {
            timer += Time.deltaTime;

            m_particleShape.radius = Mathf.Lerp(m_startSize, m_targetSize, m_animCurve.Evaluate(timer / m_effectTime));
            yield return null;
        }
    }
}

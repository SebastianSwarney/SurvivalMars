using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkPrefab : MonoBehaviour
{
    public float m_startSinkSpeed;
    [Tooltip("How far the object should sink")]
    public float m_distanceToSink;

    public float m_sinkTime;

    private Rigidbody m_rb;
    public float m_appliedGravity;
    [Header("debugging")]
    public bool m_debugging;
    public Color m_gizmosColor1;
    public Mesh m_mesh;
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        StartCoroutine(CheckSink());
    }

    private void FixedUpdate()
    {
        m_rb.velocity += Vector3.down * (m_appliedGravity / 50);
    }

    private IEnumerator CheckSink()
    {
        while (m_rb.velocity.magnitude > m_startSinkSpeed)
        {
            yield return null;
        }

        m_rb.isKinematic = true;
        Vector3 startSinkPos = transform.position;
        float sinkTimer = 0;
        while (sinkTimer < m_sinkTime)
        {
            transform.position = Vector3.Lerp(startSinkPos, new Vector3(startSinkPos.x, startSinkPos.y - m_distanceToSink, startSinkPos.z), sinkTimer / m_sinkTime);
            sinkTimer += Time.deltaTime;
            yield return null;
        }
        Destroy(m_rb);
        Destroy(this);
    }



    private void OnDrawGizmos()
    {
        if (!m_debugging) return;
        Gizmos.color = m_gizmosColor1;
        
        Gizmos.DrawWireMesh(m_mesh, new Vector3(transform.position.x, transform.position.y - m_distanceToSink, transform.position.z),transform.rotation,transform.localScale);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour, IAreaFx
{
    public float m_farthestDis;
    public float m_closestDis;
    public float m_shakeAmount;
    public float m_shakeTime;
    public GameObject m_shakeObject;

    private Coroutine m_shakeCoroutine;

    [Header("Debugging")]
    public bool m_debuggingTools;
    public Color m_gizmos1, m_gizmos2;

    private void StartShaking(float p_effectDistance)
    {

        StopAllCoroutines();
        m_shakeCoroutine = StartCoroutine(ShakeCamera(p_effectDistance));

    }

    private IEnumerator ShakeCamera(float p_effectDistance)
    {
        float percent = Mathf.Clamp(1 - ((p_effectDistance - m_closestDis) / (m_farthestDis - m_closestDis)),0,1);
        float fxAmount = Mathf.Lerp(0, m_shakeAmount, percent);
        float fxTime = 0;
        while (fxTime < m_shakeTime)
        {
            Vector2 newShakePos = Random.insideUnitCircle * Mathf.Lerp(fxAmount, 0, fxTime / m_shakeTime);
            m_shakeObject.transform.localPosition = newShakePos;
            fxTime += Time.deltaTime;
            yield return null;
        }
        m_shakeObject.transform.localPosition = Vector3.zero;
    }

    public void PerformEffect(Vector3 p_emitterPosition)
    {
        
        float effectDistance = Vector3.Distance(p_emitterPosition, m_shakeObject.transform.position);
        
        if (effectDistance < m_farthestDis)
        {

            StartShaking(effectDistance);
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debuggingTools) return;
        Gizmos.color = m_gizmos1;
        Gizmos.DrawWireSphere(transform.position, m_farthestDis);
        Gizmos.color = m_gizmos2;
        Gizmos.DrawWireSphere(transform.position, m_closestDis);
    }
}

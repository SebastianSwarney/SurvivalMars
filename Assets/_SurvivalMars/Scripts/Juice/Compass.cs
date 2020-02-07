using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public bool m_stayRelativetoWorld;
    public Transform m_targetTransform;
    public Transform m_transformToChange;
    public float angle;

    private void Start()
    {
        if(m_transformToChange == null)
        {
            m_transformToChange = transform;
        }
    }
    private void Update()
    {
        if (m_stayRelativetoWorld)
        {
            m_transformToChange.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);

        }
        else
        {
            float newAngle = Mathf.Abs(Vector3.Angle(new Vector3(transform.forward.x, 0f, transform.forward.z), Vector3.forward))*2;
            float rotateDir = Vector3.Dot(m_targetTransform.forward, Vector3.right);
            angle = newAngle;
            m_transformToChange.localEulerAngles = new Vector3(m_transformToChange.localEulerAngles.x, -(m_targetTransform.eulerAngles.y - (newAngle * Mathf.Sign(rotateDir))), m_transformToChange.localEulerAngles.z);

        }
        
    }

}

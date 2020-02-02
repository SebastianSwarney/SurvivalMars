using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyeMovement : MonoBehaviour
{
    public Transform m_eyeRootX;

    public float m_rotationSensitivity;

    public float m_rotationSpeed;

    public void MoveEyes(Vector3 p_targetPos)
    {

        //Create a direction towards the target
        Vector3 currentTarget = (new Vector3(p_targetPos.x, 0f, p_targetPos.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized * m_rotationSensitivity;


        //Isolate the target, so only one axis has to be worked with
        //Find the horizontal distance from the target to the turret.
        float disXFromTransform = Vector3.Distance(new Vector3(m_eyeRootX.position.x, 0f, m_eyeRootX.position.z), new Vector3(p_targetPos.x, 0, p_targetPos.z));

        //Find the difference in height
        float disYFromTarget = Mathf.Abs(p_targetPos.y - m_eyeRootX.position.y);

        //Use the distance and height to create a virtual target, that rotates with the y axis, thus negating the z axis
        currentTarget = transform.localRotation * new Vector3(0, disYFromTarget * (p_targetPos.y > m_eyeRootX.transform.position.y ? 1 : -1), disXFromTransform).normalized * m_rotationSensitivity;
        Debug.DrawLine(m_eyeRootX.position, currentTarget + m_eyeRootX.position, Color.green);

        //Find which way to turn, and how much to rotate
        float currentDistance = Vector3.Cross(m_eyeRootX.forward, currentTarget - m_eyeRootX.forward).magnitude;
        float dirToRotate = Vector3.Dot(-m_eyeRootX.up, currentTarget - m_eyeRootX.forward);
        //Edge case
        if (Vector3.Angle(m_eyeRootX.forward, currentTarget) > 150)
        {
            dirToRotate = 1;
            currentDistance = 1.5f;
        }

        //Rotate
        m_eyeRootX.Rotate(Vector3.right, (currentDistance > 3 ? m_rotationSpeed: currentDistance / 3 * m_rotationSpeed) * Mathf.Sign(dirToRotate));


    }
}

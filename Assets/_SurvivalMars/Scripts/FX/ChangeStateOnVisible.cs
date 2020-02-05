using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStateOnVisible : MonoBehaviour
{
    public List<GameObject> m_changeObjects;
    public float m_angleToDisable;

    private Coroutine m_disableCoroutine;
    private List<Camera> m_mainCameras = new List<Camera>();
    private EnemyController m_controller;

    private bool m_disabled;

    private void Start()
    {
        m_mainCameras.Add(PlayerPossesionController.Instance.m_playerChar.m_viewCamera);
        m_mainCameras.Add(PlayerPossesionController.Instance.m_rover.m_viewCamera);
        m_controller = GetComponent<EnemyController>();
        m_disableCoroutine = StartCoroutine(CheckAngleFromCamera());
        
    }


    private IEnumerator CheckAngleFromCamera()
    {
        while(true)
        {
            yield return null;

            foreach(Camera newCam in m_mainCameras)
            {
                if (newCam.gameObject.activeSelf)
                {
                    Vector3 dir = newCam.transform.forward;
                    Vector3 dirToMe = transform.position - newCam.transform.position;
                    if (Vector3.Angle(dir, dirToMe) > m_angleToDisable)
                    {
                        if (!m_disabled)
                        {
                            foreach (GameObject currentObjects in m_changeObjects)
                            {
                                currentObjects.SetActive(false);
                            }
                            m_disabled = true;
                        }
                    }
                    else
                    {
                        if (m_disabled)
                        {
                            if (m_controller.m_currentState != EnemyController.EnemyState.Died)
                            {
                                foreach (GameObject currentObjects in m_changeObjects)
                                {
                                    currentObjects.SetActive(true);
                                }
                            }
                            m_disabled = false;
                        }
                        
                    }
                }
            }
        }
    }
}

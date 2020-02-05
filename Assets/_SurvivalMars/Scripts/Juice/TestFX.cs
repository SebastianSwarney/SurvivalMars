using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FXEvent : UnityEngine.Events.UnityEvent <Vector3>{ }
public class TestFX : MonoBehaviour
{
    public FXEvent m_testEvent;

    public KeyCode m_testKeycode;

    private void Update()
    {
        if (Input.GetKeyDown(m_testKeycode))
        {
            m_testEvent.Invoke(transform.position);
        }   
    }

}

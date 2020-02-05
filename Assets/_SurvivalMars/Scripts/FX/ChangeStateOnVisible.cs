using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStateOnVisible : MonoBehaviour
{
    public List<GameObject> m_changeObjects;

    private void OnBecameInvisible()
    {
        foreach(GameObject currentObjects in m_changeObjects)
        {
            currentObjects.SetActive(false);
        }
        print("Become Invisible");
    }
    private void OnBecameVisible()
    {
        foreach (GameObject currentObjects in m_changeObjects)
        {
            currentObjects.SetActive(true);
        }
        print("Become Visible");
    }
}

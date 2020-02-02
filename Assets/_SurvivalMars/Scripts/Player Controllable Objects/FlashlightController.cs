using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
	private Light m_light;

	public void ActivateLight()
	{
		m_light.enabled = true;
	}

	public void DeactivateLight()
	{
		m_light.enabled = false;
	}

	public void TriggerLightBurst()
	{

	}

	public IEnumerator RunLightBrust()
	{
		float t = 0;
	}
}

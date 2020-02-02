using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aura2API;

public class FlashlightController : MonoBehaviour
{
	public Light m_mainLight;
	public AuraLight m_auraLight;

	public void ActivateLight()
	{
		m_mainLight.enabled = true;
		m_auraLight.enabled = true;
	}

	public void DeactivateLight()
	{
		m_mainLight.enabled = false;
		m_auraLight.enabled = false;
	}
}

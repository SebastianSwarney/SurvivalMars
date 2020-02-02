using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
	[HideInInspector]
	public ObjectHolder m_currentHolder;
	[HideInInspector]
	public Rigidbody m_rigidbody;

	private void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
	}
}

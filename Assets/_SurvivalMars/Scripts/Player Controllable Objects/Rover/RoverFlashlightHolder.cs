using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverFlashlightHolder : ObjectHolder
{
	public Transform m_objectHoldTransform;
	private RoverController m_rover;

	private void Start()
	{
		m_rover = GetComponent<RoverController>();
	}

	private void FixedUpdate()
	{
		if (m_holdingObject)
		{
			HoldingObject();
		}
	}

	public override void SelectObject(PickupObject p_newObject)
	{
		base.SelectObject(p_newObject);
		m_rover.m_hasFlashlight = true;
	}

	public override void DeselectObject()
	{
		base.DeselectObject();
		m_rover.m_hasFlashlight = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckCollisionLayer(m_objectPickupLayers, other.gameObject))
		{
			SelectObject(other.GetComponentInParent<PickupObject>());
		}
	}

	private void HoldingObject()
	{
		m_pickupObject.transform.position = m_objectHoldTransform.position;
		m_pickupObject.transform.rotation = m_objectHoldTransform.rotation;
	}
}

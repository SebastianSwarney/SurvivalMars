using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverFlashlightHolder : ObjectHolder
{
	public Transform m_objectHoldTransform;

	private void FixedUpdate()
	{
		if (m_holdingObject)
		{
			HoldingObject();
		}
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

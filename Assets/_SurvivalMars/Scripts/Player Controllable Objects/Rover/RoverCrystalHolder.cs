using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverCrystalHolder : ObjectHolder
{
	public int m_crystalAmount;
	
	public override void SelectObject(PickupObject p_newObject)
	{
		base.SelectObject(p_newObject);
		m_crystalAmount++;

		m_pickupObject.gameObject.SetActive(false);
		DeselectObject();
	}

	public override void DeselectObject()
	{
		base.DeselectObject();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckCollisionLayer(m_objectPickupLayers, other.gameObject))
		{
			SelectObject(other.GetComponentInParent<PickupObject>());
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrystalHolder : ObjectHolder
{
	public Transform m_objectHoldTransform;

	public float m_objectFollowSpeed;
	public float m_objectRotateSpeed;

	private Camera m_viewCam;

	private void Start()
	{
		m_viewCam = GetComponentInChildren<Camera>();
	}

	private void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!m_holdingObject)
			{
				FindObject();
			}
			else
			{
				DeselectObject();
			}
		}

		if (m_holdingObject)
		{
			HoldingObject();
		}
	}

	private void FindObject()
	{
		RaycastHit hit;

		if (Physics.SphereCast(m_viewCam.transform.position, 1f, m_viewCam.transform.forward, out hit, Mathf.Infinity, m_objectPickupLayers))
		{
			PickupObject foundObject = hit.collider.GetComponentInParent<PickupObject>();

			SelectObject(foundObject);
		}
	}

	private void HoldingObject()
	{
		Vector3 targetPos = Vector3.Lerp(m_pickupObject.transform.position, m_objectHoldTransform.position, m_objectFollowSpeed);

		PhysicsSeekTo(m_pickupObject.m_rigidbody, targetPos);

		Quaternion targetRot = Quaternion.Slerp(m_pickupObject.transform.rotation, m_objectHoldTransform.rotation, m_objectRotateSpeed);
		PhysicsRotateTo(m_pickupObject.m_rigidbody, targetRot);
	}
}

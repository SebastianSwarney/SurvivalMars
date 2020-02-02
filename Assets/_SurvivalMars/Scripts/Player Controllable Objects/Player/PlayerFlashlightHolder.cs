using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightHolder : ObjectHolder
{
	public Transform m_objectHoldTransform;

	public Transform m_crystalHoldTransform;

	public float m_objectHoldDistance;
	public float m_objectFollowSpeed;
	public float m_objectRotateSpeed;

	private Camera m_viewCam;

	private float m_yPosSmoothVelocity;

	private bool m_hasCrystal;

	private void Start()
	{
		m_viewCam = GetComponentInChildren<Camera>();
	}

	private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.F))
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

	public void OnGetCrystal()
	{
		m_hasCrystal = true;

		m_pickupObject.gameObject.SetActive(false);
	}

	public void OnDropCrystal()
	{
		m_hasCrystal = false;
		m_pickupObject.gameObject.SetActive(true);
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
		if (!m_hasCrystal)
		{
			Vector3 targetPos = Vector3.Lerp(m_pickupObject.transform.position, m_objectHoldTransform.position, m_objectFollowSpeed);
			PhysicsSeekTo(m_pickupObject.m_rigidbody, targetPos);

			Quaternion targetRot = Quaternion.Slerp(m_pickupObject.transform.rotation, m_objectHoldTransform.rotation, m_objectRotateSpeed);
			PhysicsRotateTo(m_pickupObject.m_rigidbody, targetRot);
		}
		else
		{
			Vector3 targetPos = Vector3.Lerp(m_pickupObject.transform.position, m_crystalHoldTransform.position, m_objectFollowSpeed);
			PhysicsSeekTo(m_pickupObject.m_rigidbody, targetPos);

			Quaternion targetRot = Quaternion.Slerp(m_pickupObject.transform.rotation, m_crystalHoldTransform.rotation, m_objectRotateSpeed);
			PhysicsRotateTo(m_pickupObject.m_rigidbody, targetRot);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightHolder : ObjectHolder
{
	public Transform m_objectHoldTransform;

	public Transform m_crystalHoldTransform;

	public float m_objectFollowSpeed;
	public float m_objectRotateSpeed;
	public PickupObject m_flashLight;

	private Camera m_viewCam;
	private bool m_hasCrystal;

	private FlashlightController m_flashLightComponent;

	private void Start()
	{
		m_viewCam = GetComponentInChildren<Camera>();

		m_flashLightComponent = m_flashLight.GetComponent<FlashlightController>();
		SelectObject(m_flashLight);
	}

	private void FixedUpdate()
	{
		if (m_holdingObject)
		{
			HoldingObject();
		}
	}

	public void OnLightBurstInputDown()
	{
		if (m_holdingObject)
		{
			m_flashLightComponent.TriggerLightBurst();
		}
	}

	public void OnFlashLightPickupInputDown()
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

	public override void SelectObject(PickupObject p_newObject)
	{
		base.SelectObject(p_newObject);
		m_flashLightComponent.ActivateLight();
	}

	public override void DeselectObject()
	{
		base.DeselectObject();
		m_flashLightComponent.DeactivateLight();
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

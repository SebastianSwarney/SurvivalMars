using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlightHolder : ObjectHolder
{
	public ObjectHolderEvent m_onFlashLightFlickerEvent;
	public ObjectHolderEvent m_onFlashLightBurstEvent;

	public Transform m_objectHoldAnchor;
	public Transform m_objectHoldTransform;

	public Transform m_crystalHoldTransform;

	public float m_objectFollowSpeed;
	public float m_objectRotateSpeed;

	//private PickupObject m_flashLight;

	public float m_flashBurstTime;

	public float m_flashBurstAngle;
	public float m_flashBurstIntensity;
	public float m_flashBurstRange;

	public AnimationCurve m_flashBurstCurve;
	public LayerMask m_enemyMask;
	public float m_flashBurstCastRadius;
	public float m_flashBurstCastDst;

	public float m_flashLightRechargeTime;

	private Camera m_viewCam;
	private bool m_hasCrystal;

	private FlashlightController m_flashLightComponent;

	private bool m_canUseFlashlight = true;

	private Vector3 m_followSmoothing;

	public float m_verticalBobFreq;
	public float m_verticalBobAmount;

	private PlayerController m_palyerController;

	private float m_bobingAnimationTimer;

	public float m_damper;
	public float m_stiffness;

	private Vector3 m_springVelocity;
	private Vector3 m_springSmoothVelocity;

	private void Start()
	{
		m_viewCam = GetComponentInChildren<Camera>();

		m_flashLightComponent = FindObjectOfType<FlashlightController>();
		SelectObject(m_flashLightComponent.GetComponent<PickupObject>());

		m_palyerController = GetComponent<PlayerController>();

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
			//m_flashLightComponent.TriggerLightBurst();

			if (m_canUseFlashlight)
			{
				StartCoroutine(RunFlashBurst(m_flashLightComponent.m_mainLight));
			}
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

		if (m_pickupObject != null)
		{
			m_pickupObject.gameObject.SetActive(false);
		}
	}

	public void OnDropCrystal()
	{
		m_hasCrystal = false;

		if (m_pickupObject != null)
		{
			m_pickupObject.gameObject.SetActive(true);
		}
	}

	public override void SelectObject(PickupObject p_newObject)
	{
		base.SelectObject(p_newObject);
		m_flashLightComponent.ActivateLight();

		//m_pickupObject.transform.parent = m_objectHoldTransform;
		//m_pickupObject.transform.position = m_objectHoldTransform.position;
		//m_pickupObject.transform.rotation = m_objectHoldTransform.rotation;
		m_pickupObject.m_rigidbody.isKinematic = true;
	}

	public override void DeselectObject()
	{
		m_pickupObject.transform.parent = null;
		m_pickupObject.m_rigidbody.isKinematic = false;

		base.DeselectObject();
		m_flashLightComponent.DeactivateLight();
	}


	private IEnumerator RunFlashBurst(Light p_targetLight)
	{
		m_canUseFlashlight = false;

		float t = 0;

		float startAngle = p_targetLight.spotAngle;
		float startIntesity = p_targetLight.intensity;
		float startRange = p_targetLight.range;

		while (t < m_flashBurstTime)
		{
			t += Time.deltaTime;

			float progress = m_flashBurstCurve.Evaluate(t / m_flashBurstTime);

			float targetAngle = Mathf.Lerp(startAngle, m_flashBurstAngle, progress);
			p_targetLight.spotAngle = targetAngle;

			float targetIntestity = Mathf.Lerp(startIntesity, m_flashBurstIntensity, progress);
			p_targetLight.intensity = targetIntestity;

			float targetRange = Mathf.Lerp(startRange, m_flashBurstRange, progress);
			p_targetLight.range = targetRange;

			yield return null;
		}

		RaycastHit[] hits = Physics.SphereCastAll(m_viewCam.transform.position, m_flashBurstAngle, m_viewCam.transform.forward, m_flashBurstRange, m_enemyMask);

		foreach (RaycastHit enemy in hits)
		{
            EnemyController newEnemy = enemy.transform.GetComponent<EnemyController>();
            if (newEnemy != null)
            {
                newEnemy.KillMe();
            }
			
		}

		p_targetLight.spotAngle = startAngle;
		p_targetLight.intensity = startIntesity;
		p_targetLight.range = startRange;

		m_onFlashLightBurstEvent.Invoke();
		m_flashLightComponent.DeactivateLight();

		StartCoroutine(FlashlightRecharge());
	}

	private IEnumerator FlashlightRecharge()
	{
		float t = 0;

		while (t < m_flashLightRechargeTime)
		{
			t += Time.deltaTime;

			float flashValue = Mathf.Sin(Mathf.Pow(t, 3));

			bool flashlightOn = flashValue < -0.5f;

			if (flashlightOn)
			{
				m_onFlashLightFlickerEvent.Invoke();

				m_flashLightComponent.ActivateLight();
			}
			else
			{
				m_flashLightComponent.DeactivateLight();
			}

			yield return null;
		}

		m_canUseFlashlight = true;

		m_flashLightComponent.ActivateLight();
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
		//float force = (-stiffness * displacement) - (damping * m_jumpVelocity);
		//m_pickupObject.m_rigidbody.AddForce((-m_stiffness * (m_pickupObject.transform.position - m_objectHoldTransform.position) - m_damper * m_pickupObject.m_rigidbody.velocity));

		Vector3 springForce = -m_stiffness * (m_pickupObject.transform.position - m_objectHoldTransform.position);
		Vector3 dampingForce = m_damper * m_springVelocity;
		Vector3 force = springForce + Vector3.one - dampingForce;
		Vector3 accel = force / 1f;
		m_springVelocity = m_springVelocity + accel * Time.fixedDeltaTime;
		Vector3 targetPos = m_pickupObject.transform.position + m_springVelocity * Time.fixedDeltaTime;

		m_pickupObject.transform.position = m_objectHoldTransform.position;
		m_pickupObject.transform.position = new Vector3(m_pickupObject.transform.position.x, targetPos.y, m_pickupObject.transform.position.z);

		m_pickupObject.transform.rotation = m_objectHoldTransform.rotation;

		if (m_palyerController.IsGrounded())
		{
			m_bobingAnimationTimer += Time.fixedDeltaTime;
			float bobbingAnimationPhase = ((Mathf.Sin(Time.time * m_verticalBobFreq) * 0.5f) + 0.5f) * m_verticalBobAmount;
			m_objectHoldTransform.position = m_objectHoldAnchor.position + Vector3.up * bobbingAnimationPhase;
		}
		else
		{
			m_bobingAnimationTimer = 0;
		}

		/*
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
		*/
	}
}

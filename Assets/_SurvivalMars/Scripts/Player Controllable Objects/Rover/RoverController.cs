using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverController : MonoBehaviour
{
	public enum MovementControllState { MovementEnabled, MovementDisabled }
	public MovementControllState m_movementControllState;

	public bool m_hasFlashlight;

	public float m_movementSpeed;

	public Transform[] m_bottomThrusters;

	public LayerMask m_groundMask;

	public float m_targetDistanceFromGround;
	public float m_targetDistanceFromGroundDeactivated;

	public float m_rotationAdjustSpeed;
	public float m_heightWeight;


	private Rigidbody m_body;
	private Vector3 m_averageRotation;
	private float m_averageDistance;
	private Vector2 m_input;
	public Camera m_viewCamera;

	public Transform m_playerHoldPos;

	public float m_exitDst;

	public GameObject[] m_activationLights;

	public GameObject[] m_redLights;

	private float m_lastActivationTime;

    [Header("Rover Stick Visual")]
    public Transform m_stickVisual;
    public float m_maxStickAngle;
    public float m_rotateStickSpeed;

	private void Start()
	{
		m_body = GetComponentInParent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (m_movementControllState == MovementControllState.MovementEnabled)
		{
			MoveRover();
		}

		if (m_hasFlashlight)
		{
			foreach (GameObject light in m_activationLights)
			{
				light.SetActive(true);
			}

			foreach (GameObject light in m_redLights)
			{
				light.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject light in m_activationLights)
			{
				light.SetActive(false);
			}

			foreach (GameObject light in m_redLights)
			{
				light.SetActive(true);
			}
		}

		Float();
        MoveStick();

    }

	public void OnPlayerExitInputDown()
	{
		if (Time.time > m_lastActivationTime + 0.1f)
		{
			OnPlayerExit();
		}
	}

	private void OnPlayerExit()
	{
		PlayerPossesionController.Instance.m_playerChar.transform.position = CheckExitPos();
		PlayerPossesionController.Instance.ControllPlayer();
	}

	public void Deactivate()
	{
		m_movementControllState = MovementControllState.MovementDisabled;
		m_viewCamera.enabled = false;

		PlayerPossesionController.Instance.m_playerChar.transform.parent = null;
	}

	public void Activate()
	{
		m_movementControllState = MovementControllState.MovementEnabled;
		m_viewCamera.enabled = true;

		PlayerPossesionController.Instance.m_playerChar.transform.parent = m_playerHoldPos;
		PlayerPossesionController.Instance.m_playerChar.transform.position = m_playerHoldPos.position;

		m_lastActivationTime = Time.time;
	}

	private Vector3 CheckExitPos()
	{
		Vector3 exitPos = Vector3.zero;

		RaycastHit hitRight;

		if (!Physics.SphereCast(transform.position, 1f, transform.right, out hitRight, m_exitDst, m_groundMask))
		{
			exitPos = transform.position + (transform.right * m_exitDst);

			return exitPos;
		}

		RaycastHit hitLeft;

		if (!Physics.SphereCast(transform.position, 1f, -transform.right, out hitLeft, m_exitDst, m_groundMask))
		{
			exitPos = transform.position + (-transform.right * m_exitDst);

			return exitPos;
		}

		exitPos = transform.position + (transform.up * m_exitDst);

		return exitPos;
	}


	public void SetMovementInput(Vector2 p_input)
	{
		m_input = p_input;
	}

	private void ThrusterMovement(Transform p_thrusterPosition, float p_input, float p_speed)
	{
		m_body.AddForceAtPosition(p_thrusterPosition.forward * p_speed * p_input, p_thrusterPosition.position, ForceMode.Force);
	}

	private void MoveRover()
	{
		Vector3 forwardMovement = transform.forward * m_input.y;
		Vector3 rightMovement = transform.right * m_input.x;

		Vector3 targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) *  m_movementSpeed;
		Vector3 targetRotation = new Vector3(0, m_input.x, 0) * m_movementSpeed;

		m_body.AddForce(targetHorizontalMovement, ForceMode.Force);
		m_body.AddTorque(targetRotation, ForceMode.Force);
	}

	private void Float()
	{
		m_averageDistance = 0;

		for (int i = 0; i < m_bottomThrusters.Length; i++)
		{
			RaycastHit hit;

			if (Physics.Raycast(m_bottomThrusters[i].position, -m_bottomThrusters[i].up, out hit, Mathf.Infinity, m_groundMask))
			{
				Debug.DrawLine(m_bottomThrusters[i].position, hit.point);

				m_averageRotation += hit.normal;
				m_averageDistance += hit.distance;
			}
		}

		m_averageRotation /= m_bottomThrusters.Length;
		m_averageDistance /= m_bottomThrusters.Length;

		Vector3 inverse = transform.InverseTransformDirection(m_averageRotation);

		Quaternion currentRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((inverse.x * Mathf.Rad2Deg), transform.localEulerAngles.y, (inverse.z * Mathf.Rad2Deg)), m_rotationAdjustSpeed);
		transform.rotation = currentRotation;

		float targetHeight = ((m_targetDistanceFromGround + transform.position.y) / (m_averageDistance)) * m_targetDistanceFromGround;

		if (m_movementControllState == MovementControllState.MovementEnabled)
		{
			targetHeight = ((m_targetDistanceFromGround + transform.position.y) / (m_averageDistance)) * m_targetDistanceFromGround;
		}
		else
		{
			targetHeight = ((m_targetDistanceFromGroundDeactivated + transform.position.y) / (m_averageDistance)) * m_targetDistanceFromGroundDeactivated;
		}

		float currentHeight = transform.position.y * m_heightWeight + targetHeight * (1 - m_heightWeight);
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
	}


    private void MoveStick()
    {
        Vector2 targetAngle = new Vector3(m_input.y * m_maxStickAngle, m_input.x * m_maxStickAngle);
        m_stickVisual.transform.localRotation = Quaternion.Lerp(m_stickVisual.transform.rotation, Quaternion.Euler(targetAngle.x, targetAngle.y, 0), m_rotateStickSpeed);
    }
	private void OnDrawGizmos()
	{
		foreach (Transform thruster in m_bottomThrusters)
		{
			Debug.DrawRay(thruster.transform.position, -thruster.up * 10, Color.red);
		}
	}
}

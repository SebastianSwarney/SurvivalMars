using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PlayerControllerEvent : UnityEvent { }

public class PlayerController : MonoBehaviour
{
	public bool m_isPlaying;

	public enum MovementControllState { MovementEnabled, MovementDisabled }
	public enum GravityState { GravityEnabled, GravityDisabled }
	public enum DamageState { Vulnerable, Invulnerable }
	public enum InputState { InputEnabled, InputDisabled }
	public PlayerState m_states;

	#region Movement Events
	public PlayerMovementEvents m_movementEvents;
	[System.Serializable]
	public struct PlayerMovementEvents
	{
		[Header("Basic Events")]
		public PlayerControllerEvent m_onLandedEvent;
		public PlayerControllerEvent m_onJumpEvent;
		public PlayerControllerEvent m_onRespawnEvent;
	}
	#endregion

	#region Camera Properties
	[Header("Camera Properties")]

	public float m_mouseSensitivity;
	public float m_maxCameraAng;
	public bool m_inverted;
	public Camera m_viewCamera;
	public Transform m_cameraRotation;
	[Space]
	#endregion

	#region Base Movement Properties
	[Header("Base Movement Properties")]

	public float m_baseMovementSpeed;
	public float m_accelerationTime;

	private float m_currentMovementSpeed;
	[HideInInspector]
	public Vector3 m_velocity;
	private Vector3 m_velocitySmoothing;
	private CharacterController m_characterController;

	[Space]
	#endregion

	#region Jumping Properties
	[Header("Jumping Properties")]

	public float m_maxJumpHeight = 4;
	public float m_minJumpHeight = 1;
	public float m_timeToJumpApex = .4f;

	public float m_graceTime;
	private float m_graceTimer;

	public float m_jumpBufferTime;
	private float m_jumpBufferTimer;

	private float m_gravity;
	private float m_maxJumpVelocity;
	private float m_minJumpVelocity;
	private bool m_isLanded;
	private bool m_offLedge;

	[Space]
	#endregion

	private Vector2 m_movementInput;
	private Vector2 m_lookInput;

	private Coroutine m_jumpBufferCoroutine;
	private Coroutine m_graceBufferCoroutine;

	private float m_currentSpeedBoost;

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();

		CalculateJump();
		LockCursor();

		m_currentMovementSpeed = m_baseMovementSpeed;
		m_jumpBufferTimer = m_jumpBufferTime;
	}

	private void OnValidate()
	{
		CalculateJump();
	}

	private void FixedUpdate()
	{
		PerformController();
	}

	public void PerformController()
	{
		CalculateCurrentSpeed();
		CalculateVelocity();

		m_characterController.Move(m_velocity * Time.deltaTime);

		CalculateGroundPhysics();

		CameraRotation();
	}

	public void Deactivate()
	{
		m_states.m_movementControllState = MovementControllState.MovementDisabled;
	}

	public void Activate()
	{
		m_states.m_movementControllState = MovementControllState.MovementEnabled;
	}

	#region Input Code
	public void SetMovementInput(Vector2 p_input)
	{
		m_movementInput = p_input;
	}

	public void SetLookInput(Vector2 p_input)
	{
		m_lookInput = p_input;
	}
	#endregion

	#region Camera Code
	private void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void ResetCamera()
	{
		m_cameraRotation.rotation = Quaternion.identity;
	}

	private void CameraRotation()
	{
		//Get the inputs for the camera
		Vector2 cameraInput = new Vector2(m_lookInput.y * ((m_inverted) ? -1 : 1), m_lookInput.x);

		//Rotate the player on the y axis (left and right)
		transform.Rotate(Vector3.up, cameraInput.y * (m_mouseSensitivity));

		float cameraXAng = m_cameraRotation.transform.eulerAngles.x;

		//Stops the camera from rotating, if it hits the resrictions
		if (cameraInput.x < 0 && cameraXAng > 360 - m_maxCameraAng || cameraInput.x < 0 && cameraXAng < m_maxCameraAng + 10)
		{
			m_cameraRotation.transform.Rotate(Vector3.right, cameraInput.x * (m_mouseSensitivity));

		}
		else if (cameraInput.x > 0 && cameraXAng > 360 - m_maxCameraAng - 10 || cameraInput.x > 0 && cameraXAng < m_maxCameraAng)
		{
			m_cameraRotation.transform.Rotate(Vector3.right, cameraInput.x * (m_mouseSensitivity));

		}

		if (m_cameraRotation.transform.eulerAngles.x < 360 - m_maxCameraAng && m_cameraRotation.transform.eulerAngles.x > 180)
		{
			m_cameraRotation.transform.localEulerAngles = new Vector3(360 - m_maxCameraAng, 0f, 0f);
		}
		else if (m_viewCamera.transform.eulerAngles.x > m_maxCameraAng && m_cameraRotation.transform.eulerAngles.x < 180)
		{
			m_cameraRotation.transform.localEulerAngles = new Vector3(m_maxCameraAng, 0f, 0f);
		}

	}
	#endregion

	#region Input Buffering Code

	private bool CheckBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
	{
		if (p_bufferTimer < p_bufferTime)
		{
			if (p_bufferTimerRoutine != null)
			{
				StopCoroutine(p_bufferTimerRoutine);
			}

			p_bufferTimer = p_bufferTime;

			return true;
		}
		else if (p_bufferTimer >= p_bufferTime)
		{
			return false;
		}

		return false;
	}

	private bool CheckOverBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
	{
		if (p_bufferTimer >= p_bufferTime)
		{
			p_bufferTimer = p_bufferTime;

			return true;
		}

		return false;
	}

	//Might want to change this so it does not feed the garbage collector monster
	private IEnumerator RunBufferTimer(System.Action<float> m_bufferTimerRef, float p_bufferTime)
	{
		float t = 0;

		while (t < p_bufferTime)
		{
			t += Time.deltaTime;
			m_bufferTimerRef(t);
			yield return null;
		}

		m_bufferTimerRef(p_bufferTime);
	}

	#endregion

	#region Player State Code
	[System.Serializable]
	public struct PlayerState
	{
		public MovementControllState m_movementControllState;
		public GravityState m_gravityControllState;
		public DamageState m_damageState;
		public InputState m_inputState;
	}

	private bool IsGrounded()
	{
		if (m_characterController.collisionFlags == CollisionFlags.Below)
		{
			return true;
		}

		return false;
	}

	private bool OnSlope()
	{
		RaycastHit hit;

		Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

		if (Physics.Raycast(bottom, Vector3.down, out hit, 0.5f))
		{
			if (hit.normal != Vector3.up)
			{
				return true;
			}
		}

		return false;
	}

	private void OnLanded()
	{
		m_isLanded = true;

		m_movementEvents.m_onLandedEvent.Invoke();
	}

	private void OnOffLedge()
	{
		m_offLedge = true;

		m_graceBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_graceTimer = (x), m_graceTime));

	}

	public void Respawn()
	{
		m_movementEvents.m_onRespawnEvent.Invoke();

		ResetCamera();
	}
	#endregion

	#region Physics Calculation Code

	private void CalculateCurrentSpeed()
	{
		float speed = m_baseMovementSpeed;
		speed += m_currentSpeedBoost;

		m_currentMovementSpeed = speed;
	}

	public void SpeedBoost(float p_boostAmount)
	{
		m_currentSpeedBoost = p_boostAmount;
	}

	private void CalculateGroundPhysics()
	{
		if (IsGrounded() && !OnSlope())
		{
			m_velocity.y = 0;
		}

		if (OnSlope())
		{
			RaycastHit hit;

			Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

			if (Physics.Raycast(bottom, Vector3.down, out hit))
			{
				m_characterController.Move(new Vector3(0, -(hit.distance), 0));
			}
		}

		if (!IsGrounded() && !m_offLedge)
		{
			OnOffLedge();
		}
		if (IsGrounded())
		{
			m_offLedge = false;
		}

		if (IsGrounded() && !m_isLanded)
		{
			OnLanded();
		}
		if (!IsGrounded())
		{
			m_isLanded = false;
		}
	}

	private void CalculateVelocity()
	{
		if (m_states.m_gravityControllState == GravityState.GravityEnabled)
		{
			m_velocity.y += m_gravity * Time.deltaTime;
		}

		if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
		{
			Vector2 input = new Vector2(m_movementInput.x, m_movementInput.y);

			Vector3 forwardMovement = transform.forward * input.y;
			Vector3 rightMovement = transform.right * input.x;

			Vector3 targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * m_currentMovementSpeed;
			Vector3 horizontalMovement = Vector3.SmoothDamp(m_velocity, targetHorizontalMovement, ref m_velocitySmoothing, m_accelerationTime);

			m_velocity = new Vector3(horizontalMovement.x, m_velocity.y, horizontalMovement.z);
		}
		else
		{
			Vector2 input = new Vector2(0, 0);

			Vector3 forwardMovement = transform.forward * input.y;
			Vector3 rightMovement = transform.right * input.x;

			Vector3 targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * m_currentMovementSpeed;
			Vector3 horizontalMovement = Vector3.SmoothDamp(m_velocity, targetHorizontalMovement, ref m_velocitySmoothing, m_accelerationTime);

			m_velocity = new Vector3(horizontalMovement.x, m_velocity.y, horizontalMovement.z);
		}

	}

	public void PhysicsSeekTo(Vector3 p_targetPosition)
	{
		Vector3 deltaPosition = p_targetPosition - transform.position;
		m_velocity = deltaPosition / Time.deltaTime;
	}
	#endregion

	#region Jump Code
	public void OnJumpInputDown()
	{
		m_jumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_jumpBufferTimer = (x), m_jumpBufferTime));

		if (CheckBuffer(ref m_graceTimer, ref m_graceTime, m_graceBufferCoroutine) && !IsGrounded() && m_velocity.y <= 0f)
		{
			GroundJump();
			return;
		}

		if (IsGrounded())
		{
			GroundJump();
			return;
		}

	}

	public void OnJumpInputUp()
	{
		if (m_velocity.y > m_minJumpVelocity)
		{
			JumpMinVelocity();
		}
	}

	private void CalculateJump()
	{
		m_gravity = -(2 * m_maxJumpHeight) / Mathf.Pow(m_timeToJumpApex, 2);
		m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
		m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_minJumpHeight);
	}

	private void GroundJump()
	{
		m_movementEvents.m_onJumpEvent.Invoke();
		JumpMaxVelocity();
	}

	private void JumpMaxVelocity()
	{
		m_velocity.y = m_maxJumpVelocity;
	}

	private void JumpMinVelocity()
	{
		m_velocity.y = m_minJumpVelocity;
	}

	private void JumpMaxMultiplied(float p_force)
	{
		m_velocity.y = m_maxJumpVelocity * p_force;
	}

	#endregion

	public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
	{
		if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
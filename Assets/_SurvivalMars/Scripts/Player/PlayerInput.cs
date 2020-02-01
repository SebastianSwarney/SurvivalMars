using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	/*
	public int m_playerId;
	public float m_shortDashInputSpeed;

	private PlayerController m_playerController;

	private bool m_lockLooking;

	private void Start()
	{
		m_playerController = GetComponent<PlayerController>();
	}

	private void Update()
	{
		GetInput();
	}

	public void GetInput()
	{
		Vector2 movementInput = new Vector2(m_playerInputController.GetAxis("MoveHorizontal"), m_playerInputController.GetAxis("MoveVertical"));
		m_playerController.SetMovementInput(movementInput);

		if (Input.GetKeyDown(KeyCode.P))
		{
			m_lockLooking = !m_lockLooking;
		}

		if (!m_lockLooking)
		{
			Vector2 lookInput = new Vector2(m_playerInputController.GetAxis("LookHorizontal"), m_playerInputController.GetAxis("LookVertical"));
			m_playerController.SetLookInput(lookInput);
		}

		if (m_playerInputController.GetButtonDown("Jump"))
		{
			m_playerController.OnJumpInputDown();
		}

		if (m_playerInputController.GetButtonUp("Jump"))
		{
			m_playerController.OnJumpInputUp();
		}
	}
	*/
}

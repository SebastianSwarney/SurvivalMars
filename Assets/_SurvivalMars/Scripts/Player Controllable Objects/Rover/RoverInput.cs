using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RoverInput : MonoBehaviour
{
	public int m_playerId;

	private RoverController m_roverController;
	private Player m_playerInputController;

	private bool m_lockLooking;

	private void Start()
	{
		m_roverController = GetComponent<RoverController>();
		m_playerInputController = ReInput.players.GetPlayer(m_playerId);
	}

	private void Update()
	{
		GetInput();
	}

	public void GetInput()
	{
		Vector2 movementInput = new Vector2(m_playerInputController.GetAxis("MoveHorizontal"), m_playerInputController.GetAxis("MoveVertical"));
		m_roverController.SetMovementInput(movementInput);

		if (m_playerInputController.GetButtonDown("Enter"))
		{
			m_roverController.OnPlayerExitInputDown();
		}
	}
}

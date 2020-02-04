using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour
{
	public int m_playerId;

	private PlayerController m_playerController;
	private Player m_playerInputController;
	private PlayerFlashlightHolder m_playerFlashlightHolder;
	private PlayerCrystalHolder m_playerCrystalHolder;

	private bool m_lockLooking;

	private void Start()
	{
		m_playerController = GetComponent<PlayerController>();
		m_playerFlashlightHolder = GetComponent<PlayerFlashlightHolder>();
		m_playerCrystalHolder = GetComponent<PlayerCrystalHolder>();
		m_playerInputController = ReInput.players.GetPlayer(m_playerId);
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

		if (m_playerInputController.GetButtonDown("PickupCrystal"))
		{
			m_playerCrystalHolder.OnPickupCrystalInputDown();
		}

		if (m_playerInputController.GetButtonDown("FlashBurst"))
		{
			m_playerFlashlightHolder.OnLightBurstInputDown();
		}

		if (m_playerInputController.GetButtonDown("FlashLightPickup"))
		{
			m_playerFlashlightHolder.OnFlashLightPickupInputDown();
		}

		if (m_playerInputController.GetButtonDown("Enter"))
		{
			m_playerController.OnEnterInputDown();
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
}

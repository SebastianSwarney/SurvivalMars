using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossesionController : MonoBehaviour
{
	public static PlayerPossesionController Instance;

	public PlayerController m_playerChar;
	public RoverController m_rover;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		ControllPlayer();
	}

	public void ControllRover()
	{
		if (m_rover.m_hasFlashlight)
		{
			m_playerChar.Deactivate();
			m_rover.Activate();
		}
	}

	public void ControllPlayer()
	{
		m_playerChar.Activate();
		m_rover.Deactivate();
	}
}

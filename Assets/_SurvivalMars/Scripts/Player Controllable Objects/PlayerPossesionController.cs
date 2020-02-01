using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossesionController : MonoBehaviour
{
	public static PlayerPossesionController Instance;

	public PlayerController m_playerChar;
	public RoverController m_rover;

	private bool change;

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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			change = !change;

			if (change)
			{
				ControllRover();
			}
			else
			{
				ControllPlayer();
			}
		}
	}

	public void ControllRover()
	{
		m_playerChar.Deactivate();
		m_rover.Activate();
	}

	public void ControllPlayer()
	{
		m_rover.Deactivate();
		m_playerChar.Activate();
	}
}

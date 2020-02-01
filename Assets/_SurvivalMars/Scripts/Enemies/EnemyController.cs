﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Wandering, Chase, Respawn }

    public EnemyState m_currentState;


    #region Respawn Values
    public float m_respawnTime;
    public Transform m_repsawnPosition;
    public GameObject m_enemyObject;

    private EnemyMovement_Base m_movementController;
    #endregion


    #region Detection Radius
    [Header("Detection Variables")]
    public float m_detectionTime;
    public float m_detectionTimer;
    private EnemyDetection m_enemyDetection;

    public float m_lostTime;
    private Coroutine m_lostCoroutine;

    private void Awake()
    {
        m_enemyDetection = GetComponentInChildren<EnemyDetection>();
        m_movementController = GetComponent<EnemyMovement_Base>();

    }
    private void Start()
    {
        SwitchState(EnemyState.Wandering);
    }

    #endregion

    private void Update()
    {
        CheckStates();
    }

    #region State Machine
    private void CheckStates()
    {
        switch (m_currentState)
        {
            case EnemyState.Wandering:

                m_movementController.IdleMovement();
                break;
            case EnemyState.Chase:
                if (m_enemyDetection.PlayerInRadius())
                {
                    print("Move To Player");
                    m_movementController.MoveToPlayer();
                }
                else
                {
                    m_movementController.MoveToLastKnownPosition();
                    m_lostCoroutine = StartCoroutine(LostTimer());
                }

                break;

            case EnemyState.Respawn:
                break;
        }
    }

    private void SwitchState(EnemyState p_newState)
    {
        m_currentState = p_newState;
        switch (p_newState)
        {
            case EnemyState.Wandering:
                ResetDetectionTime();
                m_enemyDetection.SetCollidersState(true);
                break;
            case EnemyState.Chase:
                m_enemyDetection.SetCollidersState(false);
                break;
            case EnemyState.Respawn:
                m_enemyObject.SetActive(false);
                StartCoroutine(RespawnTime());
                break;
        }
    }
    #endregion


    #region RespawnTime
    private IEnumerator RespawnTime()
    {
        yield return new WaitForSeconds(m_respawnTime);
        Respawn();

    }

    private void Respawn()
    {
        transform.position = m_repsawnPosition.position;
        m_enemyObject.SetActive(false);
    }

    public void Stunned()
    {
        m_enemyObject.SetActive(false);
        SwitchState(EnemyState.Respawn);
    }
    #endregion


    #region Detection
    public void CheckDetectionTime()
    {
        m_detectionTimer += Time.deltaTime;
        if (m_detectionTimer >= m_detectionTime)
        {
            SwitchState(EnemyState.Chase);
        }
    }

    public void ResetDetectionTime()
    {
        m_detectionTimer = 0;
    }
    #endregion

    private IEnumerator LostTimer()
    {
        yield return new WaitForSeconds(m_lostTime);
        SwitchState(EnemyState.Wandering);
    }

}

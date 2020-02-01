using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEvent: UnityEngine.Events.UnityEvent { }
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Wandering, Chase, Died,Respawn }

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

    [System.Serializable]
    public struct EnemyEvents
    {
        public EnemyEvent m_playerSpottedEvent;
        public EnemyEvent m_playerLostEvent;
        public EnemyEvent m_respawnEvent;
        public EnemyEvent m_diedEvent;
    }

    public EnemyEvents m_enemyEvents;
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
                m_enemyEvents.m_playerSpottedEvent.Invoke();
                m_enemyDetection.SetCollidersState(false);
                break;
            case EnemyState.Respawn:

                break;
            case EnemyState.Died:
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
        transform.rotation = m_repsawnPosition.rotation;
        m_enemyObject.SetActive(true);
        m_enemyEvents.m_respawnEvent.Invoke();
        SwitchState(EnemyState.Wandering);
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

    public void KillMe()
    {
        m_movementController.StopMovement();
        SwitchState(EnemyState.Died);
    }
}

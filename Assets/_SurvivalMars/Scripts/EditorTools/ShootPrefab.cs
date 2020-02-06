using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ShootPrefab : MonoBehaviour
{
    public GameObject m_shotObject;
    public List<GameObject> m_randomObjectMode;
    public bool m_shootRandomObjects;
    public float m_shootSpeed;
    public Transform m_fireSpot;
    public float m_fireSpotZOffset;
    public float m_fireSpotRange;

    public KeyCode m_increaseAmount, m_decreaseAmount;
    public int m_currentAmount = 1;



    [Header("Movement")]
    public float m_moveSpeed;
    public Transform m_cameraRotation;
    private Player m_playerInputController;
    public float m_mouseSensitivity;
    public bool m_inverted;
    public float m_maxCameraAng;
    public KeyCode m_moveUp, m_moveDown;
    public Transform m_viewCamera;

    [Header("debugging")]
    public bool m_debugging;
    public Color m_gizmosColor1;

    private void Start()
    {
        m_playerInputController = ReInput.players.GetPlayer(0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {

        CheckPlayerMovement();
        if (Input.GetKeyDown(m_increaseAmount))
        {
            m_currentAmount++;
        }else if (Input.GetKeyDown(m_decreaseAmount))
        {
            m_currentAmount--;
        }

        if (m_playerInputController.GetButtonDown("PickupCrystal"))
        {
            Shoot();
        }
    }

    private void CheckPlayerMovement()
    {
        Vector2 movementInput = new Vector2(m_playerInputController.GetAxis("MoveHorizontal"), m_playerInputController.GetAxis("MoveVertical"));
        CameraRotation(new Vector2(m_playerInputController.GetAxis("LookHorizontal"), m_playerInputController.GetAxis("LookVertical")));

        float upDown = 0;
        if (Input.GetKey(m_moveUp))
        {
            upDown = 1;
        }else if (Input.GetKey(m_moveDown))
        {
            upDown = -1;
        }

        transform.position += m_viewCamera.forward * movementInput.y * m_moveSpeed * Time.deltaTime;
        transform.position += m_viewCamera.right * movementInput.x * m_moveSpeed * Time.deltaTime;
        transform.position += m_viewCamera.up * upDown * m_moveSpeed * Time.deltaTime;


    }

    private void Shoot()
    {
        if (!m_shootRandomObjects)
        {
            for (int i = 0; i < m_currentAmount; i++)
            {
                Vector3 newPos = m_fireSpot.position + (m_fireSpot.forward * m_fireSpotZOffset) + Random.insideUnitSphere * m_fireSpotRange;
                GameObject newObject = ObjectPooler.instance.NewObject(m_shotObject, newPos, m_fireSpot.rotation);
                newObject.GetComponent<Rigidbody>().velocity = m_fireSpot.forward * m_shootSpeed;
                newObject.transform.parent = this.transform.parent;
            }
        }
        else
        {


            for (int i = 0; i < m_currentAmount; i++)
            {
                Vector3 newPos = m_fireSpot.position + (m_fireSpot.forward * m_fireSpotZOffset) + Random.insideUnitSphere * m_fireSpotRange;
                GameObject newObject = ObjectPooler.instance.NewObject(m_randomObjectMode[Random.Range(0, m_randomObjectMode.Count)], newPos, m_fireSpot.rotation);
                newObject.GetComponent<Rigidbody>().velocity = m_fireSpot.forward * m_shootSpeed;
                newObject.transform.parent = this.transform.parent;
            }
        }
        
    }



    private void CameraRotation(Vector3 p_looAmount)
    {
        //Get the inputs for the camera
        Vector2 cameraInput = new Vector2(p_looAmount.y * ((m_inverted) ? -1 : 1), p_looAmount.x);

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


    private void OnDrawGizmos()
    {
        if (!m_debugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawWireSphere((m_fireSpot.position + (m_fireSpot.forward * m_fireSpotZOffset)), m_fireSpotRange);
    }
}

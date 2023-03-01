using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2f, MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float zoomAmount;
    [SerializeField]
    private float zoomSpeed;


    [SerializeField]
    private CinemachineVirtualCamera CMVirtualCamera;

    private CinemachineTransposer transposer;
    Vector3 targetFollowOffset;
    // Start is called before the first frame update
    void Start()
    {
        transposer = CMVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = transposer.m_FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        HandleRotation();

        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.z += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.z += -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x += -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x += 1f;
        }

        moveDirection = moveDirection.z * transform.forward + moveDirection.x * transform.right;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y += 1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y += -1f;
        }

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            targetFollowOffset.y += -zoomAmount * Mathf.Sign(Input.mouseScrollDelta.y);
            targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);
        }

        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

}

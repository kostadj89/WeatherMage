#define USE_NEW_INPUT_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one InputManager!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector3 GetMousePosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;        
#endif
    }

    public bool GetMouseButtonDown()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal Vector2 GetCameraVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 moveDirection = new Vector3(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.y += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.y += -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x += -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x += 1f;
        }

        return moveDirection;
#endif
    }

    internal float GetCameraRotationAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotation.ReadValue<float>();
#else
        float rotationAmount = 0f;

        if (Input.GetKey(KeyCode.E))
        {
            rotationAmount += 1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotationAmount += -1f;
        }

        return rotationAmount;
#endif
    }

    internal float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        return Input.mouseScrollDelta.y;
#endif
    }
}

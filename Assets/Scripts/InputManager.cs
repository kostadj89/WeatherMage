using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one InputManager!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    public bool GetMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
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
    }

    internal float GetCameraRotationAmount()
    {
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
    }

    internal float GetCameraZoomAmount()
    {
        return Input.mouseScrollDelta.y;
    }
}

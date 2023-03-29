using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform actionCamera;
    [SerializeField]
    private float characterHeightMultiplier = 1.7f;
    [SerializeField]
    private float shoulderOffsetAmount = 1f;    
    [SerializeField]
    private Vector3 actionCameraHeight;

    [SerializeField]
    private Vector3 actionCameraOffset;
    
    [SerializeField]
    private float rightOffset =10f;

    [SerializeField]
    private float shootDirectionOffset = -10f ;
    [SerializeField]
    private float offsetMidCamera;
    [SerializeField]
    private float zoomDistance = 6f;

    // Start is called before the first frame update
    void Start()
    {
        BaseAction.OnAnyActionStart += OnAnyActionStart;
        BaseAction.OnAnyActionEnd += OnAnyActionEnd;

        HideActionCamera();
    }

    private void OnAnyActionEnd(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
            case AreaShootAction areaShootAction:
                HideActionCamera();
                break;
        }
    }

    private void OnAnyActionStart(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:

                SetActionCamera(shootAction.GetUnit(), shootAction.GetPotentionalTarget().GetWorldPosition());
                //SetActionCameraPositionLookAtTarget(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                //SetActionCameraPositionToMidPoint(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                //SetActionCameraPositionToUnitShoulder(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                ShowActionCamera();
                break;
            case AreaShootAction areaShootAction:
                SetActionCamera(areaShootAction.GetUnit(), LevelGrid.Instance.GetWorldFromGridPosition(areaShootAction.GetTargetGridPosition()));
                //SetActionCameraPositionLookAtTarget(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                //SetActionCameraPositionToMidPoint(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                //SetActionCameraPositionToUnitShoulder(shootAction.GetUnit(), shootAction.GetPotentionalTarget());
                ShowActionCamera();
                break;
        }
    }

    private void SetActionCameraPositionToUnitShoulder(Unit shooter, Unit targetUnit)
    {
        //setting up camera on the shoulder of the shooterUnit
        Vector3 cameraCharacterHeightPosition = Vector3.up * 1.7f;
        Vector3 shootDir = (targetUnit.GetWorldPosition() - shooter.GetWorldPosition()).normalized;
        
        Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir* shoulderOffsetAmount;

        Vector3 actionCameraPosition = shooter.GetWorldPosition() +
            cameraCharacterHeightPosition +
            shoulderOffset+
            shootDir*(-1);

        actionCamera.position = actionCameraPosition;
        actionCamera.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeightPosition);
    }

    private void SetActionCameraPositionToMidPoint(Unit shooter, Unit targetUnit)
    {
        Vector3 midPoint = (targetUnit.GetWorldPosition() + shooter.GetWorldPosition())/2f;

        Debug.Log(midPoint);

        float maxDistance = Mathf.Max(Vector3.Distance(midPoint, shooter.transform.position), Vector3.Distance(midPoint, targetUnit.transform.position));

        actionCamera.position = midPoint + new Vector3 (0,4, offsetMidCamera*maxDistance) ;
        actionCamera.LookAt(midPoint);
        //actionCamera.RotateAround(midPoint, Vector3.right, 45f);
    }

    private void SetActionCamera(Unit shooter, Vector3 targetWorldPos)
    {
        CinemachineVirtualCamera actionCMVC = actionCamera.GetComponent<CinemachineVirtualCamera>();

        // Get the midpoint between the two units
        Vector3 midpoint = (shooter.GetWorldPosition() + targetWorldPos) / 2.0f;

        // Set the camera's position to the midpoint, offset by the zoom distance + unit height 1.7f
        actionCamera.position = midpoint+ new Vector3(0f,1.7f,0f) - Camera.main.transform.forward * zoomDistance;

        // Calculate the distance between the two units
        float distance = Vector3.Distance(shooter.GetWorldPosition(), targetWorldPos);

        // Adjust the Virtual Camera's properties
        actionCMVC.m_Lens.FieldOfView = 60; // Set the field of view to a default value
        actionCMVC.transform.LookAt(midpoint); // Look at the midpoint between the two units
        actionCMVC.m_Lens.NearClipPlane = 0.1f; // Set the near clip plane to prevent clipping
        actionCMVC.m_Lens.FarClipPlane = distance + zoomDistance+1000; // Set the far clip plane to include both units

        // Zoom the camera to frame both units
        Bounds bounds = new Bounds(midpoint, Vector3.zero);
        bounds.Encapsulate(shooter.GetWorldPosition());
        bounds.Encapsulate(targetWorldPos);
        actionCMVC.m_Lens.OrthographicSize = bounds.extents.magnitude;
    }

    private void SetActionCameraPositionLookAtTarget(Unit shooter, Unit targetUnit)
    {
        Vector3 shootDir = (targetUnit.GetWorldPosition() - shooter.GetWorldPosition()).normalized;
        Vector3 cameraPosition = shooter.GetWorldPosition() + new Vector3(0, 1.7f, 0);
        //Vector3 cameraPosition = shooter.GetWorldPosition() + (new Vector3(0, 1.7f, 0) + shooter.transform.right*rightOffset)+ shootDirectionOffset*shootDir;
        //Vector3 cameraPosition = shooter.GetWorldPosition() + Vector3.right*(Vector3.Distance(shooter.GetWorldPosition(), targetGridPosition.GetWorldPosition()))+ new Vector3(0,6f,0);
        actionCamera.position = cameraPosition;
        actionCamera.Translate(Vector3.right * 10, Space.Self);
        actionCamera.LookAt(targetUnit.GetWorldPosition()+new Vector3(0,1.7f,0));
        //actionCamera.Translate(Vector3.right*10, Space.Self);
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowActionCamera()
    {
        actionCamera.gameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCamera.gameObject.SetActive(false);
       
    }
}

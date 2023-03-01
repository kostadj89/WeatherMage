using Assets.Scripts.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAction : BaseAction//, IAction
{
    //protected CompleteActionDelegate completeAction;

    private const float stopDistanceTreshold = 0.1f;
    private Vector3 targetPosition;

    

    [SerializeField]
    private Animator unitAnimator;

    [SerializeField]
    private int maxMoveRadius = 2 ;
    private bool isBusy;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        unitAnimator.SetBool("IsRunning", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) { return; }

        if (Vector3.Distance(transform.position, targetPosition) > stopDistanceTreshold)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;

            transform.position += moveDirection * Time.deltaTime * moveSpeed;

            float forwardRotateSpeed = 10;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * forwardRotateSpeed);
        }
        else if (unitAnimator.GetBool("IsRunning"))
        {            
            //set animation param for walking to false
            unitAnimator.SetBool("IsRunning", false);
            isActive = false;
            //completeAction();
            onActionComplete();
        }
    }
    public override string GetActionName()
    {
        return "MOVE";
    }

    public override void TakeAction(Action completeActionDelegate, GridPosition targetPosition)
    {
        onActionComplete = completeActionDelegate;
        //set animation param for walking to true 
        isActive = true;
        unitAnimator.SetBool("IsRunning", true);
        this.targetPosition = LevelGrid.Instance.GetWorldFromGridPosition( targetPosition);
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {     
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        GridPosition offset;

        GridPosition testGridPos;

        for (int i = -maxMoveRadius; i <= maxMoveRadius; i++)
        {
            for (int j = -maxMoveRadius; j <= maxMoveRadius; j++)
            {
                offset = new GridPosition(i, j);
                testGridPos = offset + unitGridPosition;                

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPos) || LevelGrid.Instance.IsGridPositionOccupied(testGridPos))
                {
                    continue;
                } 

                validGridPositions.Add(testGridPos);
                //Debug.Log(testGridPos);
            }
        }

        return validGridPositions;
    }

    //public override int GetActionCost()
    //{
    //    return base.GetActionCost();
    //}

    //public void SetBusy()
    //{
    //    isBusy = true;
    //}

    //public void ClearBusy()
    //{
    //    isBusy = false;
    //}

    public bool isExecuting()
    {
        return isActive;
    }
}

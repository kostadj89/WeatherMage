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

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

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
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionEnd();            
        }
    }
    public override string GetActionName()
    {
        return "MOVE";
    }

    public override void TakeAction(Action completeActionDelegate, GridPosition targetPosition)
    {
        OnStartMoving?.Invoke(this, EventArgs.Empty);

        this.targetPosition = LevelGrid.Instance.GetWorldFromGridPosition( targetPosition);
        ActionStart(completeActionDelegate);
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

    #region AI
    
    public override ScoredEnemyAIAction GetScoredEnemyAIActionOnGridPosition(GridPosition gridPos)
    {
        int targetCountFromGridPosition = unit.GetAction<ShootAction>().GetNumberOfTargetsFromPosition(gridPos);
        return new ScoredEnemyAIAction { gridPosition = gridPos, actionValue = 10 + targetCountFromGridPosition * 15 };
    } 

    #endregion
}

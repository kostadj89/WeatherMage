using Assets.Scripts.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MoveAction : BaseAction//, IAction
{
    //protected CompleteActionDelegate completeAction;

    private const float stopDistanceTreshold = 0.1f;
    private List<Vector3> targetPositionPath;
    private int currentPositionIndex = 0;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField]
    private int maxMoveRadius = 2 ;
    private bool isBusy;

    protected override void Awake()
    {
        base.Awake();
        //targetPosition = transform.position;

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) { return; }

        Vector3 targetPosition = targetPositionPath[currentPositionIndex];

        if (Vector3.Distance(transform.position, targetPosition) > stopDistanceTreshold)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 4f;

            transform.position += moveDirection * Time.deltaTime * moveSpeed;

            float forwardRotateSpeed = 3;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * forwardRotateSpeed);
        }
        else
        {
            //go to the next grid cell in path
            currentPositionIndex++;

            //if it's the last one end action
            if(currentPositionIndex >= targetPositionPath.Count) 
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionEnd();
            }

        }
    }
    public override string GetActionName()
    {
        return "MOVE";
    }

    public override void TakeAction(Action completeActionDelegate, GridPosition targetPosition)
    {
        OnStartMoving?.Invoke(this, EventArgs.Empty);

        this.targetPositionPath = (PathfindingSquareGrid.Instance.FindAPath(unit.GetGridPosition(), targetPosition, out int pathDistance)
            .Select(x=> LevelGrid.Instance.GetWorldFromGridPosition(x))).ToList();

        this.currentPositionIndex= 0;
        //this.targetPositionPath =  new List<Vector3> { LevelGrid.Instance.GetWorldFromGridPosition(targetPosition) };
        ActionStart(completeActionDelegate);
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {     
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        GridPosition offset;

        GridPosition testGridPos;

        List<GridPosition> unvalidatedGridPositions = LevelGrid.Instance.GetAllCellsInTheRange(unitGridPosition, maxMoveRadius);

        foreach (GridPosition item in unvalidatedGridPositions)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(item)
                   || LevelGrid.Instance.IsGridPositionOccupied(item)
                   || !PathfindingSquareGrid.Instance.IsGridPositionWalkable(item)
                   || !PathfindingSquareGrid.Instance.HasAPath(unitGridPosition, item)
                   || PathfindingSquareGrid.Instance.GetPathDistanceCost(unitGridPosition, item) > maxMoveRadius * 15)
            {
                continue;
            }

            validGridPositions.Add(item);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStart;
    public static event EventHandler OnAnyActionEnd;
    protected Unit unit;
    protected bool isActive;

    protected Action onActionComplete;

    //public delegate void CompleteActionDelegate();

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();    
    public abstract void TakeAction(Action OnCompleteAction, GridPosition gridPosition);

    public virtual bool IsValidGridPositionForAction(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetAllValidGridPositionsForAction();
        return validGridPositions.Contains(gridPosition);
    }

    public virtual int GetActionCost()
    {
        return 1;
    }

    protected void ActionStart(Action onCompleteAction)
    {
        this.onActionComplete = onCompleteAction;
        isActive = true;

        OnAnyActionStart?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionEnd()
    {
        isActive = false;
        this.onActionComplete();
        OnAnyActionEnd?.Invoke(this, EventArgs.Empty);
    }

    public abstract List<GridPosition> GetAllValidGridPositionsForAction();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsActive()
    {
        return isActive;
    }

    #region AI

    public ScoredEnemyAIAction GetBestScoreAndPosForAction()
    {
        List<ScoredEnemyAIAction> enemyAIActions = new List<ScoredEnemyAIAction>();

        List<GridPosition> validaACtionGridPositions = GetAllValidGridPositionsForAction();

        //for the specific action we iterate through all valid grid positions
        foreach (GridPosition gridPos in validaACtionGridPositions)
        {
            ScoredEnemyAIAction enemyAIAction = GetScoredEnemyAIActionOnGridPosition(gridPos);
            enemyAIActions.Add(enemyAIAction);
        }

        if (enemyAIActions.Count > 0)
        {
            //sorting by action value
            enemyAIActions.Sort((ScoredEnemyAIAction a, ScoredEnemyAIAction b) => b.actionValue - a.actionValue);

            return enemyAIActions[0];
        }
        else
        {
            return null;
        }
        
    }

    public abstract ScoredEnemyAIAction GetScoredEnemyAIActionOnGridPosition(GridPosition gridPos);

    #endregion AI
}

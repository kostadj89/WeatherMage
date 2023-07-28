using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAction : BaseAction
{
    private const float BEGIN_TIME = 1.1f;
    private const float SWINGING_TIME = 1.1f;
    private const float COOLOFF_TIME = 0.5f;

    [SerializeField]
    private int maxSwordRadius =1;
    [SerializeField]
    private int meleeDamage = 1;


    private ICanTakeDamage potentionalTarget;
    private float actionStateTimer;
    private ActionState state;

    //animation params
    private bool canTakeASwing = false;
    private float forwardRotateSpeed = 10;

    //stuff regarding showing weapon, i was thinking of adding item visuals via abilities/weapon skills. so these params would control if the weapon is shown as equipped or if it's materializing
    //probs shouldn't be here but i'll change it later
    [SerializeField]
    private bool isEquippedAsItem = false;
    [SerializeField]
    private bool isMaterializing = true;
    [SerializeField]
    private GameObject itemVisual;

    private enum ActionState
    {
        None,
        BeforeTheSwing,
        AfterTheSwing
    }

    #region overrides
    public override string GetActionName()
    {
        return "Melee";
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        List<GridPosition> unvalidatedGridPositions = new List<GridPosition>();

        GridPosition originGridPosition = unit.GetGridPosition();
        GridPosition offsetGridPosition;

        //GridPosition testGridPos;

        unvalidatedGridPositions = LevelGrid.Instance.GetAllCellsInTheRange(originGridPosition, 1);
        foreach (GridPosition testGridPos in unvalidatedGridPositions)
        {
            // check to see if the cell is out of grid bounds, if yes ignore
            if (!LevelGrid.Instance.IsValidGridPosition(testGridPos))
            {
                continue;
            }

            //check to see if the cell has any unit or destructible on it, if no ignore
            if (!LevelGrid.Instance.IsGridPositionOccupied(testGridPos))
            {
                continue;
            }

            //check to see if the unit is on the same team as the playing unit, if yes ignore
            ICanTakeDamage targetUnitOrDestructible = LevelGrid.Instance.GetUnitOrDestructibleAtGridPosition(testGridPos);

            if (targetUnitOrDestructible.IsOnSameTeam(unit.IsEnemy()))
            {
                continue;
            }


            //in the end, we got an opossing unit inside of the range on the valid grid cell, and we add that unit's grid cell
            validGridPositions.Add(testGridPos);
            //Debug.Log(testGridPos);
        }

        return validGridPositions;
    }

    public override ScoredEnemyAIAction GetScoredEnemyAIActionOnGridPosition(GridPosition gridPos)
    {
        return new ScoredEnemyAIAction { gridPosition = gridPos, actionValue = 170 };
    }

    public override void TakeAction(Action OnCompleteAction, GridPosition gridPosition)
    {
        potentionalTarget = LevelGrid.Instance.GetUnitOrDestructibleAtGridPosition(gridPosition);
        canTakeASwing = true;       
        ActionStart(OnCompleteAction);
    }

    #endregion

    public event EventHandler OnStartMelee;   

    // Start is called before the first frame update
    void Start()
    {
        if (itemVisual != null) 
        { 
            SetStartingItemVisibility();
        }
    }

    private void SetStartingItemVisibility()
    {
        itemVisual.SetActive(isEquippedAsItem && !isMaterializing);
    }

    private void SetItemVisibility(bool visibility)
    {
        if (itemVisual != null)
        {
            itemVisual.SetActive(visibility);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        actionStateTimer -= Time.deltaTime;

        switch (state)
        {
            case ActionState.None: 
                break;
            case ActionState.BeforeTheSwing:

                //rotating                
                Vector3 aimDirection = (potentionalTarget.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * forwardRotateSpeed);

                break;
            case ActionState.AfterTheSwing:

                if (canTakeASwing)
                {
                    TakeASwing();
                    canTakeASwing = false;
                }

                break;
        }

        //state switch
        if (actionStateTimer <= 0)
        {
            ChangeActionState();
        }
    }

    private void TakeASwing()
    {
        potentionalTarget.TakeDamage(meleeDamage);
       
    }

    private void ChangeActionState()
    {
        switch (state)
        {
            case ActionState.None:
                
                //make item visible
                SetItemVisibility(isMaterializing);

                state = ActionState.BeforeTheSwing;
                OnStartMelee?.Invoke(this, EventArgs.Empty);
                actionStateTimer = SWINGING_TIME;
                Debug.Log("Swinging");
                break;
            case ActionState.BeforeTheSwing:
                state = ActionState.AfterTheSwing;
                actionStateTimer = COOLOFF_TIME;
                break;
            case ActionState.AfterTheSwing:
                state = ActionState.None;

                SetItemVisibility(!isMaterializing);
                ActionEnd();

                break;
        }
    }

    #region publics

    public Unit GetUnit()
    {
        return unit;
    }

    public int GetRadius()
    {
        return maxSwordRadius;
    }

    public ICanTakeDamage GetPotentionalTarget()
    {
        return potentionalTarget;
    }

    #endregion publics
}

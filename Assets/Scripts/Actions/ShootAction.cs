using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    private const float BEGIN_TIME = 1.1f;
    private const float SHOOTING_TIME = 0.1f;
    private const float COOLOFF_TIME = 0.5f;
   
    private enum ActionState
    {
        Aiming,
        Shooting,
        Cooloff
    }

    private bool canTakeAShot = false;
    private float forwardRotateSpeed = 10;

    private ActionState state = ActionState.Aiming;
    private float actionStateTimer = BEGIN_TIME;
    
    private Unit targetUnit;

    [SerializeField]
    private int maxShootRadius = 7;
    [SerializeField]
    private int damage = 35;

    //custom event args class defined below with attacker and target unit infos
    public event EventHandler OnStartShooting;
    public event EventHandler<OnShootEventArgs> OnFire;
    public class OnShootEventArgs : EventArgs
    {
        public Unit shootingUnit;
        public Unit targetUnit;
        public int damage;

        public OnShootEventArgs(Unit shootingUnit, Unit targetUnit, int damage)
        {
            this.shootingUnit = shootingUnit;
            this.targetUnit = targetUnit;
            this.damage = damage;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        GridPosition offsetGridPosition;

        GridPosition testGridPos;

        for (int i = -maxShootRadius; i <= maxShootRadius; i++)
        {
            for (int j = -maxShootRadius; j <= maxShootRadius; j++)
            {
                offsetGridPosition = new GridPosition(i, j);
                testGridPos = offsetGridPosition + unitGridPosition;

                // check to see if the cell is out of grid bounds, if yes ignore
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPos))
                {
                    continue;
                }

                //check to see if the cell is outside of the shooting distance
                int testDistance = Mathf.Abs(i) + Mathf.Abs(j);
                if (testDistance > maxShootRadius) continue;

                //check to see if the cell has any unit on it, if no ignore
                if (!LevelGrid.Instance.IsGridPositionOccupied(testGridPos))
                {
                    continue;
                }

                //check to see if the unit is on the same team as the playing unit, if yes ignore
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPos);

                //check to see if both units are on the same team
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                //in the end, we got an opossing unit inside of the range on the valid grid cell, and we add that unit's grid cell
                validGridPositions.Add(testGridPos);
                //Debug.Log(testGridPos);
            }
        }

        return validGridPositions;
    }

    public override void TakeAction(Action OnCompleteAction, GridPosition gridPosition)
    {
        //unit we shoot at
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        canTakeAShot = true;
        //unit enters aiming state
        Debug.Log("Aiming");

        ActionStart(OnCompleteAction);
        OnStartShooting?.Invoke(this,EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) { return; }

        actionStateTimer -= Time.deltaTime;

        switch (state)
        {
            case ActionState.Aiming:
                
                //rotating
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * forwardRotateSpeed);

                break;
            case ActionState.Shooting:
                
                if (canTakeAShot)
                {
                    TakeAShot();
                    canTakeAShot = false;
                }

                break;
            case ActionState.Cooloff:
                break;
        }

        //state switch
        if (actionStateTimer <= 0)
        {
            ChangeActionState();
        }
    }

    private void ChangeActionState()
    {
        switch (state)
        {
            case ActionState.Aiming:
                
                state = ActionState.Shooting;
                actionStateTimer = SHOOTING_TIME;
                Debug.Log("Shooting");

                break;
            case ActionState.Shooting:

                state = ActionState.Cooloff;
                actionStateTimer = COOLOFF_TIME;
                Debug.Log("Cooloff");

                break;
            case ActionState.Cooloff:

                state = ActionState.Aiming;
                actionStateTimer = BEGIN_TIME;

                ActionEnd();

                break;
        }
    }

    private void TakeAShot()
    {
        OnFire?.Invoke(this, new OnShootEventArgs(unit, targetUnit, damage));
    }
}

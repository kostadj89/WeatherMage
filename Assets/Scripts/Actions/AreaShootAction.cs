using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CustomProjectile;

public class AreaShootAction : BaseAction
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

    //private Unit targetUnit;
    private GridPosition targetGridPosition;

    [SerializeField]
    private int maxShootRadius = 7;
    [SerializeField]
    private int damage = 35;
    [SerializeField]
    private int explosionRadius = 1;
    [SerializeField]
    private LayerMask obstacleLayerMask;

    //projectlie to spawn
    [SerializeField]
    private Transform spellboltProjectilePrefab;
    //practically a location of the bone 
    [SerializeField]
    private Transform projectileSpawnPoint;

    //transforme of the spawned projectile
    private Transform spellboltTransform;

    //custom event args class defined below with attacker and target unit infos
    public event EventHandler OnStartShooting;
    public event EventHandler<OnAreaShootEventArgs> OnProjectileFire;
    public class OnAreaShootEventArgs : EventArgs
    {
        public Unit shootingUnit;
        public GridPosition targetedGridPosition;
        public int damage;

        public OnAreaShootEventArgs(Unit shootingUnit, GridPosition target, int damage)
        {
            this.shootingUnit = shootingUnit;
            this.targetedGridPosition = target;
            this.damage = damage;
        }
    }

    private void OnProjectileDestroyed_ShootAction(object sender, OnProjectileDestroyedArgs e)
    {
        List<GridPosition> gridPositions = LevelGrid.Instance.GetAllCellsInTheRange(e.targetPosition, explosionRadius);
        List<Unit> listOfAdjacent = LevelGrid.Instance.GetAllEnemiesFromTheCells(gridPositions);

        foreach (Unit adjacentUnit in listOfAdjacent)
        {
            adjacentUnit.TakeDamage(e.damage);
        }

        ActionEnd();
    }

    public override string GetActionName()
    {
        return "AreaShoot";
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        return GetAllValidGridPositionsForAction(unit.GetGridPosition());
    }
    private  List<GridPosition> GetAllValidGridPositionsForAction(GridPosition originGridPosition)
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        //GridPosition originGridPosition = unit.GetGridPosition();
        GridPosition offsetGridPosition;

        GridPosition testGridPos;

        for (int i = -maxShootRadius; i <= maxShootRadius; i++)
        {
            for (int j = -maxShootRadius; j <= maxShootRadius; j++)
            {
                offsetGridPosition = new GridPosition(i, j);
                testGridPos = offsetGridPosition + originGridPosition;

                // check to see if the cell is out of grid bounds, if yes ignore
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPos))
                {
                    continue;
                }

                //check to see if the cell is outside of the shooting distance
                int testDistance = Mathf.Abs(i) + Mathf.Abs(j);
                if (testDistance > maxShootRadius) continue;

                //check to see if the cell has any unit on it, if no ignore
                //if (!LevelGrid.Instance.IsGridPositionOccupied(testGridPos))
                //{
                //    continue;
                //}

                //check to see if the unit is on the same team as the playing unit, if yes ignore
                //Unit targetGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(testGridPos);

                //check to see if both units are on the same team
                //if (targetGridPosition.IsEnemy() == unit.IsEnemy())
                //{
                //    continue;
                //}

                //if there are obcastles ignore
                if(Physics.Raycast(
                    LevelGrid.Instance.GetWorldFromGridPosition(originGridPosition) + Vector3.up * 1.7f,
                    (LevelGrid.Instance.GetWorldFromGridPosition(testGridPos)-unit.GetWorldPosition()).normalized,
                    Vector3.Distance(LevelGrid.Instance.GetWorldFromGridPosition(testGridPos), unit.GetWorldPosition()),
                    obstacleLayerMask))
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
        //targetGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        targetGridPosition = gridPosition;

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
                Vector3 aimDirection = (LevelGrid.Instance.GetWorldFromGridPosition( targetGridPosition) - unit.GetWorldPosition()).normalized;
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
                //ActionEnd();

                break;
        }
    }

    private void TakeAShot()
    {
        //OnFireProjectile?.Invoke(this, new OnShootEventArgs(unit, targetGridPosition, damage));

        spellboltTransform = Instantiate(spellboltProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        Vector3 targetProjectilePos = LevelGrid.Instance.GetWorldFromGridPosition(targetGridPosition);

        //set projectile height to be the same as the spawn height
        targetProjectilePos.y += projectileSpawnPoint.position.y;

        CustomProjectile sp = spellboltTransform.GetComponent<CustomProjectile>();
        sp.Setup(targetProjectilePos);
        //listen for when it is destroyed
        sp.SetDamage(damage);
        sp.OnProjectileDestroyed += OnProjectileDestroyed_ShootAction;        
    }

    public GridPosition GetTargetGridPosition()
    { 
        return targetGridPosition; 
    }

    public Unit GetUnit()
    {
        return unit;
    }
    public int GetDamage()
    {
        return damage;
    }

    public int GetRadius()
    {
        return maxShootRadius;
    }
    public int GetNumberOfTargetsFromPosition(GridPosition gridPosition)
    {
        return GetAllValidGridPositionsForAction(gridPosition).Count;
    }

    #region AI

    public override ScoredEnemyAIAction GetScoredEnemyAIActionOnGridPosition(GridPosition gridPos)
    {
        Unit potentialEnemy = LevelGrid.Instance.GetUnitAtGridPosition(gridPos);
        return new ScoredEnemyAIAction { gridPosition = gridPos, actionValue = 100 + Mathf.RoundToInt((1f- potentialEnemy.GetCurrentHealthPercentage())*10) };
    }

    

    #endregion
}

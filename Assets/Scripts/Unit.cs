using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    //private Vector3 targetPosition;
    private GridPosition currentGridPosition;
    //Actions
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] unitActions;
    private int currentActionPoints;
    private HealthSystem healthSystem;
    
    [SerializeField]
    private int maxActionPoints = 3;
    [SerializeField]
    private bool isEnemy;   

    private void Awake()
    {
        //targetPosition=transform.position;
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        unitActions = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        
    }

    private void UpdateUnitOnTurnEnded(object sender, int e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            ResetActionPoints();
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }        
    }
    
    private void ResetActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    // Start is called before the first frame update
    void Start() 
    {
        currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(currentGridPosition, this);

        //set available actionPoints
        currentActionPoints = maxActionPoints;

        //add end turn listener
        TurnSystem.Instance.OnTurnEnded += UpdateUnitOnTurnEnded;
        healthSystem.OnDying += Die;

        OnAnyUnitSpawned?.Invoke(this,EventArgs.Empty);
    }

    private void Die(object sender, EventArgs e)
    {
        LevelGrid.Instance.ClearUnitAtGridPosition(currentGridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this,EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {        
        //checking grid position
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        if (newGridPosition != currentGridPosition)
        {
            GridPosition oldGridPosition = currentGridPosition;
            currentGridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);            
        }
    } 
    
    //publics 
    public MoveAction GetMoveAction() { return moveAction; }
    public SpinAction GetSpinAction() { return spinAction; }

    public GridPosition GetGridPosition() { return currentGridPosition; }

    public BaseAction[] GetAllUnitActions() { return unitActions; }

    public bool CanSpendActionPointsToTakeAction(BaseAction action)
    {
        return currentActionPoints >= action.GetActionCost();       
    }

    public void SpendActionPoints(int points)
    {
        currentActionPoints -= points;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionCost());
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    internal int GetActionPoints()
    {
        return currentActionPoints;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    internal void TakeDamage(int damage)
    {
        Debug.Log(this + " has been shot!");
        healthSystem.TakeDamage(damage);
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnEnded -= UpdateUnitOnTurnEnded;
    }
}

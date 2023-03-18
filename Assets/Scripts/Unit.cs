using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    //private Vector3 targetPosition;
    private GridPosition currentGridPosition;

    //Actions   
    private BaseAction[] unitActions;
    private int currentActionPoints;
    [SerializeField]
    private int maxActionPoints = 3;

    //Health
    private HealthSystem healthSystem;   
   
    //Alligence 
    [SerializeField]
    private bool isEnemy;   

    private void Awake()
    {       
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
        OnAnyUnitDead?.Invoke(this,EventArgs.Empty);
        Destroy(gameObject);
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
    public T GetAction<T>() where T:BaseAction
    {
        foreach (BaseAction item in unitActions)
        {
            if (item is T)
            {
                return (T)item;
            }
        }

        return null;
    }

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

    public float GetCurrentHealthPercentage()
    {
        return healthSystem.GetNormalizedHealth();
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

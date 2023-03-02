using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;

    //private Vector3 targetPosition;
    private GridPosition currentGridPosition;
    //Actions
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] unitActions;
    private int currentActionPoints;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        //if (Vector3.Distance(transform.position, targetPosition) > stopDistanceTreshold)
        //{
        //    Vector3 moveDirection = (targetPosition - transform.position).normalized;
        //    float moveSpeed = 4f;            
            
        //    transform.position += moveDirection * Time.deltaTime * moveSpeed;

        //    float forwardRotateSpeed = 10;
        //    transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime*forwardRotateSpeed);
        //}
        //else if (unitAnimator.GetBool("IsRunning"))
        //{
        //    //set animation param for walking to false
        //    unitAnimator.SetBool("IsRunning", false);
        //}
        
        //checking grid position
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        if (newGridPosition != currentGridPosition)
        {
            LevelGrid.Instance.UnitMovedGridPosition(this, currentGridPosition, newGridPosition);
            currentGridPosition = newGridPosition;
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

    internal void TakeDamage()
    {
        Debug.Log(this + " has been shot!");
    }
}

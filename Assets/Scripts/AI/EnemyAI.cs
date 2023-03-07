using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //Enemy states
    private enum EnemyState
    {
        Waiting,
        TakingTurn,
        Busy
    }

    const float ENEMY_TURN_LENGTH_IN_S = 3f;

    private EnemyState state;
    private float timer;

    private void Awake()
    {
        state= EnemyState.Waiting;
    }
    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.Instance.OnTurnEnded += OnTurnEnded;
        timer = ENEMY_TURN_LENGTH_IN_S;
    }

    private void OnTurnEnded(object sender, int e)
    {
        if(TurnSystem.Instance.IsPlayerTurn())
        {
            state=EnemyState.TakingTurn;
            timer = ENEMY_TURN_LENGTH_IN_S;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch(state)
        {
            case EnemyState.Waiting:
                break;
            case EnemyState.TakingTurn:

                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    state = EnemyState.Busy;
                    
                    if(TryTakeEnemyAIAction(SetTakingTurn))
                    {
                        state = EnemyState.Busy;
                    }
                    else
                    {
                        //No enemy had any more actions to take
                        TurnSystem.Instance.AdvanceTurn();
                    }                    
                }

                break; 
            case EnemyState.Busy:
                break;
        }        
    }

    private void SetTakingTurn()
    {
        timer = ENEMY_TURN_LENGTH_IN_S;
        state = EnemyState.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action OnEnemyAIActionComplete)
    {
        Debug.Log("going through enemy list and taking actions");

        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnits())
        {
            if(TryTakeUnitEnemyAIAction(enemyUnit, OnEnemyAIActionComplete))
            {
                return true;
            }
            
        }

        return false;
    }

    private bool TryTakeUnitEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
       //temp spin action
       SpinAction spinAction = enemyUnit.GetSpinAction();

        //trying to run spin on enemy unit
        GridPosition enemyGridPosition = enemyUnit.GetGridPosition();

        if (spinAction.IsValidGridPositionForAction(enemyGridPosition))
        {
            if (enemyUnit.TrySpendPointsToTakeAction(spinAction))
            {
                Debug.Log("Enemy spins!");
                spinAction.TakeAction(onEnemyAIActionComplete, enemyGridPosition);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}

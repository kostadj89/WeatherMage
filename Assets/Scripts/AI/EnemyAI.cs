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
        BaseAction bestEnemyAction= null;
        ScoredEnemyAIAction bestScoredEnemyAIAction = null;
        BaseAction[] allEnemyActions = enemyUnit.GetAllUnitActions();
        
        //going through all available enemy actions
        foreach (BaseAction enemyAction in allEnemyActions)
        {
            //if the action is too expensive skip that action, enemy doesn't have that many action points
            if (!enemyUnit.CanSpendActionPointsToTakeAction(enemyAction))
            {
                continue;
            }

            //if it's first action set it as best
            if (bestEnemyAction == null)
            {
                bestEnemyAction = enemyAction;
                //of all posible actions of type enemyAction action type, do the one with best score
                bestScoredEnemyAIAction = enemyAction.GetBestScoreAndPosForAction();
            }
            else
            {
                ScoredEnemyAIAction tempScoredAction = enemyAction.GetBestScoreAndPosForAction();

                if (tempScoredAction != null && tempScoredAction.actionValue > bestScoredEnemyAIAction.actionValue)
                { 
                    bestScoredEnemyAIAction = tempScoredAction;
                    bestEnemyAction = enemyAction;
                }
            }
        }

        //TrySpendPointsToTakeAction this actually spends points for action
        if (bestEnemyAction != null && enemyUnit.TrySpendPointsToTakeAction(bestEnemyAction))
        {
            bestEnemyAction.TakeAction(onEnemyAIActionComplete,bestScoredEnemyAIAction.gridPosition);
            return true;
        }
        else
        {
            return false;
        }        
    }
}

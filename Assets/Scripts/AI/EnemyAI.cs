using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    const float ENEMY_TURN_LENGTH_IN_S = 3f;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.Instance.OnTurnEnded += OnTurnEnded;
        timer = ENEMY_TURN_LENGTH_IN_S;
    }

    private void OnTurnEnded(object sender, int e)
    {
        timer = ENEMY_TURN_LENGTH_IN_S;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            TurnSystem.Instance.AdvanceTurn();            
        }
    }
}

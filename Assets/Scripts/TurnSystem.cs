using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private int turnNumber = 0;
    private bool isPlayerTurn;

    public static TurnSystem Instance;
    public event EventHandler<int> OnTurnEnded;
    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.Log("More than one TurnSystem object are being instantiated!");
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        turnNumber++;
        isPlayerTurn= true;
        OnTurnEnded?.Invoke(this, turnNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;
        OnTurnEnded?.Invoke(this, turnNumber);        
    }

    public int GetCurrentTurn()
    { return turnNumber; }

    public bool IsPlayerTurn() { return isPlayerTurn;}
}

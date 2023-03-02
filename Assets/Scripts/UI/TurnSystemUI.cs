using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private Button endTurnButton;
    [SerializeField]
    private GameObject enemyTurnObjectUI;
    private void Awake()
    {
        
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(EndTurnClicked);
    }
    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.Instance.OnTurnEnded += UpdateTurnUIOnTurnEnded;
        
        UpdateEnemyTurnVisual();
        UpdateEndTurnBttnVisibility();
    }

    private void UpdateTurnUIOnTurnEnded(object sender, int turnIndex)
    {
        UpdateTextUI(turnIndex);
        UpdateEnemyTurnVisual();
        UpdateEndTurnBttnVisibility();
    }

    private void UpdateTextUI(int turnIndex)
    {
        turnText.text = string.Format("Turn: {0}", turnIndex);
    }

    private void EndTurnClicked()
    {
        TurnSystem.Instance.AdvanceTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnObjectUI.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnBttnVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}

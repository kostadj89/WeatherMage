using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField]
    private Transform ActionButtonPrefab;

    [SerializeField]
    private Transform ActionButtonContainer;

    [SerializeField]
    private TextMeshProUGUI ActionPointsText;

    List<ActionButtonUI> ActionButtons;

    private void Awake()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += ChangeActionsOnUnitSelection;
        UnitActionSystem.Instance.OnSelectedActionChanged += ChangeSelectedActionVisual;
        UnitActionSystem.Instance.OnActionStarted += UpdateActionPointsUIOnActionStarted;
        TurnSystem.Instance.OnTurnEnded += UpdateActionPointsUIOnTurnEnd;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        ActionButtons = new List<ActionButtonUI>();
    }

    private void UpdateActionPointsUIOnTurnEnd(object sender, int e)
    {
        UpdateActionPointsUIText();
    }
    

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsUIText();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetAllActionButtons();
        UpdateSelectedActionVisual();
        UpdateActionPointsUIText();
    }

    //updates action buttons to show selected
    private void UpdateSelectedActionVisual()
    {
        foreach (ActionButtonUI button in ActionButtons) 
        {
            button.UpdateSelectedVisual();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //get all buttons based on baseAction components
    public void GetAllActionButtons()
    {
        DestroyAllButtons();        

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction[] unitActions = selectedUnit.GetAllUnitActions();

        Transform buttonTransform;

        foreach (BaseAction action in unitActions)
        {
            buttonTransform = Instantiate(ActionButtonPrefab, ActionButtonContainer);
            ActionButtonUI buttonUI = buttonTransform.GetComponent<ActionButtonUI>();
            buttonUI.SetBaseAction(action);
            ActionButtons.Add(buttonUI);
            //buttonTransform.GetComponent<TextMeshPro>().text = action.GetActionName();
        } 
    }

    private void DestroyAllButtons()
    {
        foreach (Transform bttn in ActionButtonContainer)
        {
            Destroy(bttn.gameObject);
        }

        ActionButtons.Clear();
    }

    private void ChangeActionsOnUnitSelection(object sender, EventArgs empty)
    {
        GetAllActionButtons();
        UpdateActionPointsUIText();
    }


    //on every action selection
    private void ChangeSelectedActionVisual(object sender, EventArgs empty)
    {
        UpdateSelectedActionVisual();
    }

    private void UpdateActionPointsUIOnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPointsUIText();
    }

    //updates text with the correct number of action points
    private void UpdateActionPointsUIText()
    {
        ActionPointsText.text = string.Format("Actions: {0}", UnitActionSystem.Instance.GetSelectedUnit().GetActionPoints());
    }
}

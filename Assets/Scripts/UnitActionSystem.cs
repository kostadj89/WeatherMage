using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//it was important to run this script before others, so it was set to run before the "Default Time"  in the ScriptExecutionOredr in Project settings
public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance  { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    //public event EventHandler OnActionBusy;
    //public event EventHandler OnActionFree;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;


    [SerializeField] private Unit  selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;

    private bool isBusy;

    //Use Awake only for initializing
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one UnitActionSystem!");
            Destroy(gameObject);
            return;
        }

        Instance= this;
    }
    // Start is called before the first frame update, use only for external refs handling
    void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    // Update is called once per frame
    void Update()
    {
        //if any other action is running do nothing
        if (isBusy) { return; }

        if (!TurnSystem.Instance.IsPlayerTurn()) { return; }

        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (Input.GetMouseButtonDown(0))
        {
           
            // if it's true that means that we've selected a unit, so no moving
            if (HandleUnitSelection()) return;
            
            HandleSelectedAction();            
        }       
    }

    //tries tor raycast to unit, if it suceeds returns true otherwise false
    private bool HandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if(Input.GetMouseButtonDown(0) && raycastHit.) { } this is ok, but Raycast returns true if there is a hit with an object from the layer unitLayerMAsk

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
        {
            //selectedUnit = raycastHit.transform.GetComponent<Unit>(); there is a better way for getting component called TryGtComponent
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {
                //if this unit is already selected, don't trigger selection event again
                if (unit == selectedUnit)
                {
                    return false;
                }

                if (unit.IsEnemy()) 
                { 
                    return false;
                }

                SetSelectedUnit(unit);
                return true;
            }
            else return false;
        }
        else return false;        
    }

    private void HandleSelectedAction()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            //we're tying actions for grid cells, each action will have a grid cell as a target
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidGridPositionForAction(mouseGridPosition))
            {
                if (selectedUnit.TrySpendPointsToTakeAction(selectedAction))
                {
                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                    SetBusy();
                    selectedAction.TakeAction(ClearBusy, mouseGridPosition);
                }                  
            }            
        }
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        //sets default move action
        SetSelectedAction(unit.GetAction<MoveAction>());
        //triggeres selcetion change events
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    { return selectedAction; }

    public void SetBusy()
    {
        isBusy= true;
        //OnActionBusy?.Invoke(this, EventArgs.Empty);
        OnBusyChanged?.Invoke(this, isBusy);
    }

    public void ClearBusy()
    { 
        isBusy= false;
        //OnActionFree?.Invoke(this, EventArgs.Empty);
        OnBusyChanged?.Invoke(this, isBusy);
    }
}

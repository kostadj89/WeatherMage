using Assets.Scripts.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction // MonoBehaviour//, IAction
{
    internal const float SPIN_ANGLE = 360f;
    private bool isSpinning;
    private float totalSpinAmount;

    //protected CompleteActionDelegate completeAction;
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) { return; }

        //if (isSpinning)
        //{
        //    transform.eulerAngles += new Vector3(0, spinAngle * Time.deltaTime, 0);
        //}

        float spinAmount = SpinAction.SPIN_ANGLE * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);

        totalSpinAmount += spinAmount;

        if (totalSpinAmount>=360f) 
        { 
            isActive= false;
            totalSpinAmount = 0f;
            //completeAction();
            onActionComplete();
        }

    }
    public override string GetActionName()
    {
        return "SPIN";
    }

    public void ToggleSpin()
    { 
        isSpinning = !isSpinning;
        isActive = isSpinning;
    }

    //public void Spin(CompleteActionDelegate completeActionDelegate)
    //{
    //    isActive = true;
    //    completeAction = completeActionDelegate;
    //}

    public override void TakeAction(Action completeActionDelegate, GridPosition gridPosition)
    {
        isActive = true;
        onActionComplete = completeActionDelegate;
    }

    public override int GetActionCost()
    {
        return 2;
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        validGridPositions.Add(unit.GetGridPosition());

        return validGridPositions;
    }
    public bool isExecuting()
    {
        return isActive;
    }
}

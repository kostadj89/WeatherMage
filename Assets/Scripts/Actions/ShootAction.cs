using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private float totalSpinAmount;
    [SerializeField]
    private int maxShootRadius;
    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        GridPosition offset;

        GridPosition testGridPos;

        for (int i = -maxShootRadius; i <= maxShootRadius; i++)
        {
            for (int j = -maxShootRadius; j <= maxShootRadius; j++)
            {
                offset = new GridPosition(i, j);
                testGridPos = offset + unitGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPos) || LevelGrid.Instance.IsGridPositionOccupied(testGridPos))
                {
                    continue;
                }

                validGridPositions.Add(testGridPos);
                //Debug.Log(testGridPos);
            }
        }

        return validGridPositions;
    }

    public override void TakeAction(Action OnCompleteAction, GridPosition gridPosition)
    {
        isActive = true;
        onActionComplete = OnCompleteAction;
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

        if (totalSpinAmount >= 360f)
        {
            isActive = false;
            totalSpinAmount = 0f;
            //completeAction();
            onActionComplete();
        }
    }
}

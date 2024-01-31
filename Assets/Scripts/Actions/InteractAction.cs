using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [SerializeField]
    private int maxInteractRadius = 1;
    public override string GetActionName()
    {
        return "Interact";
    }

    public override List<GridPosition> GetAllValidGridPositionsForAction()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition originGridPosition = unit.GetGridPosition();
        GridPosition offsetGridPosition;

        GridPosition testGridPos;

        for (int i = -maxInteractRadius; i <= maxInteractRadius; i++)
        {
            for (int j = -maxInteractRadius; j <= maxInteractRadius; j++)
            {
                offsetGridPosition = new GridPosition(i, j, 0);
                testGridPos = offsetGridPosition + originGridPosition;

                // check to see if the cell is out of grid bounds, if yes ignore
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPos))
                {
                    continue;
                }

                IInteractible interactible = LevelGrid.Instance.GetInteractibleAtGridPosition(testGridPos);

                if (interactible == null)
                {
                    continue;
                }
                //in the end, we got an opossing unit inside of the range on the valid grid cell, and we add that unit's grid cell
                validGridPositions.Add(testGridPos);
                //Debug.Log(testGridPos);
            }
        }

        return validGridPositions;
    }

    public override ScoredEnemyAIAction GetScoredEnemyAIActionOnGridPosition(GridPosition gridPos)
    {
        return new ScoredEnemyAIAction
        {
            gridPosition = gridPos,
            actionValue = 0
        };
    }

    public override void TakeAction(Action OnCompleteAction, GridPosition gridPosition)
    {
        Debug.Log("Interact action");
        IInteractible interactible = LevelGrid.Instance.GetInteractibleAtGridPosition(gridPosition);
        interactible.Interact(OnInteractionComplete);

        ActionStart(OnCompleteAction);
    }

    private void OnInteractionComplete()
    {
        ActionEnd();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] gridSystemVisualSingles;
    // Start is called before the first frame update
    void Start()
    {
        gridSystemVisualSingles = new GridSystemVisualSingle[LevelGrid.Instance.GetGridWidth(), LevelGrid.Instance.GetGridHeight()];

        for (int i = 0; i < LevelGrid.Instance.GetGridWidth(); i++)
        {
            for (int j = 0; j < LevelGrid.Instance.GetGridHeight(); j++)
            {
                GridPosition gridPos= new GridPosition(i, j);                

                gridSystemVisualSingles[i,j] = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldFromGridPosition(gridPos), Quaternion.identity).GetComponent<GridSystemVisualSingle>();
            }
        }

        HideAllGridPositions();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGridVisuals();
    }

    public void HideAllGridPositions()
    {
        for (int i = 0; i < LevelGrid.Instance.GetGridWidth(); i++)
        {
            for (int j = 0; j < LevelGrid.Instance.GetGridHeight(); j++)
            {
                gridSystemVisualSingles[i,j].Hide();
            }
        }
    }

    public void ShowReachableGridPositions(List<GridPosition> gridPositions)
    {
        gridPositions.ForEach(gridPosition => { gridSystemVisualSingles[gridPosition.x, gridPosition.y].Show(); });
    }

    public void UpdateGridVisuals()
    {
        HideAllGridPositions();
        ShowReachableGridPositions(UnitActionSystem.Instance.GetSelectedAction().GetAllValidGridPositionsForAction());
    }
}

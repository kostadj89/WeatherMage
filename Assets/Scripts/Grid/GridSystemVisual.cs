using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//this class handles visualation of the grid system
public class GridSystemVisual : MonoBehaviour
{
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        Red,
        Blue,
        Green,
        Purple
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterials;

    private GridSystemVisualSingle[,] gridSystemVisualSingles;
    private GridSystemVisualSingle lastSelected;
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

        //subscribe to events
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged_GridSystemVisual;

        UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged_GridSystemVisual;
        LevelGrid.Instance.OnAnyUnitMoved += OnAnyUnitMoved_GridSystemVisual;

        UpdateGridVisuals();

        //Show all grid pos
        int gridWidth = LevelGrid.Instance.GetGridWidth();
        int gridHeight = LevelGrid.Instance.GetGridHeight();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                gridSystemVisualSingles[x, z].Show(GetGridVisualTypeMaterial(GridVisualType.Green));
            }
        }
    }

    private void OnBusyChanged_GridSystemVisual(object sender, bool hideVisuals)
    {
        UpdateGridVisuals();                
    }

    private void OnAnyUnitMoved_GridSystemVisual(object sender, EventArgs e)
    {
        UpdateGridVisuals();
    }

    private void OnSelectedActionChanged_GridSystemVisual(object sender, EventArgs e)
    {
        UpdateGridVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastSelected!=null)
        {
            lastSelected.HideSelected();
        }
        //UpdateGridVisuals();
        Vector3 mouseWorldPos = MouseWorld.GetPosition();
        GridPosition currGridPos = LevelGrid.Instance.GetGridPosition(mouseWorldPos);

        if (LevelGrid.Instance.IsValidGridPosition(currGridPos))
        {
            lastSelected = gridSystemVisualSingles[currGridPos.x, currGridPos.y];
        }

        if (lastSelected != null)
        {
            lastSelected.ShowSelected();
        }
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

    public void ShowGridPositions(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        gridPositions.ForEach(gridPosition => { gridSystemVisualSingles[gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial( gridVisualType)); });
    }

    public void UpdateGridVisuals() 
    {
        HideAllGridPositions();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType actionGridTypeVisual;

        //if (selectedAction != null && !selectedAction.IsActive() && !UnitActionSystem.Instance.GetIsBusy()) 
        if (selectedAction != null && !UnitActionSystem.Instance.GetIsBusy())
        {
            switch (selectedAction)        
            {
                case MoveAction moveAction:
                    actionGridTypeVisual = GridVisualType.Purple;
                    break;
                case InteractAction interactAction:
                    actionGridTypeVisual = GridVisualType.Green;
                    break;
                case SpinAction spinAction:
                    actionGridTypeVisual = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    actionGridTypeVisual = GridVisualType.Red;
                    Unit unit = shootAction.GetUnit();
                    //ShowGridPositionRangeCircural(unit.GetGridPosition(), shootAction.GetRadius(), GridVisualType.Purple);
                    break;
                case AreaShootAction areaShootAction:
                    actionGridTypeVisual = GridVisualType.Red;
                    Unit unit_Area = areaShootAction.GetUnit();
                    //ShowGridPositionRangeCircural(unit_Area.GetGridPosition(), areaShootAction.GetRadius(), GridVisualType.Purple);
                    break;
                case MeleeAction meleeAction:
                    actionGridTypeVisual = GridVisualType.Red;
                    Unit unitMelee = meleeAction.GetUnit();
                    //ShowGridPositionRangeSquare(unitMelee.GetGridPosition(), meleeAction.GetRadius(), GridVisualType.Purple);
                    break;
                default:
                    actionGridTypeVisual = GridVisualType.Purple;
                    break;
            };

            ShowGridPositions(selectedAction.GetAllValidGridPositionsForAction(), actionGridTypeVisual);
        }
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterials)
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.Log("Could not time GridVisualTypeMaterial for gridVisualType: " + gridVisualType);

        return null;
    }

    private void ShowGridPositionRangeCircural(GridPosition gridPosition, int range,GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                GridPosition testGridPosition = new GridPosition(i,j) + gridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                
                int testDistance = Mathf.Abs(i) + Mathf.Abs(j);

                if ( testDistance > range )
                {
                    continue;
                }

                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositions(gridPositions, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();

        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                GridPosition testGridPosition = new GridPosition(i, j) + gridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }               

                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositions(gridPositions, gridVisualType);
    }
}

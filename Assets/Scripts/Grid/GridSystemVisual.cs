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

    private GridSystemVisualSingle[,,] gridSystemVisualSingles;
    private GridSystemVisualSingle lastSelected;
    // Start is called before the first frame update
    void Start()
    {
        int gridWidth = LevelGrid.Instance.GetGridWidth();
        int numOfFloors = LevelGrid.Instance.GetNumberOfFloors();
        int gridHeight = LevelGrid.Instance.GetGridHeight();

        gridSystemVisualSingles = new GridSystemVisualSingle[gridWidth, gridHeight, numOfFloors];
        
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                for (int k = 0; k < numOfFloors; k++)
                {
                    GridPosition gridPos = new GridPosition(i, j,k);

                    gridSystemVisualSingles[i, j,k] = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldFromGridPosition(gridPos), Quaternion.identity).GetComponent<GridSystemVisualSingle>();
                }
                
            }
        }

        HideAllGridPositions();

        //subscribe to events
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged_GridSystemVisual;

        UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged_GridSystemVisual;
        LevelGrid.Instance.OnAnyUnitMoved += OnAnyUnitMoved_GridSystemVisual;

        UpdateGridVisuals();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                for (int y = 0; y < numOfFloors; z++)
                {

                    gridSystemVisualSingles[x, z,y].Show(GetGridVisualTypeMaterial(GridVisualType.Green));
                }
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
            lastSelected = gridSystemVisualSingles[currGridPos.x, currGridPos.y,currGridPos.floor];
        }

        if (lastSelected != null)
        {
            lastSelected.ShowSelected();
        }
    }

    public void HideAllGridPositions()
    {
        int gridWidth = LevelGrid.Instance.GetGridWidth();
        int gridHeight = LevelGrid.Instance.GetGridHeight();
        int gridFloors = LevelGrid.Instance.GetNumberOfFloors();

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                for (int k = 0; k < gridFloors; k++)
                {
                    gridSystemVisualSingles[i, j, k].Hide();
                }                
            }
        }
    }

    public void ShowGridPositions(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        gridPositions.ForEach(gridPosition => { gridSystemVisualSingles[gridPosition.x, gridPosition.y,gridPosition.floor].Show(GetGridVisualTypeMaterial( gridVisualType)); });
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
                    actionGridTypeVisual = GridVisualType.Purple;
                    break;
                case SpinAction spinAction:
                    actionGridTypeVisual = GridVisualType.Purple;
                    break;
                case ShootAction shootAction:
                    actionGridTypeVisual = GridVisualType.Purple;
                    Unit unit = shootAction.GetUnit();
                    //ShowGridPositionRangeCircural(unit.GetGridPosition(), shootAction.GetRadius(), GridVisualType.Purple);
                    break;
                case AreaShootAction areaShootAction:
                    actionGridTypeVisual = GridVisualType.Purple;
                    Unit unit_Area = areaShootAction.GetUnit();
                    //ShowGridPositionRangeCircural(unit_Area.GetGridPosition(), areaShootAction.GetRadius(), GridVisualType.Purple);
                    break;
                case MeleeAction meleeAction:
                    actionGridTypeVisual = GridVisualType.Purple;
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
                //change later
                GridPosition testGridPosition = new GridPosition(i,j, 0) + gridPosition;

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
                //Change later
                GridPosition testGridPosition = new GridPosition(i, j, 0) + gridPosition;

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

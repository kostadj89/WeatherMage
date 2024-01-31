using Assets.Scripts;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//holds logic for creating a level grid, and manipulation of grid objects, like adding stuff to grid cells
public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3.0f;

    public event EventHandler OnAnyUnitMoved;

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private int numberOfFloors = 1;

    private List<IGridSystem<GridObject>> gridSystemList;
    [SerializeField]
    private bool isHex;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one LevelGrid!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystemList = new List<IGridSystem<GridObject>>();

        for (int i = 0; i < numberOfFloors; i++)
        {
            if (isHex)
            {
                //Use hex
                GridSystemHex<GridObject> gridSystemHex = new GridSystemHex<GridObject>(width, height, cellSize, i, FLOOR_HEIGHT, (GridSystemHex<GridObject> gs, GridPosition gp) => new GridObject(gs, gp));
                gridSystemList.Add(gridSystemHex);
            }
            else
            {
                //Use square
                GridSystemSquare<GridObject> gridSystemSquare = new GridSystemSquare<GridObject>(width, height, cellSize, i, FLOOR_HEIGHT, (GridSystemSquare<GridObject> gs, GridPosition gp) => new GridObject(gs, gp));
                gridSystemList.Add(gridSystemSquare);
            }
        }
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    // Start is called before the first frame update
    void Start()
    {
        PathfindingSquareGrid.Instance.Setup(width, height, cellSize);
    }

    // Update is called once per frame
    void Update()
    {

    }   

    //GridObject actions

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject= this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && !gridObject.GetAllUnitsAndDamagableObjects().Contains(unit)) 
        { 
            this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.AddUnitOrDestructible(unit);
        }
    }

    //this is bad because i'm lazy i've hardcoded that there could only be one interactible... because i haven't determined yet.. ugh probabbly only one thing, unit, destructible and interactible should be on the tile.. but maybe not.. for example, a door can be on one tile, but when the door opens unit should be able to stand on the same tile ok ok
    public void AddInteractibleAtGridPosition(GridPosition gridPosition, IInteractible interactible)
    {
        GridObject gridObject = this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && gridObject.GetInteractible()!= interactible)
        {
            this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.SetInteractible(interactible);
        }
    }

    public void AddDestructibleAtGridPosition(GridPosition gridPosition, ICanTakeDamage destructible)
    {
        GridObject gridObject = this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && !gridObject.GetAllUnitsAndDamagableObjects().Contains(destructible))
        {
            this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.AddUnitOrDestructible(destructible);
        }
    }

    public List<ICanTakeDamage> GetUnitsAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.GetAllUnitsAndDamagableObjects();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.GetUnit();
    }

    public IInteractible GetInteractibleAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.GetInteractible();
    }

    public ICanTakeDamage GetUnitOrDestructibleAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.GetUnitOrDestructible();
    }

    public void ClearUnitOrDestructibleAtGridPosition(GridPosition gridPosition, ICanTakeDamage unit)
    {
        this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.RemoveUnitOrDestructible(unit);
    }

    public void ClearInteractibleAtGridPosition(GridPosition gridPosition, IInteractible interactible)
    {
        this.gridSystemList[gridPosition.floor].GetGridObjectFromGridPos(gridPosition)?.RemoveInteractible(interactible);
    }

    //expose gridSystem
    public int GetFloorFromWorldPosition(Vector3 vector3) => Mathf.RoundToInt(vector3.y/FLOOR_HEIGHT);
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystemList[GetFloorFromWorldPosition(worldPosition)].GetGridPosFromVector(worldPosition);
    public Vector3 GetWorldFromGridPosition(GridPosition gridPosition) => GetGridSystemByFloorIndex(gridPosition.floor).GetWorldFromGridPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => GetGridSystemByFloorIndex(gridPosition.floor).IsValidGridPosition(gridPosition);
    public bool IsGridPositionOccupied(GridPosition gridPosition) => GetGridSystemByFloorIndex(gridPosition.floor).IsGridPositionOccupied(gridPosition);

    //assuming all our floors have the grid of the same width and height
    public int GetGridWidth() => GetGridSystemByFloorIndex(0).GetWidth();
    public int GetGridHeight() => GetGridSystemByFloorIndex(0).GetHeight();

    public int GetNumberOfFloors() => numberOfFloors;

    public bool GetIsHexGrid() => isHex;

    public void UnitMovedGridPosition(Unit unit,GridPosition fromPos,GridPosition toPos)
    {         
        ClearUnitOrDestructibleAtGridPosition(fromPos, unit);
        AddUnitAtGridPosition(toPos,unit);
        OnAnyUnitMoved?.Invoke(this, EventArgs.Empty);
    }

    //only checking the grid on the same floor, maybe should change later
    internal List<GridPosition> GetAllCellsInTheRange(GridPosition targetPosition, int range)
    {
        //temp floor 0
        return GetGridSystemByFloorIndex(0).GetAllCellsInTheRange(targetPosition,range);
    }

    internal List<ICanTakeDamage> GetAllPotentionalTargetsFromTheCells(List<GridPosition> gridPositions)
    {
        List<ICanTakeDamage> potentionalTargets = new List<ICanTakeDamage>();
        ICanTakeDamage tempTarget;

        foreach (GridPosition gridPosition in gridPositions)
        {
            tempTarget = GetUnitOrDestructibleAtGridPosition(gridPosition);
            if (tempTarget != null)
            {
                potentionalTargets.Add(tempTarget);
            }
        }

        return potentionalTargets;
    }

    internal IGridSystem<GridObject> GetGridSystemByFloorIndex(int floorIndex) { return gridSystemList[floorIndex]; }
}

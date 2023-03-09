using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMoved;

    [SerializeField] private Transform gridDebugObjectPrefab;
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one LevelGrid!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystem = new GridSystem<GridObject>(10, 10, 2,(GridSystem<GridObject> gs,GridPosition gp) => new GridObject(gs, gp)); 
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }   

    //GridObject actions

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject= this.gridSystem.GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && !gridObject.GetAllUnits().Contains(unit)) 
        { 
            this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.AddUnit(unit);
        }
    }

    public List<Unit> GetUnitsAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.GetAllUnits();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.GetUnit();
    }

    public void ClearUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.RemoveUnit(unit);
    }

    //expose gridSystem

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosFromVector(worldPosition);
    public Vector3 GetWorldFromGridPosition(GridPosition gridPosition) => gridSystem.GetWorldFromGridPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    public bool IsGridPositionOccupied(GridPosition gridPosition) => gridSystem.IsGridPositionOccupied(gridPosition);
    public int GetGridWidth() => gridSystem.GetWidth();
    public int GetGridHeight() => gridSystem.GetHeight();

    public void UnitMovedGridPosition(Unit unit,GridPosition fromPos,GridPosition toPos)
    {         
        ClearUnitAtGridPosition(fromPos, unit);
        AddUnitAtGridPosition(toPos,unit);
        OnAnyUnitMoved?.Invoke(this, EventArgs.Empty);
    }
}

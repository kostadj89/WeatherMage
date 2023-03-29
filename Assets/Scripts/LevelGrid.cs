using Assets.Scripts;
using JetBrains.Annotations;
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

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f;

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

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> gs,GridPosition gp) => new GridObject(gs, gp)); 
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    // Start is called before the first frame update
    void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    // Update is called once per frame
    void Update()
    {

    }   

    //GridObject actions

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject= this.gridSystem.GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && !gridObject.GetAllUnitsAndDamagableObjects().Contains(unit)) 
        { 
            this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.AddUnitOrDestructible(unit);
        }
    }

    public void AddDestructibleAtGridPosition(GridPosition gridPosition, ICanTakeDamage destructible)
    {
        GridObject gridObject = this.gridSystem.GetGridObjectFromGridPos(gridPosition);
        if (gridObject != null && !gridObject.GetAllUnitsAndDamagableObjects().Contains(destructible))
        {
            this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.AddUnitOrDestructible(destructible);
        }
    }

    public List<ICanTakeDamage> GetUnitsAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.GetAllUnitsAndDamagableObjects();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.GetUnit();
    }

    public ICanTakeDamage GetUnitOrDestructibleAtGridPosition(GridPosition gridPosition)
    {
        return this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.GetUnitOrDestructible();
    }

    public void ClearUnitOrDestructibleAtGridPosition(GridPosition gridPosition, ICanTakeDamage unit)
    {
        this.gridSystem.GetGridObjectFromGridPos(gridPosition)?.RemoveUnitOrDestructible(unit);
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
        ClearUnitOrDestructibleAtGridPosition(fromPos, unit);
        AddUnitAtGridPosition(toPos,unit);
        OnAnyUnitMoved?.Invoke(this, EventArgs.Empty);
    }

    internal List<GridPosition> GetAllCellsInTheRange(Vector3 targetPosition, int range)
    {
        List<GridPosition> gridPositions= new List<GridPosition>();

        GridPosition targetGridPosition = GetGridPosition(targetPosition);
        GridPosition tempGridPosition;

        for (int i = targetGridPosition.x - 1; i <= targetGridPosition.x +1; i++)
        {
            for (int j = targetGridPosition.y - 1; j <= targetGridPosition.y + 1; j++)
            {
                tempGridPosition = new GridPosition(i, j);

                if (tempGridPosition == targetGridPosition)
                {
                    continue;
                }

                if (gridSystem.IsValidGridPosition(tempGridPosition))
                {
                    gridPositions.Add(tempGridPosition);
                }
            }
        }

        return gridPositions;
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
}

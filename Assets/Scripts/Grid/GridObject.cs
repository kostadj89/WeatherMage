using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridCellPosition;
    private List<Unit> unitsAtGridObject;
       
    public GridObject(GridPosition gridCell)
    {
        this.gridCellPosition = gridCell;
    }
    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridCell)
    {
        this.gridSystem = gridSystem;
        this.gridCellPosition = gridCell;
        this.unitsAtGridObject = new List<Unit>();
    }

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridCell, List<Unit> unitsAtGridObject) : this(gridSystem, gridCell)
    {
        if (unitsAtGridObject == null)
        {
            this.unitsAtGridObject = new List<Unit>();
        }
        else 
        { 
            this.unitsAtGridObject = unitsAtGridObject; 
        }        
    }

    public void AddUnit(Unit unit)
    {
        this.unitsAtGridObject.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (this.unitsAtGridObject.Contains(unit))
        {
            this.unitsAtGridObject.Remove(unit);
        }        
    }

    public Unit GetUnit()
    {
        return unitsAtGridObject[0];
    }

    public List<Unit> GetAllUnits()
    { 
        return unitsAtGridObject;
    }

    public override string ToString()
    {
        string unitString = string.Empty;
        foreach (Unit unit in unitsAtGridObject)
        {
            unitString += unit + "\n";
        }
        return gridCellPosition.ToString()+ unitString;
    }

    public bool IsOccupied()
    {
        return unitsAtGridObject.Count > 0;
    }
}

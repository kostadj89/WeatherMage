using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//holds logic if the grid tile is occupied or not, and references and methods for getting occupying units, destructibles as well as interactibles
public class GridObject
{
    private IGridSystem<GridObject> gridSystem;
    private GridPosition gridCellPosition;
    private List<ICanTakeDamage> damagableObjectsAtGridObject;
    private List<IInteractible> interactibles;
       
    public GridObject(GridPosition gridCell)
    {
        this.gridCellPosition = gridCell;
    }
    public GridObject(IGridSystem<GridObject> gridSystem, GridPosition gridCell)
    {
        this.gridSystem = gridSystem;
        this.gridCellPosition = gridCell;
        this.damagableObjectsAtGridObject = new List<ICanTakeDamage>();
        this.interactibles = new List<IInteractible>();
    }

    public GridObject(GridSystemSquare<GridObject> gridSystem, GridPosition gridCell, List<ICanTakeDamage> unitsAtGridObject) : this(gridSystem, gridCell)
    {
        if (unitsAtGridObject == null)
        {
            this.damagableObjectsAtGridObject = new List<ICanTakeDamage>();
        }
        else 
        { 
            this.damagableObjectsAtGridObject = unitsAtGridObject; 
        }        
    }

    public void AddUnitOrDestructible(ICanTakeDamage unitOrDestructible)
    {
        this.damagableObjectsAtGridObject.Add(unitOrDestructible);
    }

    public void RemoveUnitOrDestructible(ICanTakeDamage unitOrDestructible)
    {
        if (this.damagableObjectsAtGridObject.Contains(unitOrDestructible))
        {
            this.damagableObjectsAtGridObject.Remove(unitOrDestructible);
        }        
    }

    public Unit GetUnit()
    {
        if (damagableObjectsAtGridObject.Count > 0)
        {
            switch (damagableObjectsAtGridObject[0])
            {
                case Unit unitAtPosition:
                    return unitAtPosition;
                default:
                    return null;
            }
            
        }
        else
        {
            return null;
        }
    }

    public ICanTakeDamage GetUnitOrDestructible()
    {
        if (damagableObjectsAtGridObject.Count > 0)
        {
            return damagableObjectsAtGridObject[0];           
        }
        else
        {
            return null;
        }
    }

    public IInteractible GetInteractible()
    {
        return interactibles.Count >0? interactibles.First(): null;
    }

    public void SetInteractible(IInteractible interactible)
    {
        if (interactibles.Contains(interactible)) { return; }
        interactibles.Add(interactible);
    }
    internal void RemoveInteractible(IInteractible interactible)
    {
        if (this.interactibles.Contains(interactible))
        {
            this.interactibles.Remove(interactible);
        }
    }

    public List<ICanTakeDamage> GetAllUnitsAndDamagableObjects()
    { 
        return damagableObjectsAtGridObject;
    }

    public override string ToString()
    {
        string unitString = string.Empty;
        foreach (ICanTakeDamage unit in damagableObjectsAtGridObject)
        {
            unitString += unit + "\n";
        }
        return gridCellPosition.ToString()+ unitString;
    }

    public bool IsOccupied()
    {
        return damagableObjectsAtGridObject.Count > 0;
    }

   
}

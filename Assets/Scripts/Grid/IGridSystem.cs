using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IGridSystem<TGridObject>
{
    TGridObject GetGridObjectFromGridPos(GridPosition gridPosition);
    GridPosition GetGridPosFromVector(Vector3 worldPosition);
    Vector3 GetWorldFromGridPosition(GridPosition gridPosition);
    List<GridPosition> GetAllCellsInTheRange(GridPosition targetPosition, int range);
    bool IsValidGridPosition(GridPosition gridPosition);
    bool IsGridPositionOccupied(GridPosition gridPosition);

    int GetWidth();
    int GetHeight();
}


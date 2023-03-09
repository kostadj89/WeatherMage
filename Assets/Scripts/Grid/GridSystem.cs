using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               y == position.y;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override string ToString()
    {
        return "(X:"+x+",Y:"+y +")";
    }

    public static bool operator == (GridPosition v, GridPosition u)
    { 
        return (v.x == u.x) && (v.y == u.y); 
    }

    public static bool operator !=(GridPosition v, GridPosition u)
    {
        return !(v == u); 
    }

    public static GridPosition operator +(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x + u.x,v.y+u.y);
    }

    public static GridPosition operator -(GridPosition v, GridPosition u)
    {
        return new GridPosition(v.x - u.x, v.y - u.y);
    }
}
public class GridSystem<TGridObject>
{ 
    private int width, height;
    private float cellSize;
    private float cellOffset = 0.2f;
    private TGridObject[,] gridObjectArray;

    //Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject delegate which accepts arguments of type GridSystem<TGridObject>, GridPosition and returns type TGridObject
    public GridSystem(int w,int h, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    { 
        this.width = w;
        this.height = h;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[w,h];
        for (int i = 0 ; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Debug.DrawLine(GetWorldPositionFromCoordinates(i, j), GetWorldPositionFromCoordinates(i, j) + Vector3.right * cellOffset, Color.white, 9999);
                GridPosition gridCell = new GridPosition(i,j);
                gridObjectArray[i,j] = createGridObject(this, gridCell);
            }
        }
    }

    public Vector3 GetWorldPositionFromCoordinates(int x, int y)
    {
        return new Vector3(x,0,y)*cellSize;
    }

    public Vector3 GetWorldFromGridPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y) * cellSize;
        //return GetWorldPositionFromCoordinates(gridPosition.x, gridPosition.y);
    }

    public GridPosition GetGridPosFromVector(Vector3 position)
    {
        return new GridPosition(Mathf.RoundToInt(position.x / cellSize), Mathf.RoundToInt(position.z / cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        Transform debugObjectTransform;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GridPosition gridPosition= new GridPosition(i,j);
                debugObjectTransform = GameObject.Instantiate(debugPrefab, GetWorldFromGridPosition(gridPosition), Quaternion.identity);
                //debugPrefab.GetComponentInChildren<TextMeshPro>().text = $"{i}, {j}";
                GridDebugObject gridDebugObject = debugObjectTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObjectFromGridPos(gridPosition) as GridObject);
            }
        }
    }
    
    public TGridObject GetGridObjectFromGridPos(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.y];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x < width && gridPosition.y < height && gridPosition.x >= 0 && gridPosition.y >= 0;
    }

    public bool IsGridPositionOccupied(GridPosition gridPosition)
    {
        return (gridObjectArray[gridPosition.x,gridPosition.y] as GridObject).IsOccupied();
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight() 
    {
        return height;
    }
}


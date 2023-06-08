using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//each grid tile is determined by the TGridObject, and grid position; grid obj will hold the info about gridPosition
public class GridSystemSquare<TGridObject> : IGridSystem<TGridObject>
{ 
    private int width, height;
    private float cellSize;
    private float cellOffset = 0.2f;
    private TGridObject[,] gridObjectArray;

    //Func<GridSystemSquare<TGridObject>, GridPosition, TGridObject> createGridObject delegate which accepts arguments of type GridSystemSquare<TGridObject>, GridPosition and returns type TGridObject
    public GridSystemSquare(int w,int h, float cellSize, Func<GridSystemSquare<TGridObject>, GridPosition, TGridObject> createGridObject)
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
                GridPosition gridPosition = new GridPosition(i,j);
                gridObjectArray[i,j] = createGridObject(this, gridPosition);
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
                gridDebugObject.SetGridObject(GetGridObjectFromGridPos(gridPosition));
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
        Debug.Log(gridObjectArray[gridPosition.x, gridPosition.y]);
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

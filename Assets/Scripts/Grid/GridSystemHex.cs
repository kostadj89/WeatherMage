using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Android;



//each grid tile is determined by the TGridObject, and grid position; grid obj will hold the info about gridPosition
public class GridSystemHex<TGridObject> : IGridSystem<TGridObject>
{
    private const float VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    private int width, height;
    private float cellSize;
    private float cellOffset = 0.2f;
    private TGridObject[,] gridObjectArray;

    private static readonly GridPosition[] DirectionsEven = new GridPosition[]
{
        new GridPosition(1, 0),       // Right
        new GridPosition(-1, -1),      // Bottom-left
        new GridPosition(0, -1),      // Bottom
        new GridPosition(-1, 0),      // Left
        new GridPosition(-1, 1),      // Top-left
        new GridPosition(0, 1)        // Top
};

    private static readonly GridPosition[] DirectionsOdd = new GridPosition[]
{
        new GridPosition(1, 0),       // Right
        new GridPosition(1, -1),      // Bottom-right
        new GridPosition(0, -1),      // Bottom
        new GridPosition(-1, 0),      // Left
        new GridPosition(1, 1),      // Top-right
        new GridPosition(0, 1)        // Top
};

    //Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject delegate which accepts arguments of type GridSystemHex<TGridObject>, GridPosition and returns type TGridObject
    public GridSystemHex(int w,int h, float cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
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
        return new Vector3(gridPosition.x, 0, 0) * cellSize +
            new Vector3(0, 0, gridPosition.y) * cellSize * VERTICAL_OFFSET_MULTIPLIER +
            GetGridRowOffset(gridPosition);
        //return GetWorldPositionFromCoordinates(gridPosition.x, gridPosition.y);
    }

    private Vector3 GetGridRowOffset(GridPosition gridPosition)
    {
        return ((gridPosition.y % 2 == 1) ? new Vector3(1, 0, 0) * cellSize * 0.5f : Vector3.zero);
    }

    public GridPosition GetGridPosFromVector(Vector3 position)
    {
        return ClosesetGridPosition(position);
    }

    private GridPosition ClosesetGridPosition(Vector3 position)
    {
        GridPosition roughlyEstimatedGridPos = new GridPosition(Mathf.RoundToInt(position.x / cellSize), Mathf.RoundToInt(position.z / cellSize / VERTICAL_OFFSET_MULTIPLIER));

        List<GridPosition> neighbourGridPositions = GetNeighbourGridPositions(roughlyEstimatedGridPos);

        GridPosition closest = roughlyEstimatedGridPos;

        foreach (GridPosition neighbour in neighbourGridPositions)
        {
            if (Vector3.Distance(GetWorldFromGridPosition(neighbour),position) < Vector3.Distance(GetWorldFromGridPosition(closest), position))
            {
                closest = neighbour;
            }
        }

        return closest;
    }

    private static List<GridPosition> GetNeighbourGridPositions(GridPosition targetGridPos)
    {
        bool isOddRow = (targetGridPos.x % 2 == 1);

        List<GridPosition> neighbourGridPositions = new List<GridPosition>()
        {
            targetGridPos + new GridPosition(-1,0),
            targetGridPos + new GridPosition(1,0),

            targetGridPos + new GridPosition(0,1),
            targetGridPos + new GridPosition(0,-1),

            targetGridPos + new GridPosition(isOddRow? +1:-1,1),
            targetGridPos + new GridPosition(isOddRow? +1:-1,-1)
        };

        List<GridPosition> positionsToRemove = new List<GridPosition>();

        foreach (GridPosition neighbour in neighbourGridPositions)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(neighbour))
            {
                positionsToRemove.Add(neighbour);
            }
        }

        foreach (GridPosition positionToRemove in positionsToRemove)
        {
            neighbourGridPositions.Remove(positionToRemove);
        }

        return neighbourGridPositions;
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
        //Debug.Log(gridObjectArray[gridPosition.x, gridPosition.y]);
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

    public List<GridPosition> GetAllCellsInTheRange(GridPosition targetPosition, int range)
    {
        GridPosition centerHex = targetPosition;
        List<GridPosition> gridPositionsInTheArea = new List<GridPosition>();
        int tilesInTheCenterRow = 2*range+1;
        GridPosition tempGridPosition;

        //get all grid positions from the same row
        for (int i = centerHex.x - range; i <= centerHex.x+range; i++)
        {
            //we fix the row, since i've fucked up by switching coordinates and y represent row
            tempGridPosition = new GridPosition(i, centerHex.y);

            if(LevelGrid.Instance.IsValidGridPosition(tempGridPosition)) gridPositionsInTheArea.Add(tempGridPosition);
        }
        //smth should be the leftmost column index of the apprasing row
        int leftMostIndex = (int)(centerHex.x - range);// + coef);// * (centerHex.x >= range ? 1 : -1)) ;

        //take the leftmost hex, and then go up and down row by row, always decresing number of hexes to add by level of depth, coef is there to tell us which number should be added (0,-1) or (-1,-1) and sim for above
        for (int i = 1; i <= range; i++)
        {
            int counter = 0;
            //offset of row
            //float coef = (centerHex.y % 2 == 0) ? i * 1 / 2f : (i + 1) * 1 / 2f ;
            //if the edge case, -1.5
            
            //check to see if we need to decrease left most index 
            if ((centerHex.y % 2 == 0) && ((centerHex.y + i) % 2 == 0) || (centerHex.y % 2 != 0) && ((centerHex.y + i) % 2 == 0))
            {
                leftMostIndex++;
            }

            while (counter< tilesInTheCenterRow - i)
            {
                

                tempGridPosition = new GridPosition(leftMostIndex + counter, centerHex.y + i);
                if (LevelGrid.Instance.IsValidGridPosition(tempGridPosition)) gridPositionsInTheArea.Add(tempGridPosition);

                if (centerHex.y-i>=0)
                {
                    tempGridPosition = new GridPosition(leftMostIndex + counter, centerHex.y - i);
                    if (LevelGrid.Instance.IsValidGridPosition(tempGridPosition)) gridPositionsInTheArea.Add(tempGridPosition);
                }                

                counter++;
            }
        }

        return gridPositionsInTheArea;
    }
}


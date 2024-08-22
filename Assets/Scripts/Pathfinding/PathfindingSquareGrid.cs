using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathfindingSquareGrid : MonoBehaviour
{
    private const int MAX_ITERATIONS = 1000;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private int width;
    private int height;
    private float cellSize;

    private GridSystemSquare<PathNode> gridSystem;

    [SerializeField]
    private Transform gridDebugObject;
    [SerializeField]
    private LayerMask obstaclesLayerMask;

    public static PathfindingSquareGrid Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            return;  
        }

        Instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    public List<GridPosition> FindAPath(GridPosition startGridPos, GridPosition endGridPos, out int pathDistance)
    {
        //init opened and closed nodes
        List<PathNode> openPathNodes = new List<PathNode>();
        List<PathNode> closedPathNodes = new List<PathNode>();

        //get start node from grid pos
        PathNode startPathNode = gridSystem.GetGridObjectFromGridPos(startGridPos);

        //get end node from grid pos
        PathNode endPathNode = gridSystem.GetGridObjectFromGridPos(endGridPos);

        //add start node to opened list first 
        openPathNodes.Add(startPathNode);

        //initialization
        for(int x = 0;x<gridSystem.GetWidth();x++)
        {
            for (int y = 0; y < gridSystem.GetHeight(); y++)
            {
                //TODO: change later to consider different floors
                GridPosition currGridPos = new GridPosition(x, y, 0);
                PathNode currPathNode = gridSystem.GetGridObjectFromGridPos(currGridPos);

                //for some reason set path cost to max int
                currPathNode.SetGCost(int.MaxValue);
                //..set heuristic to 0
                currPathNode.SetHCost(0);
                currPathNode.CalculateFCost();
                currPathNode.ResetPrevious();
            }
        }

        //setting costs for first node
        startPathNode.SetGCost(0);
        startPathNode.SetHCost(CalculateDistance(startGridPos,endGridPos));
        startPathNode.CalculateFCost();

        while (openPathNodes.Count > 0)
        {
            PathNode currPathNode = GetLowestFCostPathNode(openPathNodes);
            if (currPathNode == endPathNode)
            {
                pathDistance = currPathNode.GetFCost();
                return CalculatePath(endPathNode);
            }

            openPathNodes.Remove(currPathNode);
            closedPathNodes.Add(currPathNode);

            //go through neighbours
            List<PathNode> neighboursList = GetNeighbourList(currPathNode, closedPathNodes);
            foreach (PathNode neighbourNode in neighboursList)
            {
                if (closedPathNodes.Contains(neighbourNode))
                {
                    continue;
                }

                int tempGCost = currPathNode.GetGCost() + CalculateDistance(currPathNode.GetGridPosition(),neighbourNode.GetGridPosition());

                if (tempGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetPrevious(currPathNode);
                    neighbourNode.SetGCost(tempGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endPathNode.GetGridPosition()));
                    neighbourNode.CalculateFCost();

                    if (!openPathNodes.Contains(neighbourNode))
                    {
                        openPathNodes.Add(neighbourNode);
                    }
                }
            }
        }

        pathDistance = 0;

        //no path found
        return null;
    }

    //once the path to endNode is found call this method 
    private List<GridPosition> CalculatePath(PathNode endPathNode)
    {
        List<GridPosition> pathNodeList = new List<GridPosition>();
        pathNodeList.Add(endPathNode.GetGridPosition());

        PathNode currentNode = endPathNode;
        while (currentNode != null && currentNode.GetPreviousPathNode()!=null)
        {
            pathNodeList.Add(currentNode.GetPreviousPathNode().GetGridPosition());
            currentNode = currentNode.GetPreviousPathNode();
        }
        
        pathNodeList.Reverse();

        return pathNodeList;
    }

    //pathfinding with custom priority queue, not done
    public List<GridPosition> FindAPath2(GridPosition start, GridPosition end)
    {
        //// Create a priority queue to store the open set of nodes
        //PriorityQueue<PathNode> openSet = new PriorityQueue<PathNode>();

        //// Create a hash set to store the closed set of nodes
        //HashSet<GridPosition> closedSet = new HashSet<GridPosition>();

        //// Create the start node and add it to the open set
        //PathNode startNode = gridSystem.GetGridObjectFromGridPos(start);
        //startNode.SetGCost(0);
        //startNode.SetHCost(Mathf.RoundToInt (Vector3.Distance(LevelGrid.Instance.GetWorldFromGridPosition(start), LevelGrid.Instance.GetWorldFromGridPosition(end))));

        //openSet.Enqueue(startNode, startNode.GetFCost());

        //// Perform the A* search algorithm
        //int iterations = 0;
        //while (openSet.Count > 0 && iterations < MAX_ITERATIONS)
        //{
        //    // Get the node with the lowest total cost from the open set
        //    PathNode currentNode = openSet.Dequeue();

        //    // Check if the current node is the goal node
        //    if (currentNode.GetGridPosition() == end)
        //    {
        //        // Reconstruct the path from the start to the goal node
        //        List<GridPosition> path = new List<GridPosition>();
        //        while (currentNode != null)
        //        {
        //            path.Add(currentNode.GetGridPosition());
        //            currentNode = currentNode.GetPreviousPathNode();
        //        }
        //        path.Reverse(); // Reverse the path so that it starts at the start node
        //        return path;
        //    }

        //    // Add the current node to the closed set
        //    closedSet.Add(currentNode.GetGridPosition());

        //    // Generate the neighboring nodes of the current node
        //   GridPosition[] neighbors = GetNeighbors(currentNode.GetGridPosition());

        //    // Iterate over the neighboring nodes
        //    foreach (GridPosition neighbor in neighbors)
        //    {
        //        // Check if the neighbor is already in the closed set
        //        if (closedSet.Contains(neighbor))
        //        {
        //            continue;
        //        }

        //        // Calculate the cost to reach the neighbor from the current node
        //        int cost = currentNode.GetGCost() + (int)Vector3.Distance(LevelGrid.Instance.GetWorldFromGridPosition(currentNode.GetGridPosition()), LevelGrid.Instance.GetWorldFromGridPosition(neighbor));

        //        // Check if the neighbor is already in the open set
        //        PathNode existingNode = openSet.Find(n => n.GetGridPosition() == neighbor);
        //        if (existingNode != null)
        //        {
        //            // If the cost to reach the neighbor from the current node is lower than its current cost,
        //            // update the neighbor's cost and parent node in the search tree
        //            if (cost < existingNode.GetGCost())
        //            {
        //                existingNode.SetGCost( cost);
        //                existingNode.SetPrevious(currentNode);
        //                openSet.UpdatePriority(existingNode, existingNode.TotalCost());
        //            }
        //        }
        //        else
        //        {
        //            // Calculate the heuristic cost to reach the goal node from the neighbor
        //            int heuristic = (int)Vector3.Distance(neighbor, goal);

        //            // Create a new node for the neighbor and add it to the open set
        //            Node neighborNode = new Node(neighbor, cost, heuristic, currentNode);
        //            openSet.Enqueue(neighborNode, neighborNode.TotalCost());
        //        }
        //    }

        //    // Increment the iteration counter
        //    iterations++;
        //}

        // If the search algorithm terminates without finding a path, return null
        return null;
    }
        //--------- 
    
    //gets the distance of the 2 grid positions
    public int CalculateDistance(GridPosition a, GridPosition b)
    {
        GridPosition gridPositionDistance = a - b;
        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.y);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int yDistance = Mathf.Abs(gridPositionDistance.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return remaining*MOVE_STRAIGHT_COST + Mathf.Min(xDistance,yDistance)*MOVE_DIAGONAL_COST;
    }

    //gets the path node with the lowest cost
    public PathNode GetLowestFCostPathNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostPathNode = pathNodes[0];
        
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodes[i];
            }
        }

        return lowestFCostPathNode;
    }

    //returns list of neighbouring path nodes
    private List<PathNode> GetNeighbourList(PathNode currPathNode, List<PathNode> closedNodes)
    {
        List<PathNode> neighboursList= new List<PathNode>();

        GridPosition gridPosition = currPathNode.GetGridPosition();

        //left
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x - 1, gridPosition.y));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y, closedNodes);
        //left down
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x - 1, gridPosition.y - 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y - 1, closedNodes);
        //left up
        // neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x - 1, gridPosition.y + 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y + 1, closedNodes);
        //right
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x + 1, gridPosition.y));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y, closedNodes);
        //right down
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x + 1, gridPosition.y - 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y - 1, closedNodes);
        //right up
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x + 1, gridPosition.y + 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y + 1, closedNodes);
        //up
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x, gridPosition.y + 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y + 1, closedNodes);
        //down
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x , gridPosition.y - 1));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y - 1, closedNodes);
        //left
        //neighboursList.Add(GetPathNodeOnGridPosition(gridPosition.x - 1, gridPosition.y));
        AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y, closedNodes);

        return neighboursList;
    }

    //sets width, height in number of grid cells of cellSize size, it's called in levelGrid->Start method
    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        //TODO: change later to consider different floors
        gridSystem = new GridSystemSquare<PathNode>(width, height, cellSize, 0, LevelGrid.FLOOR_HEIGHT, (GridSystemSquare<PathNode> GridSystem, GridPosition gp) => new PathNode(gp));
        
        
        //gridSystem.CreateDebugObjects(gridDebugObject);

        //test
        //GetPathNodeOnGridPosition(1, 0).SetIsWalkable(false);
        //GetPathNodeOnGridPosition(1, 1).SetIsWalkable(false);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //TODO: change later to consider different floors
                GridPosition tempGridPos = new GridPosition(i, j, 0);
                Vector3 tempGridPosWorldPos = LevelGrid.Instance.GetWorldFromGridPosition(tempGridPos);

                float raycastOffsetDistance = 5f;

                //raycasting doesn't work from the inside of a collider, so we're offseting the starting point; starting point is under the grid position; this could be avoided by turning on the "Queries Hit Backfaces" option in proj settings>physics
                if(Physics.Raycast(tempGridPosWorldPos + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetPathNodeOnGridPosition(i, j).SetIsWalkable(false);
                }
            }
        }
    }


    private void AddPathNodeIfValidToList(List<PathNode> neighboursList, int x, int y, List<PathNode> closedNodes)
    {
        //TODO: change later to consider different floors
        GridPosition gridPos = new GridPosition(x, y, 0);
        if(gridSystem.IsValidGridPosition(gridPos) && gridSystem.GetGridObjectFromGridPos(gridPos).GetIsWalkable())
        {
            if (!LevelGrid.Instance.IsGridPositionOccupied(gridPos))
            {
                neighboursList.Add(gridSystem.GetGridObjectFromGridPos(gridPos));
            }
            else
            {
                closedNodes.Add(gridSystem.GetGridObjectFromGridPos(gridPos));
            }            
        }        
    }

    private PathNode GetPathNodeOnGridPosition(int x, int y)
    {
        //TODO: change later to consider different floors
        return gridSystem.GetGridObjectFromGridPos(new GridPosition(x, y, 0));
    }

    public bool IsGridPositionWalkable(GridPosition gridPos)
    {
        return GetPathNodeOnGridPosition(gridPos.x, gridPos.y).GetIsWalkable();
    }

    public void SetGridPositionWalkable(GridPosition gridPos, bool isWalkableVal)
    {
        GetPathNodeOnGridPosition(gridPos.x, gridPos.y).SetIsWalkable(isWalkableVal);
    }

    public bool HasAPath(GridPosition a, GridPosition b)
    {
        return FindAPath(a, b, out int pathDistance) != null;
    }

    public int GetPathDistanceCost(GridPosition a, GridPosition b)
    {
        FindAPath(a, b, out int pathDistance);
        return pathDistance;
    }

}

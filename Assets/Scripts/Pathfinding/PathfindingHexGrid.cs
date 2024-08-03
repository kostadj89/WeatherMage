using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathfindingHexGrid : MonoBehaviour
{
    private const int MAX_ITERATIONS = 1000;
    private const int MOVE_STRAIGHT_COST = 10;  

    private int width;
    private int height;
    private float cellSize;
    private int numberOfFloors;

    private List<GridSystemHex<PathNode>> gridSystemList;

    [SerializeField]
    private Transform gridDebugObject;
    [SerializeField]
    private LayerMask obstaclesLayerMask;

    public static PathfindingHexGrid Instance;

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
        GridSystemHex<PathNode> startingGridSystemHex = GedGridSystemOnTheFloor(startGridPos.floor);
        GridSystemHex<PathNode> targetGridSystemHex = GedGridSystemOnTheFloor(endGridPos.floor);
       //get start node from grid pos
       PathNode startPathNode = startingGridSystemHex.GetGridObjectFromGridPos(startGridPos);

        //get end node from grid pos
        PathNode endPathNode = targetGridSystemHex.GetGridObjectFromGridPos(endGridPos);

        //add start node to opened list first 
        openPathNodes.Add(startPathNode);

        //initialization
        //added temp index for the floor
        for (int x = 0; x < gridSystemList[0].GetWidth(); x++)
        {
            //added temp index for the floor
            for (int y = 0; y < gridSystemList[0].GetHeight(); y++)
            {

                //TODO: change later to consider different floors
                GridPosition currGridPos = new GridPosition(x, y, 0);
                //added temp index for the floor
                PathNode currPathNode = gridSystemList[0].GetGridObjectFromGridPos(currGridPos);

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
        startPathNode.SetHCost(CalculateHeuristicDistance(startGridPos, endGridPos));
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

                int tempGCost = currPathNode.GetGCost() + MOVE_STRAIGHT_COST;

                if (tempGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetPrevious(currPathNode);
                    neighbourNode.SetGCost(tempGCost);
                    neighbourNode.SetHCost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endPathNode.GetGridPosition()));
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
        while (currentNode != null && currentNode.GetPreviousPathNode() != null)
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
    public int CalculateHeuristicDistance(GridPosition a, GridPosition b)
    {
        return Mathf.RoundToInt(MOVE_STRAIGHT_COST * Vector3.Distance(LevelGrid.Instance.GetWorldFromGridPosition(a), LevelGrid.Instance.GetWorldFromGridPosition(b)));

        //GridPosition gridPositionDistance = a - b;
        //int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.y);
        //int xDistance = Mathf.Abs(gridPositionDistance.x);
        //int yDistance = Mathf.Abs(gridPositionDistance.y);
        //int remaining = Mathf.Abs(xDistance - yDistance);
        //return remaining * MOVE_STRAIGHT_COST + Mathf.Min(xDistance, yDistance) * MOVE_STRAIGHT_COST;
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
        List<PathNode> neighboursList = new List<PathNode>();

        GridPosition gridPosition = currPathNode.GetGridPosition();

        //left
        AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y, closedNodes);
        //right
        AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y, closedNodes);

        if (gridPosition.x%2==0)
        {
            //left down
            AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y - 1, closedNodes);
            //left up
            AddPathNodeIfValidToList(neighboursList, gridPosition.x - 1, gridPosition.y + 1, closedNodes);

            //right down
            AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y - 1, closedNodes);
            //right up
            AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y + 1, closedNodes);
        }
        else
        {
            //left down
            AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y - 1, closedNodes);
            //left up
            AddPathNodeIfValidToList(neighboursList, gridPosition.x, gridPosition.y + 1, closedNodes);

            //right down
            AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y - 1, closedNodes);
            //right up
            AddPathNodeIfValidToList(neighboursList, gridPosition.x + 1, gridPosition.y + 1, closedNodes);
        }

        return neighboursList;
    }

    //sets width, height in number of grid cells of cellSize size, it's called in levelGrid->Start method
    public void Setup(int width, int height, float cellSize, int numberOfFloors)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.numberOfFloors = numberOfFloors;
        
        this.gridSystemList = new List<GridSystemHex<PathNode>>();

        for (int i = 0; i < numberOfFloors; i++)
        {
            //TODO: change later to consider different floors
            GridSystemHex<PathNode> tempPathfindingGridSystem = new GridSystemHex<PathNode>(width, height, cellSize, i, LevelGrid.FLOOR_HEIGHT, (GridSystemHex<PathNode> GridSystem, GridPosition gp) => new PathNode(gp));
            gridSystemList.Add(tempPathfindingGridSystem);
        }



        //gridSystem.CreateDebugObjects(gridDebugObject);

        //test
        //GetPathNodeOnGridPosition(1, 0).SetIsWalkable(false);
        //GetPathNodeOnGridPosition(1, 1).SetIsWalkable(false);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for(int k = 0; k < numberOfFloors; k++)
                {
                    //TODO: change later to consider different floors
                    GridPosition tempGridPos = new GridPosition(i, j, k);               
                
                    Vector3 tempGridPosWorldPos = LevelGrid.Instance.GetWorldFromGridPosition(tempGridPos);

                    //we've changed this from 5 to 1 so that it doesn't get tiles from different floor since the height of the floor is around
                    float raycastOffsetDistance = 1f;

                    //raycasting doesn't work from the inside of a collider, so we're offseting the starting point; starting point is under the grid position; this could be avoided by turning on the "Queries Hit Backfaces" option in proj settings>physics
                    if (Physics.Raycast(tempGridPosWorldPos + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                    {
                        GetPathNodeOnGridPosition(i, j, k).SetIsWalkable(false);
                    }
                }
            }
        }
    }


    private void AddPathNodeIfValidToList(List<PathNode> neighboursList, int x, int y, List<PathNode> closedNodes)
    {

        //TODO: change later to consider different floors
        GridPosition gridPos = new GridPosition(x, y, 0);
        //added temp index for the floor
        if (gridSystemList[0].IsValidGridPosition(gridPos) && gridSystemList[0].GetGridObjectFromGridPos(gridPos).GetIsWalkable())
        {
            if (!LevelGrid.Instance.IsGridPositionOccupied(gridPos))
            {
                //added temp index for the floor
                neighboursList.Add(gridSystemList[0].GetGridObjectFromGridPos(gridPos));
            }
            else
            {
                //added temp index for the floor
                closedNodes.Add(gridSystemList[0].GetGridObjectFromGridPos(gridPos));
            }
        }
    }
    private GridSystemHex<PathNode> GedGridSystemOnTheFloor(int floor)
    {
        return gridSystemList[floor];
    }

    private PathNode GetPathNodeOnGridPosition(int x, int y, int floor)
    {
        //TODO: change later to consider different floors
        return GedGridSystemOnTheFloor(floor).GetGridObjectFromGridPos(new GridPosition(x, y, floor));
    }

    public bool IsGridPositionWalkable(GridPosition gridPos)
    {
        return GetPathNodeOnGridPosition(gridPos.x, gridPos.y, gridPos.floor).GetIsWalkable();
    }

    public void SetGridPositionWalkable(GridPosition gridPos, bool isWalkableVal)
    {
        GetPathNodeOnGridPosition(gridPos.x, gridPos.y, gridPos.floor).SetIsWalkable(isWalkableVal);
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

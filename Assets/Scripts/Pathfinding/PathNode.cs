using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    //cost from start to this node
    private int gCost;
    //heuristic from that node to finish
    private int hCost;
    //g+h
    private int fCost;

    private bool isWalkable = true;

    private PathNode previousPathNode;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return "Node: "+gridPosition.ToString();
    }

    public void CalculateFCost()
    {
        fCost =  gCost + hCost;
    }

    public int GetFCost()
    {
        return fCost;
    }

    public int GetGCost()
    {
        return gCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public void SetGCost(int value)
    {
        gCost = value;
    }

    public void SetHCost(int value)
    {
        hCost = value;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public PathNode GetPreviousPathNode()
    {
        return previousPathNode;
    }

    public void SetPrevious(PathNode pathNode)
    {
        previousPathNode = pathNode;
    }

    public void ResetPrevious()
    {
        previousPathNode = null;
    }

    public bool GetIsWalkable()
    { 
        return isWalkable; 
    }

    public void SetIsWalkable(bool value)
    {
        isWalkable = value;
    }
}

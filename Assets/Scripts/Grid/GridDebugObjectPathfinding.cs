using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GridDebugObjectPathfinding : GridDebugObject
{
    [SerializeField] TextMeshPro fCost;
    [SerializeField] TextMeshPro gCost;
    [SerializeField] TextMeshPro hCost;
    [SerializeField] SpriteRenderer walkableCellSpriteRenderer;

    private PathNode pathNode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        gCost.text = pathNode.GetGCost().ToString();
        hCost.text = pathNode.GetHCost().ToString();
        fCost.text = pathNode.GetFCost().ToString();

        //walkableCellSpriteRenderer.color = pathNode.GetIsWalkable()? Color.green.WithAlpha(0.01f) : Color.red.WithAlpha(0.01f);        
    }

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = gridObject as PathNode;
    }
}

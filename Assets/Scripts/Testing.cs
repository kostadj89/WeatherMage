using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField]
    private Unit Unit;

    [SerializeField]
    private GridSystemVisual GridSystemVisual;

    private GridPosition hoverOverGridPosition;
    private GridPosition previousHoverOverGridPosition;
    // Start is called before the first frame update
    void Start()
    {
        previousHoverOverGridPosition = new GridPosition(-100, -100);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N) && Unit != null) 
        //{
        //    //Unit.GetComponent<SpinAction>().ToggleSpin();
        //    //GridSystemVisual.ShowGridPositions(Unit.GetMoveAction().GetValidGridPositionsForAction());
        //}
        hoverOverGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (LevelGrid.Instance.IsValidGridPosition(hoverOverGridPosition) && hoverOverGridPosition!=previousHoverOverGridPosition)
        {
            Debug.Log(hoverOverGridPosition);
            previousHoverOverGridPosition = hoverOverGridPosition;
        }
        
    }
}

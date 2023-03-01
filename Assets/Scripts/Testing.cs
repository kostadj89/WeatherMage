using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField]
    private Unit Unit;

    [SerializeField]
    private GridSystemVisual GridSystemVisual;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && Unit != null) 
        {
            //Unit.GetComponent<SpinAction>().ToggleSpin();
            //GridSystemVisual.ShowReachableGridPositions(Unit.GetMoveAction().GetValidGridPositionsForAction());
        }

        //Debug.Log(gridSystem.GetGridPosFromVector(MouseWorld.GetPosition()));
    }
}

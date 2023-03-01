using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject gridObject;

    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private float fontSize;

    private void Awake()
    {
        if (textMeshPro!=null)
        {
            textMeshPro.fontSize = fontSize;
        }
    }

    public void SetGridObject(GridObject gridObject)
    { 
        this.gridObject = gridObject;
        textMeshPro.text = this.gridObject.ToString();
    }

    private void Update()
    {
        textMeshPro.text = this.gridObject.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private object gridObject;

    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private float fontSize;

    private void Awake()
    {
        if (textMeshPro != null)
        {
            textMeshPro.fontSize = fontSize;
        }
    }

    public virtual void SetGridObject(object gridObject)
    { 
        this.gridObject = gridObject;
        textMeshPro.text = this.gridObject.ToString();
    }

    protected virtual void Update()
    {
        textMeshPro.text = this.gridObject.ToString();
    }
}

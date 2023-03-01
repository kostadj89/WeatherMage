using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyObjectUI : MonoBehaviour
{
    private void Awake()
    {
        //UnitActionSystem.Instance.OnActionBusy += SetBusyUI;
        //UnitActionSystem.Instance.OnActionFree += ClearBusyUI;
        UnitActionSystem.Instance.OnBusyChanged += ChangeBusyUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetBusyUI(object sender, EventArgs eventArgs)
    {
        gameObject.SetActive(true);
    }
    private void ClearBusyUI(object sender, EventArgs e)
    {
        gameObject.SetActive(false); 
    }
    private void ChangeBusyUI(object sender, bool e)
    {
        gameObject.SetActive(e);
    }
}

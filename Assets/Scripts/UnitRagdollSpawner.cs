using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform ragdollUnit;
    [SerializeField]
    private Transform ARootBone;
    private HealthSystem healthSystem;
    // Start is called before the first frame update
    void Start()
    {
        healthSystem = transform.GetComponent<HealthSystem>();
        healthSystem.OnDying += OnDying_UnitRagdollSpawner;
    }

    private void OnDying_UnitRagdollSpawner(object sender, EventArgs e)
    {
        Transform transformRagdoll = Instantiate(ragdollUnit, transform.position, transform.rotation);
        transformRagdoll.GetComponent<UnitRagdollSetup>().Setup(ARootBone);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

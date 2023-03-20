using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeManager : MonoBehaviour
{
    [SerializeField]
    private ScreenShake screenShaker;
    // Start is called before the first frame update
    void Start()
    {
        SpellBoltProjectile.OnAnyProjectileDestroyed += OnAnyProjectileDestroyed_ScreenShakeManager;
    }

    private void OnAnyProjectileDestroyed_ScreenShakeManager(object sender, SpellBoltProjectile.OnProjectileDestroyedArgs e)
    {
        screenShaker.Shake(0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
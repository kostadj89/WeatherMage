using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootAction;
using static CustomProjectile;

public class UnitAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator unitAnimator;    

    private void Awake()
    {
        //if(TryGetComponent<Animator>(out Animator animatorOnInstance))
        //{
        //    unitAnimator = animatorOnInstance;
        //}
        //else
        //{
        //    Debug.Log("Unit doesn't have animator assigned");
        //}

        //try to get all possible actions which have some kind of animation to them, and subscribe to their events
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += OnStartMoving_AnimationManager;
            
            if (unitAnimator != null)
            {
                unitAnimator.SetBool("IsRunning", false);
            }
            
            moveAction.OnStopMoving += OnStopMoving_AnimationManager;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnStartShooting += OnStartShooting_AnimationManager;
            //CustomProjectile.OnAnyProjectileDestroyed += OnProjectileDestroyed_UnitAnimator;
            //shootAction.OnFireProjectile += OnFire_AnimationManager;
        }
    }

    private void OnFire_AnimationManager(object sender, ShootAction.OnShootEventArgs onShootEventArgs)
    {
        //spellboltTransform = Instantiate(spellboltProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        //Vector3 targetProjectilePos = onShootEventArgs.targetUnit.GetWorldPosition();
        
        ////set projectile height to be the same as the spawn height
        //targetProjectilePos.y += projectileSpawnPoint.position.y;

        //CustomProjectile sp = spellboltTransform.GetComponent<CustomProjectile>();
        //sp.Setup(targetProjectilePos);
        //sp.OnProjectileDestroyed += OnProjectileDestroyed_UnitAnimator;
        //sp.SetDamage(onShootEventArgs.damage);
    }    

    private void OnStartShooting_AnimationManager(object sender, EventArgs e)
    {
        if (unitAnimator != null)
        {
            unitAnimator.SetTrigger("TakeAShot");
        }              
    }

    private void OnStopMoving_AnimationManager(object sender, EventArgs e)
    {
        if (unitAnimator != null)
        {
            unitAnimator.SetBool("IsRunning", false);
        }
       
    }

    private void OnStartMoving_AnimationManager(object sender, EventArgs e)
    {
        if (unitAnimator!= null)
        {
            unitAnimator.SetBool("IsRunning", true);
        }
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

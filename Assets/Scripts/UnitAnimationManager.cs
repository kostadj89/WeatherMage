using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootAction;
using static SpellBoltProjectile;

public class UnitAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator unitAnimator;

    [SerializeField]
    private Transform spellboltProjectilePrefab;

    [SerializeField]
    private Transform projectileSpawnPoint;

    private Transform spellboltTransform;

    private void Awake()
    {
        //try to get all possible actions which have some kind of animation to them, and subscribe to their events
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += OnStartMoving_AnimationManager;
            unitAnimator.SetBool("IsRunning", false);
            moveAction.OnStopMoving += OnStopMoving_AnimationManager;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnStartShooting += OnStartShooting_AnimationManager;
            shootAction.OnFire += OnFire_AnimationManager;
        }
    }

    private void OnFire_AnimationManager(object sender, ShootAction.OnShootEventArgs onShootEventArgs)
    {
        spellboltTransform = Instantiate(spellboltProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        Vector3 targetProjectilePos = onShootEventArgs.targetUnit.GetWorldPosition();
        
        //set projectile height to be the same as the spawn height
        targetProjectilePos.y += projectileSpawnPoint.position.y;

        SpellBoltProjectile sp = spellboltTransform.GetComponent<SpellBoltProjectile>();
        sp.Setup(targetProjectilePos);
        sp.OnProjectileDestroyed += OnProjectileDestroyed_UnitAnimator;
        sp.SetDamage(onShootEventArgs.damage);
    }

    private void OnProjectileDestroyed_UnitAnimator(object sender, OnProjectileDestroyedArgs e)
    {
      Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(LevelGrid.Instance.GetGridPosition(e.targetPosition));
        targetUnit.TakeDamage(e.damage);
    }

    private void OnStartShooting_AnimationManager(object sender, EventArgs e)
    {
        unitAnimator.SetTrigger("TakeAShot");        
    }

    private void OnStopMoving_AnimationManager(object sender, EventArgs e)
    {
        unitAnimator.SetBool("IsRunning", false);
    }

    private void OnStartMoving_AnimationManager(object sender, EventArgs e)
    {
        unitAnimator.SetBool("IsRunning", true);
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

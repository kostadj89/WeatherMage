using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellBoltProjectile : MonoBehaviour
{
    private const float TRESHOLD = 0.1f;
    private Vector3 targetPosition;
    private float tresholdDistance = TRESHOLD;
    private int damage;

    [SerializeField]
    private float projectileMovSpeed=200f;
    [SerializeField]
    private TrailRenderer trailRenderer;
    [SerializeField]
    private Transform projectileHitVFXPrefab;
    [SerializeField]
    private bool projecileTailZigZag = true;
    [SerializeField]
    private float projectileHightOffset;
    [SerializeField]
    private float projectileTailDeformationPower;

    // added for any projectile destroyed event
    public static event EventHandler<OnProjectileDestroyedArgs> OnAnyProjectileDestroyed;
    public event EventHandler<OnProjectileDestroyedArgs> OnProjectileDestroyed;
    public class OnProjectileDestroyedArgs : EventArgs
    {
        public int damage;
        public Vector3 targetPosition;
    }

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    internal void SetDamage(int damage)
    {
        this.damage= damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 projectileMoveDirection = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        //added so we coud get zig-zag movement
        Vector3 heightOffsetVector = projecileTailZigZag ? new Vector3(0, UnityEngine.Random.Range(-projectileHightOffset, projectileHightOffset) + Mathf.Sin(distanceBeforeMoving * projectileTailDeformationPower), 0) : new Vector3(0,0,0);

        transform.position += (projectileMoveDirection + heightOffsetVector) * projectileMovSpeed * Time.deltaTime ;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)//(Vector3.Distance(transform.position,targetPosition)<= tresholdDistance)
        {
            transform.position = targetPosition;
            Instantiate(projectileHitVFXPrefab, targetPosition, Quaternion.identity);
            //unparent trail
            trailRenderer.transform.parent = null;

            //aded for screen shake event but maybe just the static version should be kept
            OnAnyProjectileDestroyed.Invoke(this, new OnProjectileDestroyedArgs { targetPosition = targetPosition, damage = damage });
            OnProjectileDestroyed.Invoke(this, new OnProjectileDestroyedArgs { targetPosition = targetPosition, damage = damage });

            Destroy(gameObject); 
        }
    }
}

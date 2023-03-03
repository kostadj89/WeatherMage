using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellBoltProjectile : MonoBehaviour
{
    private const float TRESHOLD = 0.1f;
    private Vector3 targetPosition;
    private float tresholdDistance = TRESHOLD;

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

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
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
        Vector3 heightOffsetVector = projecileTailZigZag ? new Vector3(0, Random.Range(-projectileHightOffset, projectileHightOffset) + Mathf.Sin(distanceBeforeMoving * projectileTailDeformationPower), 0) : new Vector3(0,0,0);

        transform.position += (projectileMoveDirection + heightOffsetVector) * projectileMovSpeed * Time.deltaTime ;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)//(Vector3.Distance(transform.position,targetPosition)<= tresholdDistance)
        {
            transform.position = targetPosition;
            Instantiate(projectileHitVFXPrefab, targetPosition, Quaternion.identity);
            //unparent trail
            trailRenderer.transform.parent = null;
            Destroy(gameObject); 
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSetup : MonoBehaviour
{
    [SerializeField]
    private Transform ARootBone;

    [SerializeField]
    private float explosionForce = 300f;
    [SerializeField]
    private float explosionRange=10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ARootBone);      

        ApplyExplosionToRagdoll(ARootBone, explosionForce, transform.position, explosionRange);
    }

    private void MatchAllChildTransforms(Transform original,Transform clone)
    {
        foreach (Transform child in original)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }
    
    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
           if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

           ApplyExplosionToRagdoll(child,explosionForce,explosionPosition, explosionRange);
        }
    }
}

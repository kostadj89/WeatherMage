using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;
    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Duplicate ScreenShake instance");
            return;
        }
        
        cinemachineImpulseSource= GetComponent<CinemachineImpulseSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Shake(float intesity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intesity);
    }
}

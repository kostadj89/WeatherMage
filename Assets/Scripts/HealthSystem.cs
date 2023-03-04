using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int health = 100;

    public event EventHandler OnDying;
    public event EventHandler OnTakeDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0)
        {
            health = 0;
        }

        OnTakeDamage?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDying?.Invoke(this,EventArgs.Empty);
    }
}

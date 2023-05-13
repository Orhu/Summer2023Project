using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth = 5;
    private int currentHealth;
    public UnityEvent<float> OnDamageTaken;
    public int CurrentHealth {
        get { return currentHealth; } 
        set
        {
            currentHealth = value;
            
            OnDamageTaken?.Invoke(currentHealth);
            
            if (value <= 0)
            {
                Destroy(gameObject);
            }
            
        }
    }

    private void Start()
    {
        currentHealth = MaxHealth;
    }
}

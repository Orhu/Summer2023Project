using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 5;
    private int currentHealth;
    public int CurrentHealth {
        get { return currentHealth; } 
        set 
        {
            if (value <= 0)
            {
                Destroy(gameObject);
            }
            currentHealth = value;
        }
    }

    private void Start()
    {
        currentHealth = MaxHealth;
    }
}

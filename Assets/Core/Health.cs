using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 5;
    int currentHealth;
    public int CurrentHealth {
        get { return CurrentHealth; } 
        set 
        {
            if (value <= 0)
            {
                Destroy(gameObject);
            }
            CurrentHealth = value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 5;
    [System.NonSerialized]
    public int currentHealth;

    delegate void AttackNotification(Attack attack);
    AttackNotification onAttacked;

    void Start()
    {
        currentHealth = MaxHealth;
    }

    public void Attack(Attack attack)
    {
        currentHealth -= attack.damage;
        onAttacked(attack);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public int damage;
    [System.NonSerialized]
    public GameObject causer;

    public Attack(Attack attack, GameObject causer)
    {
        damage = attack.damage;
        this.causer = causer;
    }
    // Status effects go here
    // Knockback goes here
}

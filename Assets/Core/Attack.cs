using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data of an attack
/// </summary>
[System.Serializable]
public class Attack
{
    [Tooltip("The damage this attack deals")]
    public int damage;
    // The causer of this attack.
    [System.NonSerialized]
    public GameObject causer;

    /// <summary>
    /// Copy Constructor for an attack.
    /// </summary>
    /// <param name="attack"> The attack to copy </param>
    /// <param name="causer"> The new causer </param>
    public Attack(Attack attack, GameObject causer)
    {
        damage = attack.damage;
        this.causer = causer;
    }
    // Status effects go here
    // Knockback goes here
}

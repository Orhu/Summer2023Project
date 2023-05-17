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
    [Tooltip("The status effects to apply when this is received")]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();


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


    public Attack(int damage, GameObject causer)
    {
        this.damage = damage;
        this.causer = causer;
    }

    public static Attack operator *(Attack attack, int integer)
      => new Attack(attack.damage * integer, attack.causer);
}
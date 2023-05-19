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
    public Object causer;

    /// <summary>
    /// Copy Constructor for an attack.
    /// </summary>
    /// <param name="attack"> The attack to copy </param>
    /// <param name="causer"> The new causer </param>
    public Attack(Attack attack, Object causer)
    {
        damage = attack.damage;
        statusEffects = attack.statusEffects;
        this.causer = causer;
    }
    // Status effects go here
    // Knockback goes here

    /// <summary>
    /// Create a new damaging attack.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="causer"> The causer of the damage </param>
    public Attack(int damage, Object causer)
    {
        this.damage = damage;
        this.causer = causer;
    }

    /// <summary>
    /// Create a new damaging attack.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="causer"> The causer of the damage </param>
    public Attack(int damage, List<StatusEffect> statusEffects, Object causer) : this(damage, causer)
    {
        this.statusEffects = statusEffects;
    }

    /// <summary>
    /// Multiplies an attack so it is applied a certain number of times.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The number of times to apply it. </param>
    /// <returns> The multiplied attack </returns>
    public static Attack operator *(Attack attack, int integer)
    {
        if (integer > 1)
        {
            List<StatusEffect> newStatusEffects = new List<StatusEffect>(attack.statusEffects.Count * integer);
            for (int i = 0; i < integer; i++)
            {
                foreach (StatusEffect statusEffect in attack.statusEffects)
                {
                    newStatusEffects.Add(statusEffect);
                }
            }
            return new Attack(attack.damage * integer, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new Attack(attack, attack.causer);
    }
}
using CardSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data of an attack
/// </summary>
[System.Serializable]
public class AttackData
{
    [Tooltip("The damage this attack deals")]
    public int damage;
    [Tooltip("The status effects to apply when this is received")]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    [Tooltip("The actions played by the hit game object.")]
    public List<Action> hitActions = new List<Action>();


    // The causer of this attack.
    [System.NonSerialized]
    public Object causer;

    /// <summary>
    /// Copy Constructor for an attack.
    /// </summary>
    /// <param name="attack"> The attack to copy </param>
    /// <param name="causer"> The new causer </param>
    public AttackData(AttackData attack, Object causer)
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
    public AttackData(int damage, Object causer)
    {
        this.damage = damage;
        this.causer = causer;
    }

    /// <summary>
    /// Create a new damaging attack.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="causer"> The causer of the damage </param>
    public AttackData(int damage, List<StatusEffect> statusEffects, Object causer) : this(damage, causer)
    {
        this.statusEffects = statusEffects;
    }

    /// <summary>
    /// Multiplies an attack so it is applied a certain number of times.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The number of times to apply it. </param>
    /// <returns> The multiplied attack </returns>
    public static AttackData operator *(AttackData attack, int integer)
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
            return new AttackData(attack.damage * integer, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new AttackData(attack, attack.causer);
    }

    /// <summary>
    /// Reverses the multiplication of an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The number of times it was applied. </param>
    /// <returns> The divided attack </returns>
    public static AttackData operator /(AttackData attack, int integer)
    {
        if (integer > 1)
        {
            List<StatusEffect> newStatusEffects = new List<StatusEffect>(attack.statusEffects.Count / integer);
            for (int i = 0; i < newStatusEffects.Count; i++)
            {
                foreach (StatusEffect statusEffect in attack.statusEffects)
                {
                    newStatusEffects.Add(statusEffect);
                }
            }
            return new AttackData(attack.damage / integer, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new AttackData(attack, attack.causer);
    }

    /// <summary>
    /// Adds damage to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The damage to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator +(AttackData attack, int integer)
    {
        return new AttackData(attack.damage + integer, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Removes damage from an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The damage to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator -(AttackData attack, int integer)
    {
        return new AttackData(attack.damage - integer, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Adds status effects to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="effects"> The status effects to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator +(AttackData attack, List<StatusEffect> effects)
    {
        effects.AddRange(attack.statusEffects);
        return new AttackData(attack.damage, effects, attack.causer);
    }

    /// <summary>
    /// Removes status effects to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="effects"> The status effects to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator -(AttackData attack, List<StatusEffect> effects)
    {
        foreach (StatusEffect statusEffect in attack.statusEffects)
        {
            effects.Remove(statusEffect);
        }
        return new AttackData(attack.damage, effects, attack.causer);
    }
}
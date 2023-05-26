using CardSystem;
using Skaillz.EditInline;
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
    [Tooltip("The type of damage that will be applied")]
    public DamageType damageType;
    [Tooltip("The number of tiles that hit objects will be pushed back")]
    public float knockback = 0f;
    [EditInline]
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
    public AttackData(AttackData attack, Object causer)
    {
        damage = attack.damage;
        damageType = attack.damageType;
        knockback = attack.knockback;
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
    public AttackData(int damage, DamageType damageType, Object causer)
    {
        this.damage = damage;
        this.damageType = damageType;
        this.causer = causer;
    }

    /// <summary>
    /// Create a new damaging attack.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="causer"> The causer of the damage </param>
    public AttackData(int damage, DamageType damageType, float knockback, List<StatusEffect> statusEffects, Object causer) : this(damage, damageType, causer)
    {
        this.statusEffects = statusEffects;
        this.knockback = knockback;
    }


    public AttackData(List<StatusEffect> statusEffects, Object causer)
    {
        this.statusEffects = statusEffects;
        this.causer = causer;
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
            return new AttackData(attack.damage * integer, attack.damageType, attack.knockback * integer, new List<StatusEffect>(newStatusEffects), attack.causer);
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
            return new AttackData(attack.damage / integer, attack.damageType, attack.knockback / integer, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new AttackData(attack, attack.causer);
    }

    /// <summary>
    /// Adds damage to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The damage to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator +(AttackData attack, int damage)
    {
        return new AttackData(attack.damage + damage, attack.damageType, attack.knockback, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Removes damage from an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The damage to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator -(AttackData attack, int damage)
    {
        return new AttackData(attack.damage - damage, attack.damageType, attack.knockback, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Adds knockback to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="knockback"> The damage to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator +(AttackData attack, float knockback)
    {
        return new AttackData(attack.damage, attack.damageType, attack.knockback + knockback, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Removes knockback from an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="knockback"> The damage to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static AttackData operator -(AttackData attack, float knockback)
    {
        return new AttackData(attack.damage, attack.damageType, attack.knockback - knockback, attack.statusEffects, attack.causer);
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
        return new AttackData(attack.damage, attack.damageType, attack.knockback, effects, attack.causer);
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
        return new AttackData(attack.damage, attack.damageType, attack.knockback, effects, attack.causer);
    }

    public enum DamageType
    {
        Physical,
        Special,
    }
}
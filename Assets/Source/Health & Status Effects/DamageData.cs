using Skaillz.EditInline;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data relating to a single damage event.
/// </summary>
[System.Serializable]
public class DamageData
{
    [Tooltip("The damage this attack deals")]
    public int damage;
    [Tooltip("The type of damage that will be applied")]
    public DamageType damageType;
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
    public DamageData(DamageData attack, Object causer)
    {
        damage = attack.damage;
        damageType = attack.damageType;
        statusEffects = attack.statusEffects;
        this.causer = causer;
    }

    /// <summary>
    /// Create a new damage data.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="damageType"> The type of damage dealt. </param>
    /// <param name="causer"> The causer of the damage </param>
    public DamageData(int damage, DamageType damageType, Object causer)
    {
        this.damage = damage;
        this.damageType = damageType;
        this.causer = causer;
    }

    /// <summary>
    /// Create a new damage data.
    /// </summary>
    /// <param name="damage"> The damage it will deal. </param>
    /// <param name="damageType"> The type of damage dealt. </param>
    /// <param name="statusEffects"> The status effects applied. </param>
    /// <param name="causer"> The causer of the damage </param>
    public DamageData(int damage, DamageType damageType, List<StatusEffect> statusEffects, Object causer) : this(damage, damageType, causer)
    {
        this.statusEffects = statusEffects;
    }

    /// <summary>
    /// Create a new damage data.
    /// </summary>
    /// <param name="statusEffects"> The status effects applied. </param>
    /// <param name="causer"> The causer of the damage. </param>
    public DamageData(List<StatusEffect> statusEffects, Object causer)
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
    public static DamageData operator *(DamageData attack, int integer)
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
            return new DamageData(attack.damage * integer, attack.damageType, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new DamageData(attack, attack.causer);
    }

    /// <summary>
    /// Reverses the multiplication of an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The number of times it was applied. </param>
    /// <returns> The divided attack </returns>
    public static DamageData operator /(DamageData attack, int integer)
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
            return new DamageData(attack.damage / integer, attack.damageType, new List<StatusEffect>(newStatusEffects), attack.causer);
        }

        return new DamageData(attack, attack.causer);
    }

    /// <summary>
    /// Adds damage to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="damage"> The damage to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static DamageData operator +(DamageData attack, int damage)
    {
        return new DamageData(attack.damage + damage, attack.damageType, attack.statusEffects, attack.causer);
    }
    /// <summary>
    /// Removes damage from an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="integer"> The damage to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static DamageData operator -(DamageData attack, int damage)
    {
        return new DamageData(attack.damage - damage, attack.damageType, attack.statusEffects, attack.causer);
    }

    /// <summary>
    /// Adds status effects to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="effects"> The status effects to add. </param>
    /// <returns> A copy of the modified attack </returns>
    public static DamageData operator +(DamageData attack, List<StatusEffect> effects)
    {
        effects.AddRange(attack.statusEffects);
        return new DamageData(attack.damage, attack.damageType, effects, attack.causer);
    }

    /// <summary>
    /// Removes status effects to an attack.
    /// </summary>
    /// <param name="attack"> The original attack. </param>
    /// <param name="effects"> The status effects to remove. </param>
    /// <returns> A copy of the modified attack </returns>
    public static DamageData operator -(DamageData attack, List<StatusEffect> effects)
    {
        foreach (StatusEffect statusEffect in attack.statusEffects)
        {
            effects.Remove(statusEffect);
        }
        return new DamageData(attack.damage, attack.damageType, effects, attack.causer);
    }

    public enum DamageType
    {
        Physical,
        Special,
    }
}
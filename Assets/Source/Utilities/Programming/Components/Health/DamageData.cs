using Skaillz.EditInline;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
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

        [Tooltip("The status effects to apply when this is received")]
        [EditInline]
        public List<StatusEffect> statusEffects = new List<StatusEffect>();

        [Tooltip("Makes this cause invincibility if it does no damage and not cause invincibility if it does.")]
        public bool invertInvincibility = false;

        [Tooltip("Whether or not to not create a damage number prefab on hit.")]
        public bool dontShowDamageNumber = false;

        //[Tooltip("Whether or not to show more damage numbers after first hit on a given enemy")]
        // public bool repeatedDamageNumbers = true;

        // The causer of this attack.
        [System.NonSerialized]
        public GameObject causer;

        /// <summary>
        /// Copy Constructor for an attack.
        /// </summary>
        /// <param name="attack"> The attack to copy </param>
        /// <param name="causer"> The new causer </param>
        public DamageData(DamageData attack, GameObject causer)
        {
            damage = attack.damage;
            damageType = attack.damageType;
            statusEffects = new List<StatusEffect>(attack.statusEffects);
            invertInvincibility = attack.invertInvincibility;
            dontShowDamageNumber = attack.dontShowDamageNumber;
            this.causer = causer;
        }

        /// <summary>
        /// Create a new damage data.
        /// </summary>
        /// <param name="damage"> The damage it will deal. </param>
        /// <param name="damageType"> The type of damage dealt. </param>
        /// <param name="causer"> The causer of the damage </param>
        /// <param name="invertInvincibility"> Whether or not this should cause invincibility if it does no damage and not cause invincibility if it does. </param>
        /// <param name="dontShowDamageNumber"> Whether or not to create a damage number prefab on hit. </param>
        public DamageData(int damage, DamageType damageType, GameObject causer, bool invertInvincibility = false, bool dontShowDamageNumber = false)
        {
            this.damage = damage;
            this.damageType = damageType;
            this.causer = causer;
            this.invertInvincibility = invertInvincibility;
            this.dontShowDamageNumber = dontShowDamageNumber;
        }

        /// <summary>
        /// Create a new damage data.
        /// </summary>
        /// <param name="damage"> The damage it will deal. </param>
        /// <param name="damageType"> The type of damage dealt. </param>
        /// <param name="statusEffects"> The status effects applied. </param>
        /// <param name="causer"> The causer of the damage </param>
        /// <param name="invertInvincibility"> Whether or not this should cause invincibility if it does no damage and not cause invincibility if it does. </param>
        /// <param name="dontShowDamageNumber"> Whether or not to create a damage number prefab on hit. </param>
        public DamageData(int damage, DamageType damageType, List<StatusEffect> statusEffects, GameObject causer, bool invertInvincibility = false, bool dontShowDamageNumber = false) : this(damage, damageType, causer, invertInvincibility, dontShowDamageNumber)
        {
            this.statusEffects = new List<StatusEffect>(statusEffects);
        }

        /// <summary>
        /// Create a new damage data.
        /// </summary>
        /// <param name="statusEffects"> The status effects applied. </param>
        /// <param name="causer"> The causer of the damage. </param>
        /// <param name="invertInvincibility"> Whether or not this should cause invincibility if it does no damage and not cause invincibility if it does. </param>
        /// <param name="dontShowDamageNumber"> Whether or not to create a damage number prefab on hit. </param>
        public DamageData(List<StatusEffect> statusEffects, GameObject causer, bool invertInvincibility = false, bool dontShowDamageNumber = false)
        {
            this.statusEffects = new List<StatusEffect>(statusEffects);
            this.causer = causer;
            this.invertInvincibility = invertInvincibility;
            this.dontShowDamageNumber = dontShowDamageNumber;
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
            List<StatusEffect> newEffects = new List<StatusEffect>(effects);
            newEffects.AddRange(attack.statusEffects);
            return new DamageData(attack.damage, attack.damageType, newEffects, attack.causer);
        }

        public enum DamageType
        {
            Physical,
            Special,
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> This as a string. </returns>
        public override string ToString()
        {
            string value = "";
            if (damage != 0)
            {
                value += $"{Math.Abs(damage)}\n";
            }

            foreach (StatusEffect effect in statusEffects)
            {
                value += $" + {effect.GetType().Name}";
            }
            return value;
        }
    }
}
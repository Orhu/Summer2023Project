using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>

namespace Attacks
{
    /// <summary>
    /// Makes an attack deal additional damage and status effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddDamage", menuName = "Cards/AttackModifers/Add[Stat]/AddDamage")]
    internal class AddDamage : AttackModifier
    {
        [Tooltip("The additional damage")]
        [SerializeField] private int damage;
        [Tooltip("The additional status effects to apply")]
        [SerializeField] private List<StatusEffect> statusEffects;

        // The projectile this modifies
        public override Projectile ModifiedProjectile
        {
            set
            {
                value.attackData = value.attackData + damage + statusEffects;
            }
        }
    }
}

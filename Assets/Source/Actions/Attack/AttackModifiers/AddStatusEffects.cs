using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>

namespace Attacks
{
    /// <summary>
    /// Makes an attack deal additional status effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddStatusEffects", menuName = "Cards/AttackModifers/Add[Stat]/AddStatusEffects")]
    internal class AddStatusEffects : AttackModifier
    {
        [Tooltip("The additional status effects to apply")]
        [SerializeField] private List<StatusEffect> statusEffects;

        // The projectile this modifies
        public override Projectile ModifiedProjectile
        {
            set
            {
                value.attackData = value.attackData + statusEffects;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>

namespace Attacks
{
    /// <summary>
    /// Makes an attack's projectile last longer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddLifetime", menuName = "Cards/AttackModifers/Add[Stat]/AddLifetime")]
    internal class AddLifetime : AttackModifier
    {
        [Tooltip("The additional lifetime in seconds")]
        [SerializeField] private float lifetime;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.remainingLifetime += lifetime;
            }
        }
    }
}

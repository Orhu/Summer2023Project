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
    [CreateAssetMenu(fileName = "NewAddSpeed", menuName = "Cards/AttackModifers/Add[Stat]/AddSpeed")]
    internal class AddSpeed : AttackModifier
    {
        [Tooltip("The additional initial speed to add in tiles/s.")]
        [SerializeField] private float initialSpeed;
        [Tooltip("The additional acceleration to add in tiles/s^2.")]
        [SerializeField] private float acceleration;
        [Tooltip("The additional max speed to add in tiles/s.")]
        [SerializeField] private float maxSpeed;
        [Tooltip("The additional min speed to add in tiles/s.")]
        [SerializeField] private float minSpeed;

        // The projectile this modifies
        public override Projectile ModifiedProjectile
        {
            set
            {
                value.speed += initialSpeed;
                value.acceleration += acceleration;
                value.maxSpeed += maxSpeed;
                value.minSpeed += minSpeed;
            }
        }
    }
}

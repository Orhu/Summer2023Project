using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// Adds to homing stats of a projectile.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHoming", menuName = "Cards/AttackModifers/Add[Stat]/AddHoming")]
    internal class AddHoming : AttackModifier
    {
        [Tooltip("The homing speed to add.")]
        [SerializeField] private float homingSpeed;
        [Tooltip("The amount of homing time to add.")]
        [SerializeField] private float homingTime;

        // The projectile this modifies
        public override Projectile ModifiedProjectile
        {
            set
            {
                value.homingSpeed += homingSpeed;
                value.remainingHomingTime += homingTime;
            }
        }
    }
}

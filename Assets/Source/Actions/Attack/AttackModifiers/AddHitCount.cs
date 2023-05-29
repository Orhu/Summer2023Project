using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// Adds to the maximum number of times an attack can hit enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHitCount", menuName = "Cards/AttackModifers/Add[Stat]/AddHitCount")]
    internal class AddHitCount : AttackModifier
    {
        [Tooltip("The number additional hits to add")]
        [SerializeField] private int hitCountToAdd;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.remainingHits += hitCountToAdd;
            }
        }
    }
}

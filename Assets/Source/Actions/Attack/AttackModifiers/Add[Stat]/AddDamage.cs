using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack deal additional damage and status effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddDamage", menuName = "Cards/AttackModifers/Add[Stat]/AddDamage")]
    public class AddDamage : AttackModifier
    {
        [Tooltip("The additional damage")]
        [SerializeField] private int damage;
        [Tooltip("The additional status effects to apply")]
        [SerializeField] private List<StatusEffect> statusEffects;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.attackData = value.attackData + damage + statusEffects;
        }
    }
}

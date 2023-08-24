using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack deal additional status effects.
    /// </summary>
    [CreateAssetMenu(menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplyStatusEffects")]
    public class MultiplyStatusEffects : AttackModifier
    {
        [Tooltip("Number of multiples to create for each valid status effect")]
        [SerializeField] private int multiplier = 1;

        [Tooltip("The type of status effect to be multiplied. If no instances of the status effect are found, an instance of this status effect will be added instead.")]
        [SerializeField] private StatusEffect statusToMultiply;

        // Number of multiplied statuses (if 0 at end of Initialize, will place a copy of statusToMultiply status on the projectile instead).
        private int multipleCount = 0; 


        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile attachedProjectile)
        {
            Type statusType = statusToMultiply.GetType();
            List<StatusEffect> originalStatusEffects = new List<StatusEffect>(attachedProjectile.attack.attack.statusEffects);
            for (int i = 1; i < multiplier; i++)
            {
                foreach (StatusEffect statusEffect in originalStatusEffects)
                {
                    if (statusEffect.GetType() == statusType) {
                        multipleCount += 1;
                        attachedProjectile.attackData.statusEffects.Add(statusEffect);
                    }
                }
            }
            if (multipleCount == 0) {
                attachedProjectile.attackData.statusEffects.Add(statusToMultiply);
            } 
        }
    }
}
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
        [Tooltip("The amount the status effect count will be multiplied by.")] [Min(2)]
        [SerializeField] private int damageFactor = 2;


        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile attachedProjectile)
        {
            List<StatusEffect> originalStatusEffects = new List<StatusEffect>(attachedProjectile.attack.attack.statusEffects);
            for (int i = 1; i < damageFactor; i++)
            {
                foreach (StatusEffect statusEffect in originalStatusEffects)
                {
                    attachedProjectile.attackData.statusEffects.Add(statusEffect);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlayActionInheritDamage", menuName = "Cards/AttackModifers/Play Action Inherit Damage")]
    public class PlayActionInheritDamage : PlayAction
    {
        [Tooltip("Whether or not to inherit the damage of the parent.")]
        public bool inheritDamage = true;

        [Tooltip("Whether or not to inherit the status effects of the parent.")]
        public bool inheritStatusEffects = true;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            base.Initialize(value);
            OverrideDamage damgeOverride = CreateInstance<OverrideDamage>();
            damgeOverride.damageData = value.attack.attack;
            damgeOverride.overrideDamage = inheritDamage;
            damgeOverride.overrideStatusEffects = inheritStatusEffects;
            modifiers.Insert(0, damgeOverride);
        }
    }
}
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// An the base for all modifier that effect projectiles
    /// </summary>
    public abstract class AttackModifier : ScriptableObject
    {
        [Tooltip("The first index in the attack sequence to modify")] [Min(0)]
        public int minAttackSequenceIndex = 0;

        [Tooltip("The last index in the attack sequence to modify")] [Min(0)]
        public int maxAttackSequenceIndex = int.MaxValue;

        [Tooltip("The interval on which the range of this modifier's valid indices loops")] [Min(2)]
        public int attackSequenceLoopInterval = int.MaxValue;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public abstract void Initialize(Projectile attachedProjectile);

        [Tooltip("Determines the order in which the modifier is applied to an action based on other modifier's priorities (lower values first)")]
        public int priority = 0;
    }
}

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

        // The projectile this modifies
        public abstract Projectile modifiedProjectile { set; }
    }
}

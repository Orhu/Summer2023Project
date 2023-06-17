using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// An the base for all modifier that effect projectiles
    /// </summary>
    public abstract class AttackModifier : ScriptableObject
    {
        // The projectile this modifies
        public abstract Projectile modifiedProjectile { set; }
    }
}

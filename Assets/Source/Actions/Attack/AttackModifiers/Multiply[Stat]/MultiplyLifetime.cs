using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile last longer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMultiplyLifetime", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplyLifetime")]
    public class MultiplyLifetime : AttackModifier
    {
        [Tooltip("The amount to multiply the lifetime by")]
        [SerializeField] private float lifetimeFactor = 1f;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.remainingLifetime += (lifetimeFactor - 1) * value.attack.lifetime;
        }
    }
}

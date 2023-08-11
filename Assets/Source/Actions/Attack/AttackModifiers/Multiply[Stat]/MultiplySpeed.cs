using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile last longer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMultiplySpeed", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplySpeed")]
    public class MultiplySpeed : AttackModifier
    {
        [Tooltip("The additional initial speed to add in tiles/s.")]
        [SerializeField] private float initialSpeedFactor = 1f;

        [Tooltip("The additional acceleration to add in tiles/s^2.")]
        [SerializeField] private float accelerationFactor = 1f;

        [Tooltip("The additional max speed to add in tiles/s.")]
        [SerializeField] private float maxSpeedFactor = 1f;

        [Tooltip("The additional min speed to add in tiles/s.")]
        [SerializeField] private float minSpeedFactor = 1f;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.speed += (initialSpeedFactor - 1) * value.attack.initialSpeed;
            value.acceleration += (accelerationFactor - 1) * value.attack.acceleration;
            value.maxSpeed += (maxSpeedFactor - 1) * value.attack.maxSpeed;
            value.minSpeed += (minSpeedFactor - 1) * value.attack.minSpeed;
        }
    }
}
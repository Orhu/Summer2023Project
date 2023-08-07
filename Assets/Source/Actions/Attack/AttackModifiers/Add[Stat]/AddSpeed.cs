using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile last longer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddSpeed", menuName = "Cards/AttackModifers/Add[Stat]/AddSpeed")]
    public class AddSpeed : AttackModifier
    {
        [Tooltip("The additional initial speed to add in tiles/s.")]
        [SerializeField] private float initialSpeed;

        [Tooltip("The additional acceleration to add in tiles/s^2.")]
        [SerializeField] private float acceleration;

        [Tooltip("The additional max speed to add in tiles/s.")]
        [SerializeField] private float maxSpeed;

        [Tooltip("The additional min speed to add in tiles/s.")]
        [SerializeField] private float minSpeed;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.speed += initialSpeed;
            value.acceleration += acceleration;
            value.maxSpeed += maxSpeed;
            value.minSpeed += minSpeed;
        }
    }
}
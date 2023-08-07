using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds to homing stats of a projectile.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHoming", menuName = "Cards/AttackModifers/Add[Stat]/AddHoming")]
    public class AddHoming : AttackModifier
    {
        [Tooltip("The percentage of the max speed to add to the acceleration towards the target. 1 = 100%")]
        [SerializeField] private float homingSpeed;

        [Tooltip("The amount of duration that this will home for, in seconds to add.")]
        [SerializeField] private float homingTime;

        [Tooltip("The amount of time to wait before homing begins, in seconds to add.")]
        [SerializeField] private float homingDelay;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.homingSpeed += homingSpeed * value.maxSpeed; // This allows homing to scale appropriately based on the velocity of the projectile
            value.remainingHomingTime += homingTime;
        }
    }
}

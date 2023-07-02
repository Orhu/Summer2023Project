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

        [Tooltip("The number of seconds until the homing speed will be at max velocity")]
        [SerializeField] private float homingTimeToMaxVelocity = 1f;

        [Tooltip("The amount of duration that this will home for, in seconds to add.")]
        [SerializeField] private float homingTime;

        [Tooltip("The amount of time to wait before homing begins, in seconds to add.")]
        [SerializeField] private float homingDelay;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.homingSpeed += homingSpeed * value.maxSpeed; // This allows homing to scale appropriately based on the velocity of the projectile
                value.homingSpeed += value.homingSpeed * homingTimeToMaxVelocity;
                value.remainingHomingTime += homingTime;
            }
        }
    }
}

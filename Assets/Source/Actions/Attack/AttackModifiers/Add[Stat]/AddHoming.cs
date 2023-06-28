using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds to homing stats of a projectile.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHoming", menuName = "Cards/AttackModifers/Add[Stat]/AddHoming")]
    public class AddHoming : AttackModifier
    {
        [Tooltip("The speed in tile/s^2 that projectiles will accelerate towards the closest enemy, to add. 10% = 1")]
        [SerializeField] private float homingSpeed;

        [Tooltip("The amount of duration that this will home for, in seconds to add.")]
        [SerializeField] private float homingTime;

        [Tooltip("The amount of time to wait before homing begins, in seconds to add.")]
        [SerializeField] private float homingDelay;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.homingSpeed += homingSpeed*value.maxSpeed; // This allows homing to scale appropriately based on the velocity of the projectile
                value.remainingHomingTime += homingTime;
            }
        }
    }
}

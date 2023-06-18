using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds to homing stats of a projectile.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHoming", menuName = "Cards/AttackModifers/Add[Stat]/AddHoming")]
    public class AddHoming : AttackModifier
    {
        [Tooltip("The homing speed to add.")]
        [SerializeField] private float homingSpeed;

        [Tooltip("The amount of homing time to add.")]
        [SerializeField] private float homingTime;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.homingSpeed += homingSpeed;
                value.remainingHomingTime += homingTime;
            }
        }
    }
}

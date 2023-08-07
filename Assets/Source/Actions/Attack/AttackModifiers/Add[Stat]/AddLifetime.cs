using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile last longer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddLifetime", menuName = "Cards/AttackModifers/Add[Stat]/AddLifetime")]
    public class AddLifetime : AttackModifier
    {
        [Tooltip("The additional lifetime in seconds")]
        [SerializeField] private float lifetime;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.remainingLifetime += lifetime;
        }
    }
}
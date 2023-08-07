using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds to the maximum number of times an attack can hit enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHitCount", menuName = "Cards/AttackModifers/Add[Stat]/AddHitCount")]
    public class AddHitCount : AttackModifier
    {
        [Tooltip("The number additional hits to add")]
        [SerializeField] private int hitCountToAdd;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.remainingHits += hitCountToAdd;
        }
    }
}


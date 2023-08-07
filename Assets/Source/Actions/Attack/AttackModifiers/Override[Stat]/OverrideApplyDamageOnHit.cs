using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Overrides projectile apply damage on hit.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOverrideApplyDamageOnHit", menuName = "Cards/AttackModifers/Override[Stat]/OverrideApplyDamageOnHit")]
    public class OverrideApplyDamageOnHit : AttackModifier
    {
        [Tooltip("Whether or not this projectile should apply damage on hit.")]
        [SerializeField] private bool newApplyDamageOnHit;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.applyDamageOnHit = newApplyDamageOnHit;
        }
    }
}
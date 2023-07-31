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

        // The projectile this modifies
        public override void Initialize(Projectile value)
        {
            value.applyDamageOnHit = newApplyDamageOnHit;
        }
    }
}
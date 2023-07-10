using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Overrides projectile damage.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOverrideDamage", menuName = "Cards/AttackModifers/Override[Stat]/OverrideDamage")]
    public class OverrideDamage : AttackModifier
    {
        [Tooltip("The new damage to deal.")]
        [SerializeField] private DamageData damageData;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.attackData = damageData;

                if (value.causer != null && value.causer?.GetComponent<IActor>() is IActor causedBy)
                {
                    value.attackData.damage = Mathf.RoundToInt(value.attackData.damage * causedBy.GetDamageMultiplier());
                }
            }
        }
    }
}
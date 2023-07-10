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
        public DamageData damageData;

        [Tooltip("Whether or not to override the damage of the parent.")]
        public bool overrideDamage = true;

        [Tooltip("Whether or not to override the status effects of the parent.")]
        public bool overrideStatusEffects = true;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                if (overrideDamage)
                {
                    value.attackData.damage = damageData.damage;

                    if (value.causer != null && value.causer?.GetComponent<IActor>() is IActor causedBy)
                    {
                        value.attackData.damage = Mathf.RoundToInt(value.attackData.damage * causedBy.GetDamageMultiplier());
                    }
                }
                if (overrideStatusEffects)
                {
                    value.attackData.statusEffects = damageData.statusEffects;
                }
            }
        }
    }
}
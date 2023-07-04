using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack deal additional damage.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMultiplyDamage", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplyDamage")]
    public class MultiplyDamage : AttackModifier
    {
        [Tooltip("The amount damage will be multiplied by.")]
        [SerializeField] private float damageFactor = 1f;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                value.attackData.damage += (int)((damageFactor - 1) * value.attack.attack.damage);
            }
        }
    }
}
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

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.attackData.damage += (int)((damageFactor - 1) * value.attack.attack.damage);
        }
    }
}
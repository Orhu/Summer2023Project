using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that causes this to instkill slowed enemies
    /// </summary>
    [CreateAssetMenu(fileName = "NewShatter", menuName = "Cards/AttackModifers/Shatter")]
    public class Shatter : AttackModifier
    {
        [Tooltip("Enemies with this status effect will be killed.")]
        [SerializeField] private StatusEffect killStatusEffect;

        // The damage to apply on kill.
        private const int INSTAKILL_DAMAGE = 99999;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.onOverlap += Instakill;
        }

        /// <summary>
        /// Applies knockback to the hit collider.
        /// </summary>
        /// <param name="collision"> The collider that was hit. </param>
        private void Instakill(Collider2D collision)
        {
            if (collision.GetComponent<Health>() is Health health)
            {
                if (!health.HasStatusEffect(killStatusEffect)) { return; }

                health.ReceiveAttack(new DamageData(INSTAKILL_DAMAGE, DamageData.DamageType.Physical, null), true);
            }
        }
    }
}

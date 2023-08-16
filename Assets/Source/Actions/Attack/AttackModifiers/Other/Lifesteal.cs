using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Causes an attack to heal the causer.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLifesteal", menuName = "Cards/AttackModifers/Lifesteal")]
    public class Lifesteal : AttackModifier
    {
        [Tooltip("The percentage of the damage dealt to heal the causer for")]
        [Min(0)]
        [SerializeField] private float healAmount = 1f;

        // The health to heal on hit.
        private Health causerHealth;

        // The projectile that will steal life.
        Projectile lifestealingProjectile;

        public override void Initialize(Projectile value)
        {
            value.onOverlap += HealCauser;
            causerHealth = value.causer.GetComponent<Health>();
            lifestealingProjectile = value;
        }

        /// <summary>
        /// Applies knockback to the hit collider.
        /// </summary>
        /// <param name="collision"> The collider that was hit. </param>
        private void HealCauser(Collider2D collision)
        {
            if (collision.GetComponent<Health>() == null || !collision.CompareTag("Inanimate")) { return; }

            causerHealth.Heal((int)(lifestealingProjectile.attackData.damage * healAmount));
        }
    }
}

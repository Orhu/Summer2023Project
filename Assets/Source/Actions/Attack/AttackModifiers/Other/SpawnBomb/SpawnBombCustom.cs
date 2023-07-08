using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that spawns explosive bombs when the projectile hits or is destroyed has a custom damage that is unrelated to the projectile that spawned it.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpawnBomb", menuName = "Cards/AttackModifers/Spawn Custom Damage Bomb")]
    public class SpawnBombCustom : SpawnBomb
    {
        [Tooltip("The damage that this will deal.")]
        public DamageData damageData;

        // Gets the damage multiplier from the causer.
        public override Projectile modifiedProjectile
        {
            set
            {
                base.modifiedProjectile = value;
                damageData.damage = Mathf.RoundToInt(damageData.damage * value.causer.GetComponent<IActor>().GetDamageMultiplier());
            }
        }

        /// <summary>
        /// Spawns the bomb on a collision.
        /// </summary>
        /// <param name="collision"> The thing collided with. </param>
        /// <param name="bomb"> The bomb that was created. </param>
        protected override void CreateBomb(Collider2D collision, out Bomb bomb)
        {
            base.CreateBomb(collision, out bomb);
            bomb.damageData = damageData;
        }
    }
}

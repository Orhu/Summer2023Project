using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that spawns explosive bombs when the projectile hits or is destroyed.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpawnBomb", menuName = "Cards/AttackModifers/SpawnBomb")]
    public class SpawnBomb : AttackModifier
    {
        [Tooltip("The radius in tiles of the explosion caused by the bomb.")]
        [Min(0f)]
        public float explosionRadius = 2f;

        [Tooltip("The time in seconds after the bomb is spawned until it detonates.")]
        [Min(0f)]
        public float fuseTime = 2f;

        [Tooltip("Whether or not this should stick to the hit object.")]
        public bool sticky = false;

        [Tooltip("Whether or not this should ignore the same objects as it's parent projectile.")]
        public bool inheritIgnore = false;

        private enum SpawnMode { OnHit, OnDestroyed }
        [Tooltip("When bombs are spawned.")]
        [SerializeField] private SpawnMode spawnMode;

        [Tooltip("The visuals that will be attached to the bomb.")]
        public GameObject bombVisuals;


        // The projectile this modifies
        private Projectile _modifiedProjectile;
        public override Projectile modifiedProjectile
        {
            set
            {
                _modifiedProjectile = value;
                if (spawnMode == SpawnMode.OnDestroyed)
                {
                    _modifiedProjectile.onDestroyed += CreateBomb;
                }
                else
                {
                    _modifiedProjectile.onOverlap += CreateBomb;
                    _modifiedProjectile.onHit += (Collision2D collision) => CreateBomb(collision.collider);
                }
            }
        }

        /// <summary>
        /// Spawns the bomb on a collision.
        /// </summary>
        /// <param name="collision"> The thing collided with. </param>
        /// <param name="bomb"> The bomb that was created. </param>
        protected virtual void CreateBomb(Collider2D collision, out Bomb bomb)
        {
            bomb = new GameObject().AddComponent<Bomb>();
            bomb.name = "Bomb";
            bomb.explosionRadius = explosionRadius;
            bomb.fuseTime = fuseTime;
            bomb.damageData = _modifiedProjectile.attackData;

            if (bombVisuals != null)
            {
                Instantiate(bombVisuals).transform.parent = bomb.transform;
            }

            if (inheritIgnore)
            {
                bomb.ignoredObjects = _modifiedProjectile.ignoredObjects;
            }

            if (sticky && collision != null)
            {
                bomb.transform.parent = collision.transform;
            }

            bomb.transform.position = _modifiedProjectile.transform.position;
        }
        private void CreateBomb(Collider2D collision)
        {
            CreateBomb(collision, out Bomb bomb);
        }
        private void CreateBomb()
        {
            CreateBomb(null, out Bomb bomb);
        }
    }
}

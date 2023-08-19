using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that spawns explosive bombs when the projectile hits or is destroyed.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpawnBomb", menuName = "Cards/AttackModifers/SpawnBomb")]
    public class SpawnBomb : AttackModifier
    {
        [Tooltip("The radius in tiles of the explosion caused by the bomb.")] [Min(0f)]
        public float explosionRadius = 2f;

        [Tooltip("The knockback caused by the explosion in tiles.")]
        public KnockbackInfo knockback = new KnockbackInfo(0f, 0.2f, false);

        [Tooltip("The time in seconds after the bomb is spawned until it detonates.")] [Min(0f)]
        public float fuseTime = 2f;

        [Tooltip("Whether or not this should destroy blockers.")]
        public bool destroyBlockers = false;

        [Tooltip("Whether or not this should stick to the hit object.")]
        public bool sticky = false;

        [Tooltip("Whether or not this should ignore the same objects as it's parent projectile.")]
        public bool inheritIgnore = false;

        private enum SpawnMode { OnHit, OnDestroyed, OnHitOrDestroy }
        [Tooltip("When bombs are spawned.")]
        [SerializeField] private SpawnMode spawnMode;

        [Tooltip("The visuals that will be attached to the bomb.")]
        public GameObject bombVisuals;


        // The projectile this modifies
        private Projectile modifiedProjectile;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            modifiedProjectile = value;
            if (spawnMode == SpawnMode.OnDestroyed)
            {
                modifiedProjectile = value;
            }

            if (spawnMode == SpawnMode.OnDestroyed)
            {
                modifiedProjectile.onDestroyed += () => CreateBomb(); 
            }
            else if (spawnMode == SpawnMode.OnHit)
            {
                modifiedProjectile.onOverlap += (Collider2D collider) => CreateBomb(collider);
                modifiedProjectile.onHit += (Collision2D collision) => CreateBomb(collision.collider);
            }
            else
            {
                System.Action<Collider2D> spawnBombOnOverlap = (Collider2D collider) => CreateBomb(collider);
                modifiedProjectile.onDestroyed +=
                    () =>
                    {
                        modifiedProjectile.onOverlap -= spawnBombOnOverlap;
                        CreateBomb();
                    };
                modifiedProjectile.onOverlap += spawnBombOnOverlap;
                modifiedProjectile.onHit += (Collision2D collision) => CreateBomb(collision.collider);
            }
        }

        /// <summary>
        /// Spawns the bomb on a collision.
        /// </summary>
        /// <param name="collision"> The thing collided with. </param>
        /// <returns> The bomb that was created. </returns>
        protected virtual Bomb CreateBomb(Collider2D collision = null)
        {
            if (modifiedProjectile.forceDestroy) 
            {
                return null;
            }

            Bomb bomb = new GameObject().AddComponent<Bomb>();
            bomb.name = "Bomb";
            bomb.knockback = knockback;
            bomb.explosionRadius = explosionRadius;
            bomb.destroyBlockers = destroyBlockers;
            bomb.fuseTime = fuseTime;
            bomb.damageData = modifiedProjectile.attackData;

            if (bombVisuals != null)
            {
                Instantiate(bombVisuals).transform.parent = bomb.transform;
            }

            if (inheritIgnore)
            {
                bomb.ignoredObjects = modifiedProjectile.ignoredObjects;
            }

            if (sticky && collision != null)
            {
                bomb.transform.parent = collision.transform;
            }

            bomb.transform.position = modifiedProjectile.transform.position;
            return bomb;
        }
    }
}

using UnityEngine;

/// <summary>
/// An action modifier that spawns explosive bombs when the projectile hits or is destroyed.
/// </summary>
namespace Attacks
{
    [CreateAssetMenu(fileName = "NewSpawnBomb", menuName = "Cards/AttackModifers/SpawnBomb")]
    internal class SpawnBomb : AttackModifier
    {
        public enum SpawnMode { OnHit, OnDestroyed }

        [Tooltip("The radius in tiles of the explosion caused by the bomb.")] [Min(0f)]
        public float explosionRadius = 2f;

        [Tooltip("The time in seconds after the bomb is spawned until it detonates.")] [Min(0f)]
        public float fuseTime = 2f;

        [Tooltip("Whether or not this should stick to the hit object.")]
        public bool sticky = false;

        [Tooltip("Whether or not this should ignore the same objects as it's parent projectile.")]
        public bool inheritIgnore = false;

        [Tooltip("When bombs are spawned.")]
        public SpawnMode spawnMode;

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
                    _modifiedProjectile.onHit += CreateBomb;
                }
            }
        }

        private void CreateBomb()
        {
            CreateBomb(null);
        }
        private void CreateBomb(Collider2D collider)
        {
            Bomb newBomb = new GameObject().AddComponent<Bomb>();
            newBomb.explosionRadius = explosionRadius;
            newBomb.fuseTime = fuseTime;
            newBomb.damageData = _modifiedProjectile.attackData;
            
            if (bombVisuals != null)
            {
                Instantiate(bombVisuals).transform.parent = newBomb.transform;
            }

            if (inheritIgnore)
            {
                newBomb.ignoredObjects = _modifiedProjectile.IgnoredObjects;
            }

            if (sticky)
            {
                newBomb.transform.parent = collider.transform;
            }

            newBomb.transform.position = _modifiedProjectile.transform.position;
        }
    }
}

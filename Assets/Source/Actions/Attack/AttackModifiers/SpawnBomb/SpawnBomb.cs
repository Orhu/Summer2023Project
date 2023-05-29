using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
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

        [Tooltip("Whether or not this should ignore the same objects as it's parent projectile.")]
        public bool inheritIgnore = false;

        [Tooltip("When bombs are spawned.")]
        public SpawnMode spawnMode;

        [Tooltip("The visuals that will be attached to the bomb.")]
        public GameObject bombVisuals;


        // The projectile this modifies
        [HideInInspector] private Projectile modifiedProjectile;
        public override Projectile ModifiedProjectile
        {
            set
            {
                modifiedProjectile = value;
                if (spawnMode == SpawnMode.OnDestroyed)
                {
                    modifiedProjectile.onDestroyed += CreateBomb;
                }
                else
                {
                    modifiedProjectile.onHit += CreateBomb;
                }
            }
        }

        private GameObject bombPrefab;


        private void Awake()
        {
            bombPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Source/Actions/Attack/AttackModifiers/SpawnBomb/Bomb.prefab");
        }

        private void CreateBomb()
        {
            Bomb newBomb = Instantiate(bombPrefab).GetComponent<Bomb>();
            newBomb.explosionRadius = explosionRadius;
            newBomb.fuseTime = fuseTime;
            newBomb.damageData = modifiedProjectile.attackData;
            
            if (bombVisuals != null)
            {
                Instantiate(bombVisuals).transform.parent = newBomb.transform;
            }

            if (inheritIgnore)
            {
                newBomb.ignoredObjects = modifiedProjectile.IgnoredObjects;
            }

            newBomb.transform.position = modifiedProjectile.transform.position;
        }
    }
}

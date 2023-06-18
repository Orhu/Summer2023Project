using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Scriptable object that holds a list of enemy pools
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewEnemyPool", menuName = "Generation/EnemyPools", order = 1)]
    public class EnemyPools : ScriptableObject
    {
        [Tooltip("The enemy pools")]
        public List<EnemyPool> enemyPools;

        /// <summary>
        /// Creates a deep copy of this enemy pool
        /// </summary>
        /// <returns> The deep copy </returns>
        public EnemyPools Copy()
        {
            EnemyPools copy = ScriptableObject.CreateInstance<EnemyPools>();
            copy.enemyPools = new List<EnemyPool>();

            foreach (EnemyPool enemyPool in enemyPools)
            {
                EnemyPool poolCopy;
                poolCopy.enemies = new List<GameObject>();
                foreach (GameObject enemy in enemyPool.enemies)
                {
                    poolCopy.enemies.Add(enemy);
                }
                copy.enemyPools.Add(poolCopy);
            }

            return copy;
        }
    }

    /// <summary>
    /// Holds a list of enemies
    /// </summary>
    [System.Serializable]
    public struct EnemyPool
    {
        [Tooltip("The enemies in this pool")]
        public List<GameObject> enemies;
    }
}
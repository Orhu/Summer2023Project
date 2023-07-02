using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores enemy types and their associated enemies
    /// </summary>
    public class EnemyTypesToEnemies : ScriptableObject
    {
        [Tooltip("Tile types and the tiles they can spawn")]
        List<EnemyTypeEnemies> enemyTypesToEnemies;

        /// <summary>
        /// Gets the enemies with the associated enemy types
        /// </summary>
        /// <param name="enemyTypes"> The enemy types to get the tiles of </param>
        /// <returns> The enemies associated with those enemy types </returns>
        public HashSet<GameObject> At(EnemyType enemyTypes)
        {
            HashSet<GameObject> enemies = new HashSet<GameObject>();
            foreach (EnemyTypeEnemies enemyTypeEnemies in enemyTypesToEnemies)
            {
                if (enemyTypes.HasFlag(enemyTypeEnemies.tileType))
                {
                    foreach (GameObject enemy in enemyTypeEnemies.enemies)
                    {
                        enemies.Add(enemy);
                    }
                }
            }

            return enemies;
        }
    }

}
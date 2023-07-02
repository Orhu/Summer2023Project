using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores enemy types and their associated enemies
    /// </summary>
    [System.Serializable] [CreateAssetMenu(fileName = "NewEnemyTypesToEnemies", menuName = "Generation/EnemyTypesToEnemies")]
    public class EnemyTypesToEnemies : ScriptableObject
    {
        [Tooltip("Enemy types and the enemies they can spawn")]
        public List<EnemyTypeEnemies> enemyTypesToEnemies;

        /// <summary>
        /// Gets the enemies with the associated enemy types. Averages the weight if a enemy appears in multiple enemy types.
        /// </summary>
        /// <param name="enemyTypes"> The enemy types to get the enemies of </param>
        /// <returns> The enemies associated with those enemy types </returns>
        public GenericWeightedThings<GameObject> At(EnemyType enemyTypes)
        {
            Dictionary<GameObject, int> enemyCounts = new Dictionary<GameObject, int>();
            Dictionary<GameObject, float> enemyTotalWeights = new Dictionary<GameObject, float>();
            foreach (EnemyTypeEnemies enemyTypeEnemies in enemyTypesToEnemies)
            {
                if (enemyTypes.HasFlag(enemyTypeEnemies.enemyType))
                {
                    foreach (GenericWeightedThing<GameObject> enemy in enemyTypeEnemies.enemies.things)
                    {
                        if (enemyCounts.ContainsKey(enemy.thing))
                        {
                            enemyCounts[enemy.thing]++;
                            enemyTotalWeights[enemy.thing] += enemy.weight;
                        }
                        else
                        {
                            enemyCounts.Add(enemy.thing, 1);
                            enemyTotalWeights.Add(enemy.thing, enemy.weight);
                        }
                    }
                }
            }

            GenericWeightedThings<GameObject> enemys = new GenericWeightedThings<GameObject>();
            foreach (KeyValuePair<GameObject, int> enemyCount in enemyCounts)
            {
                GenericWeightedThing<GameObject> newTile = new GenericWeightedThing<GameObject>(enemyCount.Key, enemyTotalWeights[enemyCount.Key] / enemyCount.Value);
                enemys.Add(newTile, true);
            }

            return enemys;
        }
    }

}
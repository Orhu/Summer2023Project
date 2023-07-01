using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Spawns a random enemy from the chosen enemy pool
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        /// <summary>
        /// Calls the spawn enemy function
        /// </summary>
        private void Start()
        {
            SpawnEnemy();

            // Turn off the sprite renderer (This is so the enemy spawner can have a sprite in template creator but not in game)
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        /// <summary>
        /// Spawns a random enemy from the room enemy pool
        /// </summary>
        private void SpawnEnemy()
        {
            /*List<GameObject> enemies = FloorGenerator.floorGeneratorInstance.currentRoom.template.chosenEnemyPool.enemies;
            GameObject randomEnemy = enemies[Random.Range(0, enemies.Count)];
            FloorGenerator.floorGeneratorInstance.currentRoom.template.chosenEnemyPool.enemies.Remove(randomEnemy);
            randomEnemy = Instantiate(randomEnemy, transform);
            randomEnemy.SetActive(true);*/
        }
    }
}
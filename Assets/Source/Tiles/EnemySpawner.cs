using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Tile))]
    public class EnemySpawner : MonoBehaviour
    {
        [Tooltip("The enemy types that this spawner can spawn")]
        public EnemyType enemyTypes;

        /// <summary>
        /// Spawns the enemy
        /// </summary>
        private void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            GenericWeightedThings<GameObject> possibleEnemies = FloorGenerator.templateParams.enemyTypesToEnemies.At(enemyTypes);
            if (possibleEnemies == null)
            {
                Debug.LogError("No enemies associated with enemy type " + enemyTypes);
            }
            GameObject chosenEnemy = possibleEnemies.GetRandomThing();
            chosenEnemy = Instantiate(chosenEnemy);
            Tile currentTile = GetComponent<Tile>();
            currentTile.room.AddEnemy(chosenEnemy);
        }
    }
}
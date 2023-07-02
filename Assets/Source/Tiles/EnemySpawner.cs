using System.Collections;
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
            if (possibleEnemies == null || possibleEnemies.things == null || possibleEnemies.things.Count == 0)
            {
                Debug.LogError("No enemies associated with enemy type " + enemyTypes);
            }
            GameObject chosenEnemy = possibleEnemies.GetRandomThing();
            string name = chosenEnemy.name;
            chosenEnemy = Instantiate(chosenEnemy);
            chosenEnemy.name = name;
            Tile currentTile = GetComponent<Tile>();
            chosenEnemy.transform.parent = currentTile.room.template.transform;
            chosenEnemy.transform.localPosition = (Vector2)currentTile.gridLocation;

            if (chosenEnemy.GetComponent<Tile>() != null)
            {
                chosenEnemy.GetComponent<Tile>().shouldDisable = false;
            }
        }
    }
}
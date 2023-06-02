using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Calls the spawn enemy function
    /// </summary>
    private void Start()
    {
        SpawnEnemy();
    }

    /// <summary>
    /// Spawns a random enemy from the room enemy pool
    /// </summary>
    private void SpawnEnemy()
    {
        List<GameObject> enemies = GetComponentInParent<Room>().template.enemies;
        GameObject randomEnemy = enemies[Random.Range(0, enemies.Count)];
        randomEnemy = Instantiate(randomEnemy, transform);
        randomEnemy.SetActive(true);
    }
}

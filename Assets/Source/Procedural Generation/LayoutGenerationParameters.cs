using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The parameters that affect the layout generation
/// </summary>
[System.Serializable]
public class LayoutGenerationParameters
{
    [Tooltip("The (approximate) number of normal rooms to generate")]
    public int numNormalRooms;

    [Tooltip("The variance of randomness for the number of normal rooms to generate")]
    public int numNormalRoomsVariance;

    [Tooltip("The size (in tiles) of a room")]
    public Vector2Int roomSize;

    [Tooltip("The number of special rooms that will appear")]
    public int numSpecialRooms;

    [Tooltip("The number of doors that is preferred")]
    [Range(0, 4)]
    public int preferredNumDoors;

    [Tooltip("How strictly the generation adheres to the preferred number of doors (100 = very strict, 0 = not strict at all)")]
    [Range(0, 100)]
    public float strictnessNumDoors;

    [Tooltip("The normal room")]
    public GameObject normalRoom;

    [Tooltip("The boss room")]
    public GameObject bossRoom;

    [Tooltip("The start room")]
    public GameObject startRoom;
}

public enum RoomType
{
    None,
    Normal,
    Start,
    Special,
    Boss,
    Exit
}

[System.Serializable]
public class RoomGenerationParameters
{
    [Tooltip("The number of enemies that will spawn in a room")]
    [Min(0)]
    public int numEnemies;

    [Tooltip("The enemies that can spawn and their weights")]
    public List<WeightedObject> enemies;

    // All the weights of the enemies added together
    [System.NonSerialized]
    public float totalEnemyWeight;

    [Tooltip("The number of obsticles that will spawn in a room")]
    [Min(0)]
    public int numObstacles;

    [Tooltip("The obstacles that can spawn and their weights")]
    public List<WeightedObject> obstacles;

    // All the weights of the obstacles added together
    [System.NonSerialized]
    public float totalObstacleWeight;

    [Tooltip("The object that appears in the exit room idk")]
    public GameObject exitRoomObject;

    /// <summary>
    /// Recalculates the total weight of the enemies and obstacles
    /// </summary>
    public void CalcTotalWeights()
    {
        CalcTotalEnemyWeight();
        CalcTotalObstacleWeight();
    }

    /// <summary>
    /// Recalculates the total weight of all the enemies
    /// </summary>
    void CalcTotalEnemyWeight()
    {
        totalEnemyWeight = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            totalEnemyWeight += enemies[i].weight;
        }
    }

    /// <summary>
    /// Recalculates the total weight of all the obstacles
    /// </summary>
    void CalcTotalObstacleWeight()
    {
        totalObstacleWeight = 0;
        for (int i = 0; i < obstacles.Count; i++)
        {
            totalObstacleWeight += obstacles[i].weight;
        }
    }

    /// <summary>
    /// Adds another room generation parameters to this one
    /// </summary>
    /// <param name="other"> The other room generation parameters </param>
    public void Add(RoomGenerationParameters other)
    {
        numEnemies += other.numEnemies;

        for (int i = 0; i < other.enemies.Count; i++)
        {
            bool enemyFound = false;
            for (int j = 0; j < enemies.Count; j++)
            {
                // This may need to be changed
                if (enemies[j].spawnableObject == other.enemies[i].spawnableObject)
                {
                    enemies[j].weight += other.enemies[i].weight;
                    enemyFound = true;
                    break;
                }
            }

            totalEnemyWeight += other.enemies[i].weight;
            if (!enemyFound)
            {
                WeightedObject newEnemy = new WeightedObject();
                newEnemy.spawnableObject = other.enemies[i].spawnableObject;
                newEnemy.weight = other.enemies[i].weight;
                enemies.Add(newEnemy);
            }
        }

        for (int i = 0; i < other.obstacles.Count; i++)
        {
            bool obstacleFound = false;
            for (int j = 0; j < obstacles.Count; j++)
            {
                if (obstacles[j].spawnableObject == other.obstacles[i].spawnableObject)
                {
                    obstacles[j].weight += other.obstacles[i].weight;
                    obstacleFound = true;
                    break;
                }
            }

            totalObstacleWeight += other.obstacles[i].weight;
            if (!obstacleFound)
            {
                WeightedObject newObstacle = new WeightedObject();
                newObstacle.spawnableObject = other.obstacles[i].spawnableObject;
                newObstacle.weight = other.obstacles[i].weight;
                obstacles.Add(newObstacle);
            }
        }

    }

    /// <summary>
    /// Removes other room generation parameters from this one
    /// </summary>
    /// <param name="other"> The other room generation parameters </param>
    public void Remove(RoomGenerationParameters other)
    {
        if (numEnemies - other.numEnemies <= 0)
        {
            numEnemies = 0;
        }
        else
        {
            numEnemies -= other.numEnemies;
        }

        for (int i = 0; i < other.enemies.Count; i++)
        {
            for (int j = 0; j < enemies.Count; j++)
            {
                if (enemies[j].spawnableObject == other.enemies[i].spawnableObject)
                {
                    if (enemies[j].weight - other.enemies[i].weight <= 0)
                    {
                        totalEnemyWeight -= enemies[j].weight;
                        enemies.RemoveAt(j);
                    }
                    else
                    {
                        enemies[j].weight -= other.enemies[i].weight;
                    }
                    break;
                }
            }
        }

        if (numObstacles - other.numObstacles <= 0)
        {
            numObstacles = 0;
        }
        else
        {
            numObstacles -= other.numObstacles;
        }

        for (int i = 0; i < other.obstacles.Count; i++)
        {
            for (int j = 0; j < obstacles.Count; j++)
            {
                if (obstacles[j].spawnableObject == other.obstacles[i].spawnableObject)
                {
                    if (obstacles[j].weight - other.obstacles[i].weight <= 0)
                    {
                        totalObstacleWeight -= obstacles[j].weight;
                        obstacles.RemoveAt(j);
                    }
                    else
                    {
                        obstacles[j].weight -= other.obstacles[i].weight;
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Returns a random object in the given list, with the likelyhood of getting any particular object weighted by the weight values
    /// </summary>
    /// <param name="objects"> The list to choose from </param>
    /// <param name="totalWeight"> The total weight within that list </param>
    /// <returns> A random object </returns>
    GameObject GetRandomObjectWeighted(List<WeightedObject> objects, float totalWeight)
    {
        float randomPercent = Random.value;
        float percentCounter = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            percentCounter += (objects[i].weight / totalWeight);
            if (percentCounter >= randomPercent)
            {
                return objects[i].spawnableObject;
            }
        }

        return objects[objects.Count - 1].spawnableObject;
    }

    /// <summary>
    /// Returns a random enemy, with the likelyhood of getting any enemy weighted by the weight values
    /// </summary>
    /// <returns> A random enemy </returns>
    public GameObject GetRandomEnemyWeighted()
    {
        return GetRandomObjectWeighted(enemies, totalEnemyWeight);
    }

    /// <summary>
    /// Returns a random obstacle, with the likelyhood of getting any obstacle weighted by the weight values
    /// </summary>
    /// <returns></returns>
    public GameObject GetRandomObstacleWeighted()
    {
        return GetRandomObjectWeighted(obstacles, totalObstacleWeight);
    }
};

/// <summary>
/// A class that stores an enemy alongside a spawn weight
/// </summary>
[System.Serializable]
public class WeightedObject
{
    [Tooltip("The object that can be spawned")]
    public GameObject spawnableObject;

    [Tooltip("The likelyhood of spawning the object  (not a percent, it's out of all the weights added together)")]
    [Min(0)]
    public float weight;
}
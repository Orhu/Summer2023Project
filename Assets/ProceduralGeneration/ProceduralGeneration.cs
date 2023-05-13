using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the layout of rooms
/// </summary>
public class ProceduralGeneration : MonoBehaviour
{
    [Tooltip("The size (in tiles) of a room")]
    public Vector2Int roomSize = new Vector2Int(11, 11);

    // TODO: Actually implement this
    [Tooltip("The size (in unity units) of a tile")]
    public Vector2 cellSize = new Vector2(1, 1);

    [Tooltip("The size (in rooms) of the map")]
    [SerializeField]
    Vector2 mapSize;

    [Tooltip("The room generation parameters")]
    [SerializeField] 
    RoomGenerationParameters roomGenerationParameters;

    [Tooltip("Default room that will be instantiated when generating")]
    [SerializeField] 
    GameObject room;

    // The instance
    public static ProceduralGeneration proceduralGenerationInstance { get; private set; }

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    void Awake()
    {
        if (proceduralGenerationInstance != null && proceduralGenerationInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        proceduralGenerationInstance = this;
    }

    /// <summary>
    /// Generates the rooms
    /// </summary>
    void Start()
    {
        roomGenerationParameters.CalcTotalWeight();
        Generate();
    }

    /// <summary>
    /// Generates the rooms
    /// </summary>
    void Generate()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector2 location;
                location.x = i * roomSize.x * cellSize.x;
                location.y = j * roomSize.y * cellSize.y;
                CreateRoom(location);
            }
        }
    }

    /// <summary>
    /// Creates a room with the given tilemap and a given location
    /// </summary>
    /// <param name="location"> The location of the new room </param>
    void CreateRoom(Vector2 location)
    {
        GameObject newRoom = Instantiate(room, location, Quaternion.identity);
        newRoom.transform.parent = this.transform;
        newRoom.GetComponent<Room>().tile = room.GetComponent<Room>().tile;
        newRoom.GetComponent<Room>().size = roomSize;
        newRoom.GetComponent<Room>().cellSize = cellSize;
        newRoom.GetComponent<Room>().directions = Direction.Right | Direction.Up | Direction.Left | Direction.Down;
        newRoom.SetActive(true);
    }

    /// <summary>
    /// Gets the current room generation parameters
    /// </summary>
    /// <returns> The room generation paramters </returns>
    public RoomGenerationParameters GetRoomGenerationParameters()
    {
        return roomGenerationParameters;
    }

    /// <summary>
    /// Changes the room generation paramters by the added amount
    /// </summary>
    /// <param name="addedRoomParams"> The amount to change the room generation parmaters by </param>
    public void AddRoomGenerationParameters(RoomGenerationParameters addedRoomParams)
    {
        roomGenerationParameters.Add(addedRoomParams);
    }
}

/// <summary>
/// The parameters for room generation
/// </summary>
[System.Serializable]
public class RoomGenerationParameters
{
    [Tooltip("The number of enemies that will spawn in a room")]
    public int numEnemies;

    [Tooltip("The enemies that can spawn and their weights")]
    public List<WeightedEnemy> enemies;

    // All the weights of the enemies added together
    [System.NonSerialized]
    public float totalWeight;

    public void CalcTotalWeight()
    {
        totalWeight = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            totalWeight += enemies[i].weight;
        }
    }

    /// <summary>
    /// Adds another room generation paramters to this one
    /// </summary>
    /// <param name="other"> The other room generation parameters </param>
    public void Add(RoomGenerationParameters other)
    {
        numEnemies += other.numEnemies;

        bool enemyFound = false;
        for (int i = 0; i < other.enemies.Count; i++)
        {
            for (int j = 0; i < enemies.Count; j++)
            {
                // This may need to be changed
                if (enemies[j].enemy == other.enemies[i].enemy)
                {
                    enemies[j].weight += other.enemies[i].weight;
                    enemyFound = true;
                    break;
                }
            }

            totalWeight += other.enemies[i].weight;
            if (!enemyFound)
            {
                WeightedEnemy newEnemy = new WeightedEnemy();
                newEnemy.enemy = other.enemies[i].enemy;
                newEnemy.weight = other.enemies[i].weight;
                enemies.Add(newEnemy);
            }
        }
    }

    public GameObject GetRandomEnemyWeighted()
    {
        float randomPercent = Random.value;
        Debug.Log(randomPercent);
        float percentCounter = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            percentCounter += (enemies[i].weight / totalWeight);

            if (percentCounter >= randomPercent)
            {
                return enemies[i].enemy;
            }
        }

        return enemies[enemies.Count - 1].enemy;
    }
};

[System.Serializable]
public class WeightedEnemy
{
    public GameObject enemy;
    public float weight;
}

/// <summary>
/// Stores door directions
/// </summary>
public enum Direction
{
    None = 0,
    Right = 1,
    Up = 2,
    Left = 4,
    Down = 8
}
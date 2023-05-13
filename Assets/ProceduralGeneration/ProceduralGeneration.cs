using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the layout of rooms
/// </summary>
public class ProceduralGeneration : MonoBehaviour
{
    // The size of the map
    [SerializeField] Vector2 mapSize;

    // The size (in tiles) of a room
    [SerializeField] public Vector2Int roomSize = new Vector2Int(11, 11);

    // The size of a tile
    // TODO: Actually implement this
    [SerializeField] public Vector2 tilesize = new Vector2(1, 1);

    // The room generation parameters
    [SerializeField] RoomGenerationParameters roomGenerationParameters;

    // Default room that will be instantiated when generating
    [SerializeField] GameObject room;

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
                location.x = i * roomSize.x * tilesize.x;
                location.y = j * roomSize.y * tilesize.y;
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
        newRoom.GetComponent<Room>().tilesize = tilesize;
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
    [SerializeField] public int numEnemies;
    [SerializeField] public GameObject enemy;

    /// <summary>
    /// Adds another room generation paramters to this one
    /// </summary>
    /// <param name="other"> The other room generation parameters </param>
    public void Add(RoomGenerationParameters other)
    {
        numEnemies += other.numEnemies;
    }

};
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

    // The tilemaps that can be chosen from
    // TODO: Remove this
    [SerializeField] List<GameObject> possibleTilemaps;

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
                GameObject tilemap = possibleTilemaps[Random.Range(0, possibleTilemaps.Count - 1)];
                Vector2 location;
                Vector2 tileSize;
                tileSize.x = tilemap.GetComponent<Grid>().cellSize.x;
                tileSize.y = tilemap.GetComponent<Grid>().cellSize.y;
                location.x = i * tileSize.x * room.GetComponent<Room>().size.x;
                location.y = j * tileSize.y * room.GetComponent<Room>().size.y;
                CreateRoom(tilemap, location);
            }
        }
    }

    /// <summary>
    /// Creates a room with the given tilemap and a given location
    /// </summary>
    /// <param name="tilemap"> The tilemap of the new room </param>
    /// <param name="location"> The location of the new room </param>
    void CreateRoom(GameObject tilemap, Vector2 location)
    {
        GameObject newRoom = Instantiate(room, location, Quaternion.identity);
        newRoom.transform.parent = this.transform;
        newRoom.GetComponent<Room>().tilemap = tilemap;
        newRoom.GetComponent<Room>().size = room.GetComponent<Room>().size;
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
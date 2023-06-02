using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a grid of tiles, and handles any room behaviors
/// </summary>
public class Room : MonoBehaviour
{
    // The template used to generate this room
    [HideInInspector] public Template template;

    // The grid of tiles
    [HideInInspector] public Tile[,] roomGrid;

    // The size of the room
    [HideInInspector] public Vector2Int roomSize;

    // Returns the size of the room, x * y
    [HideInInspector]
    public int maxSize
    {
        // TODO not sure if this is right
        get => roomSize.x * roomSize.y;
    }

    // The type of the room
    [HideInInspector] public RoomType roomType;

    // The location of the room in the map
    [HideInInspector] public Vector2Int roomLocation;

    // Whether this room has been generated or not
    [HideInInspector] private bool generated = false;
    
    /// <summary>
    /// Gets the tile at the given world position
    /// </summary>
    /// <param name="worldPos"> The world position </param>
    /// <returns> The tile </returns>
    public Tile WorldPosToTile(Vector2 worldPos)
    {
        Debug.Log("first world pos: " + worldPos.ToString());
        Vector2Int gridLocation = new Vector2Int();
        gridLocation.x = Mathf.RoundToInt(worldPos.x - (transform.position.x - roomSize.x / 2));
        gridLocation.y = Mathf.RoundToInt(worldPos.y - (transform.position.y - roomSize.y / 2));
        return roomGrid[gridLocation.x, gridLocation.y];
    }

    /// <summary>
    /// Gets the world position of the given tile
    /// </summary>
    /// <param name="tile"> The tile </param>
    /// <returns> The world position </returns>
    public Vector2 TileToWorldPos(Tile tile)
    {
        Vector2 worldPos = new Vector2();
        worldPos.x = tile.gridLocation.x + transform.position.x - roomSize.x / 2 + 0.5f;
        worldPos.y = tile.gridLocation.y + transform.position.y - roomSize.y / 2 + 0.5f;
        return worldPos;
    }

    /// <summary>
    /// Gets the neighbors of a given tile
    /// </summary>
    /// <param name="tile"> The tile </param>
    /// <returns> The neighbors </returns>
    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int i = 1; i <= (int) Direction.Down; i *= 2)
        {
            Vector2Int locationOffset = new Vector2Int();
            locationOffset.x = System.Convert.ToInt32(((Direction) i & Direction.Right) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Left) != Direction.None);
            locationOffset.y = System.Convert.ToInt32(((Direction) i & Direction.Up) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Down) != Direction.None);

            bool outOfXRange = tile.gridLocation.x + locationOffset.x < 0 || tile.gridLocation.x + locationOffset.x >= roomSize.x;
            bool outOfYRange = tile.gridLocation.y + locationOffset.y < 0 || tile.gridLocation.y + locationOffset.y >= roomSize.y;
            if (outOfXRange || outOfYRange) { continue; }

            neighbors.Add(roomGrid[tile.gridLocation.x + locationOffset.x, tile.gridLocation.y + locationOffset.y]);
        }

        return neighbors;
    }

    /// <summary>
    /// TODO: Implement this
    /// </summary>
    public void OpenDoors()
    {

    }

    /// <summary>
    /// TODO: Implement this
    /// </summary>
    public void CloseDoors()
    {

    }

    /// <summary>
    /// Handles when the player enters the room
    /// </summary>
    /// <param name="collision"> The collider that entered the trigger </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FloorGenerator.floorGeneratorInstance.currentRoom = this;
            if (!generated)
            {
                Template template = FloorGenerator.floorGeneratorInstance.templateGenerationParameters.GetRandomTemplate(roomType);
                GetComponent<TemplateGenerator>().Generate(this, template);
                generated = true;
            }
        }
    }

    /// <summary>
    /// Handles when the player exits the room
    /// </summary>
    /// <param name="collision"> The collider that exited the trigger </param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<TilemapRenderer>().enabled = false;
        }*/
    }
}

/// <summary>
/// Stores the type of a room
/// </summary>
[System.Serializable]
public enum RoomType
{
    None,
    Normal,
    Start,
    Special,
    Boss,
    Exit
}

/// <summary>
/// A dictionary that maps room types to exterior generation parameters
/// </summary>
[System.Serializable]
public class RoomTypesToRoomExteriorGenerationParameters
{
    [Tooltip("A list of room types to exterior generation parameters")]
    [SerializeField] public List<RoomTypeToRoomExteriorGenerationParameters> roomTypesToRoomExteriorGenerationParameters;

    /// <summary>
    /// Gets the exterior generation parameters associated with the given room type
    /// </summary>
    /// <param name="roomType"> The room type to find the exterior generation parameters of </param>
    /// <returns> The exterior generation parameters </returns>
    public RoomExteriorGenerationParameters At(RoomType roomType)
    {
        for (int i = 0; i < roomTypesToRoomExteriorGenerationParameters.Count; i++)
        {
            if (roomTypesToRoomExteriorGenerationParameters[i].roomType == roomType)
            {
                return roomTypesToRoomExteriorGenerationParameters[i].roomExteriorGenerationParameters;
            }
        }

        throw new System.Exception("No room of type " + roomType.ToString() + " in dictionary of room types to room exterior generation parameters");
    }
}

/// <summary>
/// A sturct that holds a room type and its associated exterior generation parameters
/// </summary>
[System.Serializable]
public struct RoomTypeToRoomExteriorGenerationParameters
{
    [Tooltip("The type")]
    [SerializeField] public RoomType roomType;

    [Tooltip("The generation parameters associated with that type")]
    [SerializeField] public RoomExteriorGenerationParameters roomExteriorGenerationParameters;
}

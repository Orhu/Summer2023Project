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

    // The enemies alive in this room
    public List<GameObject> livingEnemies;

    // Whether this room has been generated or not
    private bool generated = false;

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
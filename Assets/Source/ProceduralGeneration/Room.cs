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

    // The doors of this room
    private List<Door> doors;

    // Whether this room has been generated or not
    private bool generated = false;

    /// <summary>
    /// Opens all the doors
    /// </summary>
    public void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.Open();
        }
    }

    /// <summary>
    /// Closes all the doors
    /// </summary>
    public void CloseDoors()
    {
        foreach (Door door in doors)
        {
            door.Close();
        }
    }

    /// <summary>
    /// The function called when the room is entered
    /// </summary>
    /// <param name="direction"> The direction the room is being entered from </param>
    public void Enter(Direction direction)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        FloorGenerator.floorGeneratorInstance.currentRoom = this;

        bool shouldCloseDoors = !generated && template.enemyPools != null;

        Generate();

        // Move player into room, then close/activate doors (so player doesn't get trapped in door)
        StartCoroutine(MovePlayer(direction));

        ActivateDoors();
        if (shouldCloseDoors)
        {
            CloseDoors();
        }
    }

    /// <summary>
    /// The function called when the room is exited
    /// </summary>
    public void Exit()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        DeactivateDoors();
    }

    /// <summary>
    /// Activates the doors, making it so they can be entered and exited
    /// </summary>
    public void ActivateDoors()
    {
        foreach (Door door in doors)
        {
            door.enterable = true;
        }
    }

    /// <summary>
    /// Deactivates the doors, making it so they cannot be entered
    /// </summary>
    public void DeactivateDoors()
    {
        foreach (Door door in doors)
        {
            door.enterable = false;
        }
    }

    /// <summary>
    /// Moves the player into the room 
    /// </summary>
    /// <param name="direction"> The direction the player entered in </param>
    /// <returns> Enumerator so other functions can wait for this to finish </returns>
    public IEnumerator MovePlayer(Direction direction)
    {
        float movePlayerSeconds = 3;
        yield return new WaitForSeconds(movePlayerSeconds);
    }

    /// <summary>
    /// Generates the layout of the room
    /// </summary>
    public void Generate()
    {
        if (!generated)
        {
            Template template = FloorGenerator.floorGeneratorInstance.templateGenerationParameters.GetRandomTemplate(roomType);

            GetComponent<TemplateGenerator>().Generate(this, template);
            generated = true;
        }
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
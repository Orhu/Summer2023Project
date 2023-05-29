using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The generation parameters for an entire floor
/// </summary>
[System.Serializable]
public class FloorGenerationParameters
{
    [Tooltip("The generation parameters for the layout of this floor")]
    [SerializeField] public LayoutGenerationParameters layoutGenerationParameters;

    [Tooltip("The template generation parameters for this floor")]
    [SerializeField] public TemplateGenerationParameters templateGenerationParameters;

    [Tooltip("The size of a room on this floor")]
    [SerializeField] public Vector2Int roomSize;

    [Tooltip("A dictionary that holds room types and their associated exterior generation parameters for this floor")]
    [SerializeField] public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters;
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

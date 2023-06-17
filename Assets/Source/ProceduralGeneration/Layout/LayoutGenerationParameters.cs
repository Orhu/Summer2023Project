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
    [SerializeField] public int numNormalRooms;

    [Tooltip("The variance of randomness for the number of normal rooms to generate")]
    [SerializeField] public int numNormalRoomsVariance;

    [Tooltip("The number of special rooms that will appear")]
    [SerializeField] public int numSpecialRooms;

    [Tooltip("The number of doors that is preferred")] [Range(1, 4)]
    [SerializeField] public int preferredNumDoors;

    [Tooltip("How strictly the generation adheres to the preferred number of doors (100 = very strict, 0 = not strict at all)")] [Range(0, 100)]
    [SerializeField] public float strictnessNumDoors;
}

/// <summary>
/// Stores a list of room types and their associated layout parameters
/// </summary>
[System.Serializable]
public class RoomTypesToLayoutParameters
{
    
}

/// <summary>
/// Stores a room type and its associated layout parameters
/// </summary>
[System.Serializable]
public class RoomTypeToLayoutParameters
{
    [Tooltip("The room type")]
    public RoomType roomType;
    
    [Tooltip("The number of this type of room to appear in the layout")]
    public int numRooms;

    [Tooltip("The variance allowed with the number of rooms")]
    public int numRooomsVariance;
}
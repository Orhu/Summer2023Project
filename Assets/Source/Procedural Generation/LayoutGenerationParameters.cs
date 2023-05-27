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
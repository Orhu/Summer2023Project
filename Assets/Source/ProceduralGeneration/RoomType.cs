using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data about a room type
/// </summary>
public class RoomType : ScriptableObject
{
    [Tooltip("Whether or not this room type can only spawn as a dead end")]
    public bool deadEnd { get; private set; }

    [Tooltip("Whether or not this room type determines it's room size explicitly (only allowed for rooms that are dead ends)")]
    public bool useExplicitRoomSize { get; private set; }

    [Tooltip("Whether or not this room is a normal room (this might not be needed)")]
    public bool normalRoom { get; private set; }

    [Tooltip("The size multiplier of this room: What size is this room compared to the size of a normal room?")]
    public Vector2Int sizeMultiplier { get; private set; }

    [Tooltip("The explicit room size of this room type")]
    public Vector2Int explicitRoomSize { get; private set; }

    [Tooltip("The acceptable cells doors can appear in")]
    public List<Vector2Int> acceptableDoorCells { get; private set; }
}

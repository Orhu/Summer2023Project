using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data about a room type
/// </summary>
public class RoomType : ScriptableObject
{
    public bool deadEnd { get; private set; }
    public bool useExplicitRoomSize { get; private set; }
    public bool normalRoom { get; private set; }
    public Vector2Int sizeMultiplier { get; private set; }
    public Vector2Int explicitRoomSize { get; private set; }
    public List<Vector2Int> acceptableDoorCells { get; private set; }
}

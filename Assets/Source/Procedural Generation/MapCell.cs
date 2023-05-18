using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The cell that is stored in the room grid, to keep track of room generation
/// </summary>
public class MapCell
{
    [Tooltip("The directions that this cell has")]
    public Direction direction = Direction.None;

    [Tooltip("Whether this cell has been added to the list to be generated")]
    public bool added = false;

    [Tooltip("Whether this cell has been generated")]
    public bool generated = false;

    [Tooltip("The location in the map (each integer corresponds to one room)")]
    public Vector2Int location;

    [Tooltip("The room at this location")]
    public GameObject room;

    // A dictionary of directions to door locations
    [HideInInspector]
    public Dictionary<Direction, int> directionToDoorPos;
}

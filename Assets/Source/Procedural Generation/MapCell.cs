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

    [Tooltip("The type of room this is")]
    public RoomType type = RoomType.None;

    [Tooltip("Whether this cell has been visited")]
    public bool visited = false;

    [Tooltip("The location in the map (each integer corresponds to one room)")]
    public Vector2Int location;

    [Tooltip("The room at this location")]
    public GameObject room;
}

/// <summary>
/// Stores door directions
/// </summary>
[System.Flags]
public enum Direction
{
    None = 0,
    Right = 1,
    Up = 2,
    Left = 4,
    Down = 8,
    All = 15
}

/// <summary>
/// Specifies what directions a room must have and which directions a room must not have, and the max number of directions it can have
/// </summary>
public class DirectionConstraint
{
    // The directions that the room must have 
    public Direction mustHave = Direction.None;

    // The directions that the room must not have
    public Direction mustNotHave = Direction.None;

    // The maximum number of directions the room can have
    public int maxDirections = 4;
}

public class Map
{
    public MapCell[,] map;
    public MapCell startCell;
    public Vector2Int mapSize;
}
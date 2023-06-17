using UnityEngine;

namespace Cardificer
{
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
    /// The cell that is stored in the room grid, to keep track of room generation
    /// </summary>
    public class MapCell
    {
        // The directions that this cell has
        public Direction direction = Direction.None;

        // The type of room this is
        public RoomType type = RoomType.None;

        // Whether this cell has been visited
        public bool visited = false;

        // The location in the map (each integer corresponds to one room)
        public Vector2Int location;

        // The room at this location
        public GameObject room;
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

    /// <summary>
    /// Holds a 2D array, a start cell, and a map size
    /// </summary>
    public class Map
    {
        // The 2D array of map cells
        public MapCell[,] map;

        // The start cell (or midpoint)
        public MapCell startCell;

        // The size of the map
        public Vector2Int mapSize;
    }
}
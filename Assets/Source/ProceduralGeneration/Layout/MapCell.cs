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

        // Whether this cell has been visited
        public bool visited = false;

        // For map use only
        public bool seenByMap = false;

        // The location in the map (each integer corresponds to one room)
        public Vector2Int location;

        // The room at this location
        public Room room;
    }

    /// <summary>
    /// The map cell that will show up in the predefined map
    /// </summary>
    [System.Serializable]
    public class PredefinedMapCell
    {
        // The direction of this cell
        public Direction direction;

        // The room type of this cell
        public RoomType roomType;
    }

    /// <summary>
    /// Holds a 2D array, a start cell, and a map size
    /// </summary>
    public class Map
    {
        // The 2D array of map cells
        public MapCell[,] map;

        // The start room (or midpoint)
        public Room startRoom;

        // The size of the map
        public Vector2Int mapSize;
    }
}
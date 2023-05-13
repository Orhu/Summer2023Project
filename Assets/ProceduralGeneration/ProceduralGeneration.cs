using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the layout of rooms
/// </summary>
public class ProceduralGeneration : MonoBehaviour
{
    [Tooltip("The size (in tiles) of a room")]
    public Vector2Int roomSize = new Vector2Int(11, 11);

    // TODO: Actually implement this
    [Tooltip("The size (in unity units) of a tile")]
    public Vector2 cellSize = new Vector2(1, 1);

    [Tooltip("The size (in rooms) of the map")]
    [SerializeField]
    Vector2 mapSize;

    [Tooltip("The room generation parameters")]
    [SerializeField] 
    RoomGenerationParameters roomGenerationParameters;

    [Tooltip("Default room that will be instantiated when generating")]
    [SerializeField] 
    GameObject room;

    // The instance
    public static ProceduralGeneration proceduralGenerationInstance { get; private set; }

    /// <summary>
    /// The cell that is stored in the room grid, to keep track of room generation
    /// </summary>
    public class RoomGridCell
    {
        public Direction direction = Direction.None;
        public bool visited = false;
        public Vector2Int location;
    }

    /// <summary>
    /// A grid to keep track of room generation
    /// </summary>
    List<List<RoomGridCell>> roomGrid;

    /// <summary>
    /// Specifies what directions a room must have and which directions a room must not have
    /// </summary>
    public class DirectionConstraint
    {
        public Direction mustHave = Direction.None;
        public Direction mustNotHave = Direction.None;
    }

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    void Awake()
    {
        if (proceduralGenerationInstance != null && proceduralGenerationInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        proceduralGenerationInstance = this;
    }

    /// <summary>
    /// Generates the rooms
    /// </summary>
    void Start()
    {
        Generate();
    }

    /// <summary>
    /// Generates the rooms
    /// </summary>
    void Generate()
    {
        // Initialize the room grid
        roomGrid = new List<List<RoomGridCell>>();
        for (int i = 0; i < mapSize.x; i++)
        {
            roomGrid.Add(new List<RoomGridCell>());
            for (int j = 0; j < mapSize.y; j++)
            {
                roomGrid[i].Add(new RoomGridCell());
                roomGrid[i][j].location = new Vector2Int(i, j);
            }
        }

        // Start in the middle of the grid
        Vector2Int startPos = new Vector2Int((int) mapSize.x / 2, (int) mapSize.y / 2);
        roomGrid[(int)startPos.x][(int)startPos.y].direction = Direction.Right | Direction.Left | Direction.Up | Direction.Down;
        roomGrid[(int)startPos.x][(int)startPos.y].visited = true;
        Vector2 startLocation;
        startLocation.x = (startPos.x - mapSize.x / 2 + ((mapSize.x % 2) * (float) 1.0/2)) * roomSize.x * cellSize.x;
        startLocation.y = (startPos.y - mapSize.y / 2 + ((mapSize.y % 2) * (float) 1.0/2)) * roomSize.y * cellSize.y;
        CreateRoom(startLocation, roomGrid[startPos.x][startPos.y].direction);

        Queue<RoomGridCell> cellsToGenerate = new Queue<RoomGridCell>();
        List<RoomGridCell> initialNeighbors = GetUnvisitedNeighbors(startPos);
        
        for (int i = 0; i < initialNeighbors.Count; i++)
        {
            initialNeighbors[i].visited = true;
            cellsToGenerate.Enqueue(initialNeighbors[i]);
        }

        while (cellsToGenerate.Count != 0)
        {
            RoomGridCell currentCell = cellsToGenerate.Peek();
            DirectionConstraint directionConstraint = GetDirectionConstraint(currentCell.location);
            Direction direction = (Direction) Random.Range(0, 16);
            direction = direction | directionConstraint.mustHave;
            Direction invertedMustNotHave = (Direction)15 ^ directionConstraint.mustNotHave;
            direction = direction & invertedMustNotHave;
            Vector2 location;
            location.x = (currentCell.location.x - mapSize.x / 2 + ((mapSize.x % 2) * (float)1.0 / 2)) * roomSize.x * cellSize.x;
            location.y = (currentCell.location.y - mapSize.y / 2 + ((mapSize.y % 2) * (float)1.0 / 2)) * roomSize.y * cellSize.y;
            CreateRoom(location, direction);
            currentCell.direction = direction;

            cellsToGenerate.Dequeue();
            List<RoomGridCell> newNeighbors = GetUnvisitedNeighbors(currentCell.location, direction);
            for (int i = 0; i < newNeighbors.Count; i++)
            {
                newNeighbors[i].visited = true;
                cellsToGenerate.Enqueue(newNeighbors[i]);
            }
        }
    }

    /// <summary>
    /// Creates a room with the given tilemap and a given location
    /// </summary>
    /// <param name="location"> The location of the new room </param>
    /// <param name="direction"> The direction of the new room </param>
    void CreateRoom(Vector2 location, Direction direction)
    {
        GameObject newRoom = Instantiate(room, location, Quaternion.identity);
        newRoom.transform.parent = this.transform;
        newRoom.GetComponent<Room>().tile = room.GetComponent<Room>().tile;
        newRoom.GetComponent<Room>().size = roomSize;
        newRoom.GetComponent<Room>().cellSize = cellSize;
        newRoom.GetComponent<Room>().directions = direction;
        newRoom.SetActive(true);
    }

    /// <summary>
    /// Gets the current room generation parameters
    /// </summary>
    /// <returns> The room generation paramters </returns>
    public RoomGenerationParameters GetRoomGenerationParameters()
    {
        return roomGenerationParameters;
    }

    /// <summary>
    /// Changes the room generation paramters by the added amount
    /// </summary>
    /// <param name="addedRoomParams"> The amount to change the room generation parmaters by </param>
    public void AddRoomGenerationParameters(RoomGenerationParameters addedRoomParams)
    {
        roomGenerationParameters.Add(addedRoomParams);
    }

    /// <summary>
    /// Gets the neighbors to the pos specified in the room grid in the direction specified 
    /// </summary>
    /// <param name="pos"> The position to get the neighbors of </param>
    /// <param name="direction"> The directions to check </param>
    /// <returns> The neighbors </returns>
    List<RoomGridCell> GetUnvisitedNeighbors(Vector2Int pos, Direction direction = (Direction) 15)
    {
        List<RoomGridCell> neighbors = new List<RoomGridCell>();

        if ((direction & Direction.Right) != Direction.None && pos.x < mapSize.x - 1 && !roomGrid[pos.x + 1][pos.y].visited)
        {
            neighbors.Add(roomGrid[pos.x + 1][pos.y]);
        }
        if ((direction & Direction.Up) != Direction.None && pos.y < mapSize.y - 1 && !roomGrid[pos.x][pos.y + 1].visited)
        {
            neighbors.Add(roomGrid[pos.x][pos.y + 1]);
        }
        if ((direction & Direction.Left) != Direction.None && pos.x > 0 && !roomGrid[pos.x - 1][pos.y].visited)
        {
            neighbors.Add(roomGrid[pos.x - 1][pos.y]);
        }
        if ((direction & Direction.Down) != Direction.None && pos.y > 0 && !roomGrid[pos.x][pos.y - 1].visited)
        {
            neighbors.Add(roomGrid[pos.x][pos.y - 1]);
        }

        return neighbors;
    }

    /// <summary>
    /// Gets what directions the room at the given position must and must not have
    /// </summary>
    /// <param name="pos"> The position of the room </param>
    /// <returns> The direction constraints </returns>
    DirectionConstraint GetDirectionConstraint(Vector2Int pos)
    {
        DirectionConstraint directionConstraint = new DirectionConstraint();
        if (pos.x == mapSize.x - 1)
        {
            directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Right;
        }
        if (pos.y == mapSize.y - 1)
        {
            directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Up;
        }
        if (pos.x == 0)
        {
            directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Left;
        }
        if (pos.y == 0)
        {
            directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Down;
        }

        if (pos.x < mapSize.x - 1)
        {
            RoomGridCell rightNeighbor = roomGrid[pos.x + 1][pos.y];
            if (rightNeighbor.visited)
            {
                if ((rightNeighbor.direction & Direction.Left) != Direction.None)
                {
                    directionConstraint.mustHave = directionConstraint.mustHave | Direction.Right;
                }
                else
                {
                    directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Right;
                }
            }
        }
        if (pos.y < mapSize.y - 1)
        {
            RoomGridCell upNeighbor = roomGrid[pos.x][pos.y + 1];
            if (upNeighbor.visited)
            {
                if ((upNeighbor.direction & Direction.Down) != Direction.None)
                {
                    directionConstraint.mustHave = directionConstraint.mustHave | Direction.Up;
                }
                else
                {
                    directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Up;
                }
            }
        }
        if (pos.x > 0)
        {
            RoomGridCell leftNeighbor = roomGrid[pos.x - 1][pos.y];
            if (leftNeighbor.visited)
            {
                if ((leftNeighbor.direction & Direction.Right) != Direction.None)
                {
                    directionConstraint.mustHave = directionConstraint.mustHave | Direction.Left;
                }
                else
                {
                    directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Left;
                }
            }
        }
        if (pos.y > 0)
        {
            RoomGridCell downNeighbor = roomGrid[pos.x][pos.y - 1];
            if (downNeighbor.visited)
            {
                if ((downNeighbor.direction & Direction.Up) != Direction.None)
                {
                    directionConstraint.mustHave = directionConstraint.mustHave | Direction.Down;
                }
                else
                {
                    directionConstraint.mustNotHave = directionConstraint.mustNotHave | Direction.Down;
                }
            }
        }

        return directionConstraint;
    }
}

/// <summary>
/// The parameters for room generation
/// </summary>
[System.Serializable]
public class RoomGenerationParameters
{
    [SerializeField] public int numEnemies;
    [SerializeField] public GameObject enemy;

    /// <summary>
    /// Adds another room generation paramters to this one
    /// </summary>
    /// <param name="other"> The other room generation parameters </param>
    public void Add(RoomGenerationParameters other)
    {
        numEnemies += other.numEnemies;
    }

};

/// <summary>
/// Stores door directions
/// </summary>
public enum Direction
{
    None = 0,
    Right = 1,
    Up = 2,
    Left = 4,
    Down = 8
}
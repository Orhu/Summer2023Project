using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;

namespace DeprecatedProceduralGeneration
{
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
        Vector2Int mapSize;

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
            roomGenerationParameters.CalcTotalWeights();
            Deck.playerDeck.onCardAdded += OnCardAdded;
            Deck.playerDeck.onCardRemoved += OnCardRemoved;
            Generate();
        }

        #region Generation
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
            Vector2Int startPos = new Vector2Int((int)mapSize.x / 2, (int)mapSize.y / 2);
            roomGrid[(int)startPos.x][(int)startPos.y].direction = Direction.Right | Direction.Left | Direction.Up | Direction.Down;
            roomGrid[(int)startPos.x][(int)startPos.y].visited = true;
            Vector2 startLocation;
            startLocation.x = (startPos.x - mapSize.x / 2 + ((0) * 1.0f / 2)) * roomSize.x * cellSize.x;
            startLocation.y = (startPos.y - mapSize.y / 2 + ((0) * 1.0f / 2)) * roomSize.y * cellSize.y;
            CreateRoom(startLocation, roomGrid[startPos.x][startPos.y].direction);

            Queue<RoomGridCell> cellsToGenerate = new Queue<RoomGridCell>();
            List<RoomGridCell> initialNeighbors = GetUnvisitedNeighbors(startPos);

            for (int i = 0; i < initialNeighbors.Count; i++)
            {
                initialNeighbors[i].visited = true;
                cellsToGenerate.Enqueue(initialNeighbors[i]);
            }

            Room lastGeneratedRoom = null;

            while (cellsToGenerate.Count != 0)
            {
                RoomGridCell currentCell = cellsToGenerate.Peek();
                DirectionConstraint directionConstraint = GetDirectionConstraint(currentCell.location);
                Direction direction = (Direction)Random.Range(0, 16);
                direction = direction | directionConstraint.mustHave;
                Direction invertedMustNotHave = (Direction)15 ^ directionConstraint.mustNotHave;
                direction = direction & invertedMustNotHave;
                Vector2 location;
                location.x = (currentCell.location.x - mapSize.x / 2 + ((0) * 1.0f / 2)) * roomSize.x * cellSize.x;
                location.y = (currentCell.location.y - mapSize.y / 2 + ((0) * 1.0f / 2)) * roomSize.y * cellSize.y;
                lastGeneratedRoom = CreateRoom(location, direction);
                currentCell.direction = direction;

                cellsToGenerate.Dequeue();
                List<RoomGridCell> newNeighbors = GetUnvisitedNeighbors(currentCell.location, direction);
                for (int i = 0; i < newNeighbors.Count; i++)
                {
                    newNeighbors[i].visited = true;
                    cellsToGenerate.Enqueue(newNeighbors[i]);
                }
            }

            lastGeneratedRoom.exitRoom = true;
        }

        /// <summary>
        /// Creates a room with the given tilemap and a given location
        /// </summary>
        /// <param name="location"> The location of the new room </param>
        /// <param name="direction"> The direction of the new room </param>
        /// 

        ///
        Room CreateRoom(Vector2 location, Direction direction)
        {
            GameObject newRoom = Instantiate(room, location, Quaternion.identity);
            newRoom.transform.parent = this.transform;
            newRoom.GetComponent<Room>().tile = room.GetComponent<Room>().tile;
            newRoom.GetComponent<Room>().size = roomSize;
            newRoom.GetComponent<Room>().cellSize = cellSize;
            newRoom.GetComponent<Room>().directions = direction;
            newRoom.SetActive(true);
            return newRoom.GetComponent<Room>();
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
        /// Changes the room generation parameters by the added amount
        /// </summary>
        /// <param name="addedRoomParams"> The amount to change the room generation parameters by </param>
        public void AddRoomGenerationParameters(RoomGenerationParameters addedRoomParams)
        {
            roomGenerationParameters.Add(addedRoomParams);
        }

        /// <summary>
        /// Changes the room generation parameters by the removed amount
        /// </summary>
        /// <param name="removedRoomParams"> The amount to change the room generation parameters by S</param>
        public void RemoveRoomGenerationParameters(RoomGenerationParameters removedRoomParams)
        {
            roomGenerationParameters.Remove(removedRoomParams);
        }

        /// <summary>
        /// Gets the neighbors to the pos specified in the room grid in the direction specified 
        /// </summary>
        /// <param name="pos"> The position to get the neighbors of </param>
        /// <param name="direction"> The directions to check </param>
        /// <returns> The neighbors </returns>
        List<RoomGridCell> GetUnvisitedNeighbors(Vector2Int pos, Direction direction = (Direction)15)
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

        #endregion

        /// <summary>
        /// Updates the room generation parameters when a card is added to the deck
        /// </summary>
        /// <param name="card"> The card that was added </param>
        void OnCardAdded(Card card)
        {
            for (int i = 0; i < card.effects.Length; i++)
            {
                AddRoomGenerationParameters(card.effects[i].addedRoomGenerationParameters);
                RemoveRoomGenerationParameters(card.effects[i].removedRoomGenerationParameters);
            }
        }

        /// <summary>
        /// Updates the room generation parameters when a card is removed from the deck
        /// </summary>
        /// <param name="card"> The card that was removed </param>
        void OnCardRemoved(Card card)
        {
            for (int i = 0; i < card.effects.Length; i++)
            {
                AddRoomGenerationParameters(card.effects[i].removedRoomGenerationParameters);
                RemoveRoomGenerationParameters(card.effects[i].addedRoomGenerationParameters);
            }
        }

        /// <summary>
        /// Destroys all the rooms that have been created
        /// </summary>
        void DestroyRooms()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            transform.DetachChildren();
        }

        /// <summary>
        /// Reloads the generation. Destroyed the previous generated rooms and generates new ones
        /// </summary>
        public void Regenerate()
        {
            DestroyRooms();
            Generate();
        }
    }

    /// <summary>
    /// The parameters for room generation
    /// </summary>
    [System.Serializable]
    public class RoomGenerationParameters
    {
        [Tooltip("The number of enemies that will spawn in a room")]
        [Min(0)]
        public int numEnemies;

        [Tooltip("The enemies that can spawn and their weights")]
        public List<WeightedObject> enemies;

        // All the weights of the enemies added together
        [System.NonSerialized]
        public float totalEnemyWeight;

        [Tooltip("The number of obsticles that will spawn in a room")]
        [Min(0)]
        public int numObstacles;

        [Tooltip("The obstacles that can spawn and their weights")]
        public List<WeightedObject> obstacles;

        // All the weights of the obstacles added together
        [System.NonSerialized]
        public float totalObstacleWeight;

        [Tooltip("The object that appears in the exit room idk")]
        public GameObject exitRoomObject;

        /// <summary>
        /// Recalculates the total weight of the enemies and obstacles
        /// </summary>
        public void CalcTotalWeights()
        {
            CalcTotalEnemyWeight();
            CalcTotalObstacleWeight();
        }

        /// <summary>
        /// Recalculates the total weight of all the enemies
        /// </summary>
        void CalcTotalEnemyWeight()
        {
            totalEnemyWeight = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                totalEnemyWeight += enemies[i].weight;
            }
        }

        /// <summary>
        /// Recalculates the total weight of all the obstacles
        /// </summary>
        void CalcTotalObstacleWeight()
        {
            totalObstacleWeight = 0;
            for (int i = 0; i < obstacles.Count; i++)
            {
                totalObstacleWeight += obstacles[i].weight;
            }
        }

        /// <summary>
        /// Adds another room generation parameters to this one
        /// </summary>
        /// <param name="other"> The other room generation parameters </param>
        public void Add(RoomGenerationParameters other)
        {
            numEnemies += other.numEnemies;

            for (int i = 0; i < other.enemies.Count; i++)
            {
                bool enemyFound = false;
                for (int j = 0; j < enemies.Count; j++)
                {
                    // This may need to be changed
                    if (enemies[j].spawnableObject == other.enemies[i].spawnableObject)
                    {
                        enemies[j].weight += other.enemies[i].weight;
                        enemyFound = true;
                        break;
                    }
                }

                totalEnemyWeight += other.enemies[i].weight;
                if (!enemyFound)
                {
                    WeightedObject newEnemy = new WeightedObject();
                    newEnemy.spawnableObject = other.enemies[i].spawnableObject;
                    newEnemy.weight = other.enemies[i].weight;
                    enemies.Add(newEnemy);
                }
            }

            for (int i = 0; i < other.obstacles.Count; i++)
            {
                bool obstacleFound = false;
                for (int j = 0; j < obstacles.Count; j++)
                {
                    if (obstacles[j].spawnableObject == other.obstacles[i].spawnableObject)
                    {
                        obstacles[j].weight += other.obstacles[i].weight;
                        obstacleFound = true;
                        break;
                    }
                }

                totalObstacleWeight += other.obstacles[i].weight;
                if (!obstacleFound)
                {
                    WeightedObject newObstacle = new WeightedObject();
                    newObstacle.spawnableObject = other.obstacles[i].spawnableObject;
                    newObstacle.weight = other.obstacles[i].weight;
                    obstacles.Add(newObstacle);
                }
            }

        }

        /// <summary>
        /// Removes other room generation parameters from this one
        /// </summary>
        /// <param name="other"> The other room generation parameters </param>
        public void Remove(RoomGenerationParameters other)
        {
            if (numEnemies - other.numEnemies <= 0)
            {
                numEnemies = 0;
            }
            else
            {
                numEnemies -= other.numEnemies;
            }

            for (int i = 0; i < other.enemies.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (enemies[j].spawnableObject == other.enemies[i].spawnableObject)
                    {
                        if (enemies[j].weight - other.enemies[i].weight <= 0)
                        {
                            totalEnemyWeight -= enemies[j].weight;
                            enemies.RemoveAt(j);
                        }
                        else
                        {
                            enemies[j].weight -= other.enemies[i].weight;
                        }
                        break;
                    }
                }
            }

            if (numObstacles - other.numObstacles <= 0)
            {
                numObstacles = 0;
            }
            else
            {
                numObstacles -= other.numObstacles;
            }

            for (int i = 0; i < other.obstacles.Count; i++)
            {
                for (int j = 0; j < obstacles.Count; j++)
                {
                    if (obstacles[j].spawnableObject == other.obstacles[i].spawnableObject)
                    {
                        if (obstacles[j].weight - other.obstacles[i].weight <= 0)
                        {
                            totalObstacleWeight -= obstacles[j].weight;
                            obstacles.RemoveAt(j);
                        }
                        else
                        {
                            obstacles[j].weight -= other.obstacles[i].weight;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a random object in the given list, with the likelyhood of getting any particular object weighted by the weight values
        /// </summary>
        /// <param name="objects"> The list to choose from </param>
        /// <param name="totalWeight"> The total weight within that list </param>
        /// <returns> A random object </returns>
        GameObject GetRandomObjectWeighted(List<WeightedObject> objects, float totalWeight)
        {
            float randomPercent = Random.value;
            float percentCounter = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                percentCounter += (objects[i].weight / totalWeight);
                if (percentCounter >= randomPercent)
                {
                    return objects[i].spawnableObject;
                }
            }

            return objects[objects.Count - 1].spawnableObject;
        }

        /// <summary>
        /// Returns a random enemy, with the likelyhood of getting any enemy weighted by the weight values
        /// </summary>
        /// <returns> A random enemy </returns>
        public GameObject GetRandomEnemyWeighted()
        {
            return GetRandomObjectWeighted(enemies, totalEnemyWeight);
        }

        /// <summary>
        /// Returns a random obstacle, with the likelyhood of getting any obstacle weighted by the weight values
        /// </summary>
        /// <returns></returns>
        public GameObject GetRandomObstacleWeighted()
        {
            return GetRandomObjectWeighted(obstacles, totalObstacleWeight);
        }
    };

    /// <summary>
    /// A class that stores an enemy alongside a spawn weight
    /// </summary>
    [System.Serializable]
    public class WeightedObject
    {
        [Tooltip("The object that can be spawned")]
        public GameObject spawnableObject;

        [Tooltip("The likelyhood of spawning the object  (not a percent, it's out of all the weights added together)")]
        [Min(0)]
        public float weight;
    }

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
}
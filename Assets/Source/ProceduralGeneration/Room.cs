using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Holds a grid of tiles, and handles any room behaviors
    /// </summary>
    public class Room : MonoBehaviour
    {
        #region RUNTIME_FUNCTIONALITY

        // The enemies alive in this room
        [HideInInspector] public List<GameObject> livingEnemies;

        // The doors of this room
        [HideInInspector] public List<Door> doors = new List<Door>();

        // Called when all enemies in this room are killed.
        public System.Action onCleared;

        /// <summary>
        /// Adds an enemy to the list of living enemies
        /// </summary>
        /// <param name="enemy"> The enemy to add </param>
        public void AddEnemy(GameObject enemy)
        {
            livingEnemies.Add(enemy);
        }

        /// <summary>
        /// Removes an enemy from the list of living enemies
        /// </summary>
        /// <param name="enemy"> The enemy to remove </param>
        public void RemoveEnemy(GameObject enemy)
        {
            livingEnemies.Remove(enemy);
            StartCoroutine(CheckEnemyCount());
        }

        /// <summary>
        /// Checks the enemy count after one frame (waiting for any enemies spawned on death of another enemy)
        /// </summary>
        /// <returns> Waits on frame </returns>
        private IEnumerator CheckEnemyCount()
        {
            yield return null;
            if (livingEnemies.Count == 0)
            {
                OpenDoors();
                onCleared?.Invoke();
            }
        }

        /// <summary>
        /// Opens all the doors
        /// </summary>
        public void OpenDoors()
        {
            foreach (Door door in doors)
            {
                door.Open();
            }
        }

        /// <summary>
        /// Closes all the doors
        /// </summary>
        public void CloseDoors()
        {
            foreach (Door door in doors)
            {
                door.Close();
            }
        }

        /// <summary>
        /// The function called when the room is entered.
        /// </summary>
        /// <param name="direction"> The direction the room is being entered from. </param>
        /// <param name="spawnEnemies"> Whether or not enemies should be spawned. </param>
        /// <param name="callCleared"> Whether or not the on cleared function should be called. </param>
        public void Enter(Direction direction = Direction.None, bool spawnEnemies = true, bool callCleared = true)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            bool shouldCloseDoors = !generated;
            bool enemiesPresent = Generate(spawnEnemies);

            FloorGenerator.currentRoom = this;

            // Move player into room, then close/activate doors (so player doesn't get trapped in door)
            StartCoroutine(MovePlayer(direction, shouldCloseDoors && enemiesPresent, callCleared && (!enemiesPresent && shouldCloseDoors)));
        }

        /// <summary>
        /// The function called when the room is exited
        /// </summary>
        public void Exit()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            DeactivateDoors();
        }

        /// <summary>
        /// Activates the doors, making it so they can be entered and exited
        /// </summary>
        public void ActivateDoors()
        {
            foreach (Door door in doors)
            {
                door.enterable = true;
            }
        }

        /// <summary>
        /// Deactivates the doors, making it so they cannot be entered
        /// </summary>
        public void DeactivateDoors()
        {
            foreach (Door door in doors)
            {
                door.enterable = false;
            }
        }

        /// <summary>
        /// Moves the player into the room 
        /// </summary>
        /// <param name="direction"> The direction the player entered in </param>
        /// <param name="shouldCloseDoors"> Whether or not the doors should close </param>
        /// <param name="clearRoom"> Whether or not on cleared should be called </param>
        /// <returns> Enumerator so other functions can wait for this to finish </returns>
        public IEnumerator MovePlayer(Direction direction, bool shouldCloseDoors, bool clearRoom)
        {
            GameObject player = Player.Get();

            Vector3 bottomLeftLocation = new Vector3(transform.position.x - roomSize.x / 2, transform.position.y - roomSize.y / 2, 0);
            Vector3 topRightLocation = new Vector3(transform.position.x + roomSize.x / 2, transform.position.y + roomSize.y / 2, 0);
            Vector2 movementInput = new Vector2(0, 0);

            if ((direction & Direction.Right) != Direction.None)
            {
                movementInput.x = -1;
            }

            if ((direction & Direction.Up) != Direction.None)
            {
                movementInput.y = -1;
            }

            if ((direction & Direction.Left) != Direction.None)
            {
                movementInput.x = 1;
            }

            if ((direction & Direction.Down) != Direction.None)
            {
                movementInput.y = 1;
            }

            player.GetComponent<PlayerController>().movingEnabled = false;

            bool inXRange = (player.transform.position.x >= bottomLeftLocation.x + 0.9f && player.transform.position.x <= topRightLocation.x - 0.9f);
            bool inYRange = (player.transform.position.y >= bottomLeftLocation.y + 0.9f && player.transform.position.y <= topRightLocation.y - 0.9f);

            while ((!inXRange || !inYRange) && !(direction == Direction.None))
            {

                inXRange = (player.transform.position.x >= bottomLeftLocation.x + 0.9f && player.transform.position.x <= topRightLocation.x - 0.9f);
                inYRange = (player.transform.position.y >= bottomLeftLocation.y + 0.9f && player.transform.position.y <= topRightLocation.y - 0.9f);
                player.GetComponent<SimpleMovement>().movementInput = movementInput;
                yield return null;
            }

            player.GetComponent<PlayerController>().movingEnabled = true;
            player.GetComponent<SimpleMovement>().movementInput = new Vector2(0, 0);

            ActivateDoors();
            if (shouldCloseDoors)
            {
                CloseDoors();
            }

            if (clearRoom)
            {
                onCleared?.Invoke();
            }
        }

        #endregion

        #region GENERATION

        // The template used to generate this room
        [HideInInspector] public Template template;

        // The grid of tiles
        [HideInInspector] public Tile[,] roomGrid;

        // The size of a cell
        [HideInInspector] private Vector2Int _cellSize;
        
        public Vector2Int cellSize 
        { 
            set
            {
                _cellSize = value;
                roomSize = _cellSize * roomType.sizeMultiplier;
                // Transform map to world gives middle of bottom left cell
                Vector2 offset = new Vector2();
                offset.x = ((roomType.sizeMultiplier.x - 1) / 2.0f) * _cellSize.x + (((roomType.sizeMultiplier.x - 1) % 2) * 0.5f);
                offset.y = ((roomType.sizeMultiplier.y - 1) / 2.0f) * _cellSize.y + (((roomType.sizeMultiplier.y - 1) % 2) * 0.5f);
                transform.position = FloorGenerator.TransformMapToWorld(roomLocation, startLocation, cellSize) + offset;
            }
            get { return _cellSize; }
        }

        // The size of the room
        [HideInInspector] public Vector2Int roomSize { private set; get; }

        // The type of the room
        [HideInInspector] public RoomType roomType;

        // The location of the start room in the map
        [HideInInspector] public Vector2Int startLocation;

        // The bottom left location of the room in the map
        [HideInInspector] private Vector2Int _roomLocation;
        public Vector2Int roomLocation 
        { 
            set
            {
                _roomLocation = value;
                transform.position = FloorGenerator.TransformMapToWorld(value, startLocation, cellSize) + (roomType.sizeMultiplier / 2) * (cellSize / 2);
                name = roomType.displayName + " Room " + value.ToString();
            }
            get { return _roomLocation; } 
        }

        // Whether this room has been generated or not
        public bool generated { get; private set; } = false;

        /// <summary>
        /// Generates the template of the room
        /// </summary>
        /// <param name="spawnEnemies"> Whether or not to spawn enemies </param>
        /// <returns> Whether or not enemies were spawned </returns>
        public bool Generate(bool spawnEnemies = true)
        {
            if (!generated)
            {
                Template template = FloorGenerator.templateParams.GetRandomTemplate(roomType);
                generated = true;
                return GetComponent<TemplateGenerator>().Generate(this, template, spawnEnemies);
            }
            return false;
        }

        /// <summary>
        /// Gets all the cells a room takes up
        /// </summary>
        /// <param name="map"> The map to get the cells from </param>
        /// <returns> The room cells </returns>
        public List<MapCell> GetRoomCells(MapCell[,] map)
        {
            List<MapCell> roomCells = new List<MapCell>();

            for (int i = 0; i < roomType.sizeMultiplier.x; i++)
            {
                for (int j = 0; j < roomType.sizeMultiplier.y; j++)
                {
                    roomCells.Add(map[roomLocation.x + i, roomLocation.y + j]);
                }
            }

            return roomCells;
        }

        /// <summary>
        /// Gets the edge cells of the room using the given map
        /// </summary>
        /// <param name="map"> The map to get the cells from </param>
        /// <returns> The edge cells </returns>
        public List<MapCell> GetEdgeCells(MapCell[,] map)
        {
            List<MapCell> edgeCells = new List<MapCell>();

            if (roomType.sizeMultiplier == new Vector2Int(1, 1))
            {
                edgeCells.Add(map[roomLocation.x, roomLocation.y]);
                return edgeCells;
            }

            if (roomType.sizeMultiplier.x == 1)
            {
                for (int j = 0; j < roomType.sizeMultiplier.y; j++)
                {
                    edgeCells.Add(map[roomLocation.x, roomLocation.y + j]);
                }

                return edgeCells;
            }

            if (roomType.sizeMultiplier.y == 1)
            {
                for (int i = 0; i < roomType.sizeMultiplier.x; i++)
                {
                    edgeCells.Add(map[roomLocation.x + i, roomLocation.y]);
                }

                return edgeCells;
            }

            for (int i = 0; i < roomType.sizeMultiplier.x; i++)
            {
                edgeCells.Add(map[roomLocation.x + i, roomLocation.y]);
                edgeCells.Add(map[roomLocation.x + i, roomLocation.y + roomType.sizeMultiplier.y - 1]);
            }

            for (int j = 1; j < roomType.sizeMultiplier.y - 1; j++)
            {
                edgeCells.Add(map[roomLocation.x, roomLocation.y + j]);
                edgeCells.Add(map[roomLocation.x + roomType.sizeMultiplier.x - 1, roomLocation.y + j]);
            }

            return edgeCells;
        }

        /// <summary>
        /// Gets all the rooms that this room is connected to
        /// </summary>
        /// <param name="map"> The map to get the rooms from </param>
        /// <returns> The neighboring rooms </returns>
        public List<Room> GetNeighboringRooms(MapCell[,] map)
        {
            List<Room> neighbors = new List<Room>();

            List<MapCell> edges = GetEdgeCells(map);
            foreach (MapCell edge in edges)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (direction == Direction.None || direction == Direction.All) { continue; }

                    if (!edge.direction.HasFlag(direction)) { continue; }

                    Vector2Int locationOffset = new Vector2Int();
                    locationOffset.x = System.Convert.ToInt32(direction.HasFlag(Direction.Right)) - System.Convert.ToInt32(direction.HasFlag(Direction.Left));
                    locationOffset.y = System.Convert.ToInt32(direction.HasFlag(Direction.Up)) - System.Convert.ToInt32(direction.HasFlag(Direction.Down));
                    bool locationOutsideRoom = locationOffset.x + edge.location.x < roomLocation.x || locationOffset.x + edge.location.x >= roomLocation.x + roomType.sizeMultiplier.x;
                    locationOutsideRoom |= locationOffset.y + edge.location.y < roomLocation.y || locationOffset.y + edge.location.y >= roomLocation.y + roomType.sizeMultiplier.y;

                    if (!locationOutsideRoom) { continue; }

                    MapCell neighbor = map[edge.location.x + locationOffset.x, edge.location.y + locationOffset.y];

                    if (!neighbors.Contains(neighbor.room))
                    {
                        neighbors.Add(neighbor.room);
                    }
                }
            }

            return neighbors;
        }


        #endregion
    }
}
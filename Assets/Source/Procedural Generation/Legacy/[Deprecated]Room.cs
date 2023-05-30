using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DeprecatedProceduralGeneration
{
    /// <summary>
    /// The room, that handles enemy and trap generation when entered
    /// </summary>
    public class Room : MonoBehaviour
    {
        // TODO: change this to actually use art, also make it so collider maps only generate for walls or whatever
        [Tooltip("The tile to use to create the walls")]
        public TileBase tile;

        // The dimensions of this room
        [System.NonSerialized]
        public Vector2Int size = new Vector2Int(11, 11);

        // The size of the tiles
        // TODO: Actually implement this
        [HideInInspector]
        public Vector2 cellSize = new Vector2(1, 1);

        // The directions that this room has doors in
        [HideInInspector]
        public Direction directions;

        // The tilemap of this room; defines the shape of this room.
        Tilemap tilemap;

        // The collider that detects when this room has been entered
        BoxCollider2D roomBox;

        // Whether or not this room has been generated
        bool generated = false;

        // Whether or not this room is the exit room
        [HideInInspector]
        public bool exitRoom = false;

        /// <summary>
        /// Initializes the collision and the tilemap
        /// </summary>
        void Start()
        {
            CreateTilemap();
            GetComponent<TilemapRenderer>().enabled = false;

            roomBox = gameObject.AddComponent<BoxCollider2D>();
            roomBox.transform.parent = this.transform;
            roomBox.transform.position = this.transform.position;
            roomBox.size = new Vector3(size.x, size.y, 0);
            roomBox.isTrigger = true;
        }

        /// <summary>
        /// Creates a tilemap with the size of the room
        /// </summary>
        void CreateTilemap()
        {
            gameObject.AddComponent<Grid>();
            tilemap = gameObject.AddComponent<Tilemap>();

            Vector2Int endOffset = new Vector2Int();
            if (size.x % 2 == 0)
            {
                endOffset.x = size.x / 2 - 1;
            }
            else
            {
                endOffset.x = size.x / 2;
            }

            if (size.y % 2 == 0)
            {
                endOffset.y = size.y / 2 - 1;
            }
            else
            {
                endOffset.y = size.y / 2;
            }

            for (int x = 0; x < size.x; x++)
            {
                if (!ShouldBeDoor(new Vector2Int(x, 0)))
                {
                    tilemap.SetTile(new Vector3Int(x - (size.x / 2), -(size.y / 2), 0), tile);
                }
                if (!ShouldBeDoor(new Vector2Int(x, size.y - 1)))
                {
                    tilemap.SetTile(new Vector3Int(x - (size.x / 2), endOffset.y, 0), tile);
                }
            }

            for (int y = 1; y < size.y - 1; y++)
            {
                if (!ShouldBeDoor(new Vector2Int(0, y)))
                {
                    tilemap.SetTile(new Vector3Int(-(size.x / 2), y - (size.y / 2), 0), tile);
                }
                if (!ShouldBeDoor(new Vector2Int(size.x - 1, y)))
                {
                    tilemap.SetTile(new Vector3Int(endOffset.x, y - (size.y / 2), 0), tile);
                }
            }

            tilemap.CompressBounds();

            gameObject.AddComponent<TilemapCollider2D>();
            gameObject.AddComponent<TilemapRenderer>();
        }

        /// <summary>
        /// Checks whether the position on the tilemap should be a door
        /// </summary>
        /// <param name="pos"> The position on the tilemap to check </param>
        /// <returns> Whether or not the position should be a door </returns>
        bool ShouldBeDoor(Vector2Int pos)
        {
            if (directions == Direction.None)
            {
                return false;
            }

            if ((pos.x != 0 && pos.x != size.x - 1) && (pos.y != 0 && pos.y != size.y - 1))
            {
                return false;
            }

            if (((directions & Direction.Up) != Direction.None) && pos.y == size.y - 1 && (pos.x == (size.x / 2) || pos.x == (size.x / 2) - System.Convert.ToInt32((size.x % 2) == 0)))
            {
                return true;
            }

            if (((directions & Direction.Left) != Direction.None) && pos.x == 0 && (pos.y == (size.y / 2) || pos.y == (size.y / 2) - System.Convert.ToInt32((size.y % 2) == 0)))
            {
                return true;
            }

            if (((directions & Direction.Down) != Direction.None) && pos.y == 0 && (pos.x == (size.x / 2) || pos.x == (size.x / 2) - System.Convert.ToInt32((size.x % 2) == 0)))
            {
                return true;
            }

            if (((directions & Direction.Right) != Direction.None) && pos.x == size.x - 1 && (pos.y == (size.y / 2) || pos.y == (size.y / 2) - System.Convert.ToInt32((size.y % 2) == 0)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates the enemies and traps and other things that appear in a room
        /// </summary>
        public void GenerateRoom()
        {
            // Don't generate if already generated
            if (generated)
            {
                return;
            }
            generated = true;

            RoomGenerationParameters roomParams = ProceduralGeneration.proceduralGenerationInstance.GetRoomGenerationParameters();

            if (exitRoom)
            {
                Vector2 offset = -new Vector2(size.x / 2 - 0.5f, size.y / 2 - 0.5f);
                Vector2 location = (new Vector2(size.x / 2, size.y / 2) + offset) * cellSize;
                location.x += transform.position.x;
                location.y += transform.position.y;
                GameObject exitObject = Instantiate(roomParams.exitRoomObject, location, Quaternion.identity);
                exitObject.SetActive(true);
                return;
            }

            List<Vector2> spawnableLocations = new List<Vector2>();
            for (int i = 1; i < size.x - 1; i++)
            {
                for (int j = 1; j < size.y - 1; j++)
                {
                    if ((directions & Direction.Right) != Direction.None && ShouldBeDoor(new Vector2Int(i + 1, j)))
                    {
                        continue;
                    }
                    if ((directions & Direction.Up) != Direction.None && ShouldBeDoor(new Vector2Int(i, j + 1)))
                    {
                        continue;
                    }
                    if ((directions & Direction.Left) != Direction.None && ShouldBeDoor(new Vector2Int(i - 1, j)))
                    {
                        continue;
                    }
                    if ((directions & Direction.Down) != Direction.None && ShouldBeDoor(new Vector2Int(i, j - 1)))
                    {
                        continue;
                    }
                    spawnableLocations.Add(new Vector2(i, j));
                }
            }

            for (int i = 0; i < roomParams.numEnemies; i++)
            {
                int randomLocation = Random.Range(0, spawnableLocations.Count);
                Vector2 offset = -new Vector2(size.x / 2 - 0.5f, size.y / 2 - 0.5f);
                Vector2 enemyLocation = (spawnableLocations[randomLocation] + offset) * cellSize;
                enemyLocation.x += transform.position.x;
                enemyLocation.y += transform.position.y;
                spawnableLocations.RemoveAt(randomLocation);
                GameObject newEnemy = Instantiate(roomParams.GetRandomEnemyWeighted(), enemyLocation, Quaternion.identity);
                newEnemy.SetActive(true);
                newEnemy.transform.parent = transform;
            }

            for (int i = 0; i < roomParams.numObstacles; i++)
            {
                int randomLocation = Random.Range(0, spawnableLocations.Count);
                Vector2 offset = -new Vector2(size.x / 2 - 0.5f, size.y / 2 - 0.5f);
                Vector2 obstacleLocation = (spawnableLocations[randomLocation] + offset) * cellSize;
                obstacleLocation.x += transform.position.x;
                obstacleLocation.y += transform.position.y;
                spawnableLocations.RemoveAt(randomLocation);
                GameObject newObstacle = Instantiate(roomParams.GetRandomObstacleWeighted(), obstacleLocation, Quaternion.identity);
                newObstacle.SetActive(true);
                newObstacle.transform.parent = transform;
            }
        }

        /// <summary>
        /// Handles when the player enters the room
        /// </summary>
        /// <param name="collision"> The collider that entered the trigger </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GetComponent<TilemapRenderer>().enabled = true;
                GenerateRoom();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GetComponent<TilemapRenderer>().enabled = false;
            }
        }
    }
}
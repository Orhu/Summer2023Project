using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Interfacing class stands as the in-between for the actual room and the pathfinding algorithm. This allows
    /// pathfinding-related alterations remain isolated to this class and not be exposed to the rest of the room ecosystem.
    /// </summary>
    public class RoomInterface : MonoBehaviour
    {
        /// <summary>
        /// Represents a type of movement
        /// </summary>
        [Flags]
        public enum MovementType
        {
            None = 0,
            Walking = 1,
            Flying = 2,
            Burrowing = 4
        }

        // the room this class represents
        private Room myRoom;

        // the size of this room, in tiles
        [HideInInspector] public Vector2Int myRoomSize;

        // the tile grid of this room
        private PathfindingTile[,] roomGrid;

        // the world position of this room
        [HideInInspector] public Vector2 myWorldPosition;

        // this instance
        public static RoomInterface instance;

        [Tooltip("Draw Tiles detected as \"null\" during room-grid copying")]
        [SerializeField] private bool drawNullTiles;

        [Tooltip("Draw Tiles detected as \"walk\" during room-grid copying")]
        [SerializeField] private bool drawWalkTiles;

        [Tooltip("Draw Tiles detected as \"fly\" during room-grid copying")]
        [SerializeField] private bool drawFlyTiles;

        [Tooltip("Draw Tiles detected as \"burrow\" during room-grid copying")]
        [SerializeField] private bool drawBurrowTiles;

        /// <summary>
        /// Sets the instance to this instance
        /// </summary>
        private void Awake()
        {
            instance = this;
            
        }

        /// <summary>
        /// Assigns the onRoomChange event to grab current room
        /// </summary>
        private void Start()
        {
            FloorGenerator.onRoomChange += GrabCurrentRoom;
        }

        /// <summary>
        /// Unbinds grab room.
        /// </summary>
        private void OnDestroy()
        {
            FloorGenerator.onRoomChange -= GrabCurrentRoom;
        }


        /// <summary>
        /// Retrieves player's current room from the FloorGenerator singleton, updating this class' room reference
        /// </summary>
        private void GrabCurrentRoom()
        {
            myRoom = FloorGenerator.currentRoom;
            myRoomSize = myRoom.roomSize;
            myWorldPosition = myRoom.transform.position;
            DeepCopyGrid(myRoom.roomGrid);
        }

        /// <summary>
        /// Get the x * y size of the room, in tiles
        /// </summary>
        /// <returns> The max room size </returns>
        public int GetMaxRoomSize()
        {
            return myRoomSize.x * myRoomSize.y;
        }

        /// <summary>
        /// Copies a given tile grid into a PathfindingTile grid and assigns it to this class' myRoomGrid variable
        /// </summary>
        /// <param name="inputArray"> The input array of tiles </param>
        void DeepCopyGrid(Tile[,] inputArray)
        {
            roomGrid = new PathfindingTile[myRoomSize.x, myRoomSize.y];

            for (int x = 0; x < myRoomSize.x; x++)
            {
                for (int y = 0; y < myRoomSize.y; y++)
                {
                    Tile curTile = inputArray[x, y];
                    AddTiles(curTile, new Vector2Int(x, y));
                }
            }
        }

        /// <summary>
        /// Responsible for properly subdividing and adding tiles at proper locations in their respective grids
        /// </summary>
        /// <param name="tile"> Tile to add </param>
        /// <param name="pos"> Grid position to add tile at </param>
        void AddTiles(Tile tile, Vector2Int pos)
        {
            // TODO subdividing unimplemented
            // add to appropriate lists (if there is a Null Reference in this area, it is most likely because a null tile slipped through the cracks)
            if (tile == null)
            {
                Debug.LogWarning("Tile " + pos + " is null!");
                return;
            }
            roomGrid[pos.x, pos.y] = new PathfindingTile(tile, tile.walkMovementPenalty);
        }

        /// <summary>
        /// Gets the tile at the given world position
        /// </summary>
        /// <param name="worldPos"> The world position </param>
        /// <returns> The tile and boolean saying whether the tile was successfully found </returns>
        public (PathfindingTile, bool) WorldPosToTile(Vector2 worldPos)
        {
            Vector2Int tilePos = new Vector2Int(
                Mathf.RoundToInt(worldPos.x + myRoomSize.x / 2 - myWorldPosition.x),
                Mathf.RoundToInt(worldPos.y + myRoomSize.y / 2 - myWorldPosition.y)
            );
            try
            {
                return (roomGrid[tilePos.x, tilePos.y], true);
            }
            catch
            {
                // if we error here, it means the requested worldPos is outside of the grid
                return (null, false);
            }
        }

        /// <summary>
        /// Gets the world position of the given tile
        /// </summary>
        /// <param name="tile"> The tile </param>
        /// <returns> The world position </returns>
        public Vector2 TileToWorldPos(PathfindingTile tile)
        {
            Vector2 worldPos = new Vector2(
                tile.gridLocation.x + myWorldPosition.x - myRoomSize.x / 2,
                tile.gridLocation.y + myWorldPosition.y - myRoomSize.y / 2
            );
            return worldPos;
        }

        /// <summary>
        /// Gets the neighbors of a given tile
        /// </summary>
        /// <param name="tile"> The tile </param>
        /// <param name="movementType"> The movement type to search for </param>
        /// <returns> The neighbors. </returns>
        public List<PathfindingTile> GetNeighbors(PathfindingTile tile, MovementType movementType)
        {
            List<PathfindingTile> neighbors = new List<PathfindingTile>();

            // Loop through each adjacent tile
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Skip the current tile itself
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = tile.gridLocation.x + x;
                    int checkY = tile.gridLocation.y + y;

                    // Check if the adjacent tile is within the grid bounds
                    if (checkX >= 0 && checkX < myRoomSize.x && checkY >= 0 && checkY < myRoomSize.y)
                    {
                        if (y == 0 || x == 0)
                        {
                            // cardinal direction, just add it!
                            neighbors.Add(roomGrid[checkX, checkY]);
                        }
                        else
                        {
                            try
                            {
                                // this tile is a corner, make sure it is reachable by both of its adjacent tiles (not blocked) (prevents enemies getting stuck on corners)
                                if (roomGrid[checkX - x, checkY].allowedMovementTypes.HasFlag(movementType) &&
                                    roomGrid[checkX, checkY - y].allowedMovementTypes.HasFlag(movementType))
                                {
                                    neighbors.Add(roomGrid[checkX, checkY]);
                                }
                            }
                            catch
                            {
                                // Catch here in case one of the tiles being checked is out of bounds in the grid.
                                // We don't want to cause an error, so we simply skip adding that tile to the neighbours list.
                            }
                        }
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Draw debug gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            if ((drawWalkTiles || drawFlyTiles || drawBurrowTiles || drawNullTiles) && Application.isPlaying && roomGrid != null)
            {
                for (int x = 0; x < roomGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < roomGrid.GetLength(1); y++)
                    {
                        var t = roomGrid[x, y];

                        if (t == null)
                        {
                            if (drawNullTiles)
                            {
                                Gizmos.color = Color.blue;
                                Vector2 worldPos = new Vector2(
                                    x + myWorldPosition.x - myRoomSize.x / 2,
                                    y + myWorldPosition.y - myRoomSize.y / 2
                                );
                                Gizmos.DrawCube(worldPos, Vector3.one);
                            }
                        }
                        else
                        {
                            if (drawWalkTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Walking)
                                    ? Color.green
                                    : Color.red;
                            }
                            else if (drawFlyTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Flying)
                                    ? Color.green
                                    : Color.red;
                            }
                            else if (drawBurrowTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Burrowing)
                                    ? Color.green
                                    : Color.red;
                            }

                            if (drawWalkTiles || drawFlyTiles || drawBurrowTiles)
                            {
                                Gizmos.DrawCube(TileToWorldPos(t), Vector3.one);
                            }
                        }
                    }
                }
            }
        }
    }
}
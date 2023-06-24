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
            Walk = 1,
            Fly = 2,
            Burrow = 4
        }

        // the room this class represents
        private Room myRoom;

        // the size of this room, in tiles
        [HideInInspector] public Vector2Int myRoomSize;

        // the walking tile grid of this room
        private PathfindingTile[,] walkRoomGrid;

        // the flying tile grid of this room
        private PathfindingTile[,] flyRoomGrid;

        // the burrowing tile grid of this room
        private PathfindingTile[,] burrowRoomGrid;

        // the world position of this room
        private Vector2 myWorldPosition;

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

        // tracks list of null tiles during room copying for the purpose of displaying them as gizmos
        private List<Vector2> debugNullTiles = new List<Vector2>();

        /// <summary>
        /// Sets the instance to this instance
        /// </summary>
        private void Start()
        {
            instance = this;
            FloorGenerator.floorGeneratorInstance.onRoomChange.AddListener(GrabCurrentRoom);
        }

        /// <summary>
        /// Retrieves player's current room from the FloorGenerator singleton, updating this class' room reference
        /// </summary>
        public void GrabCurrentRoom()
        {
            myRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
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
            walkRoomGrid = new PathfindingTile[myRoomSize.x, myRoomSize.y];
            flyRoomGrid = new PathfindingTile[myRoomSize.x, myRoomSize.y];
            burrowRoomGrid = new PathfindingTile[myRoomSize.x, myRoomSize.y];

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
            walkRoomGrid[pos.x, pos.y] =
                new PathfindingTile(tile, tile.walkMovementPenalty);
            flyRoomGrid[pos.x, pos.y] =
                new PathfindingTile(tile, tile.flyMovementPenalty);
            burrowRoomGrid[pos.x, pos.y] =
                new PathfindingTile(tile, tile.burrowMovementPenalty);
        }

        /// <summary>
        /// Gets the tile at the given world position
        /// </summary>
        /// <param name="worldPos"> The world position </param>
        /// <param name="movementType"> The movement grid to search on </param>
        /// <returns> The tile and boolean saying whether the tile was successfully found </returns>
        public (PathfindingTile, bool) WorldPosToTile(Vector2 worldPos, MovementType movementType)
        {
            Vector2Int tilePos = new Vector2Int(
                Mathf.RoundToInt(worldPos.x + myRoomSize.x / 2 - myWorldPosition.x),
                Mathf.RoundToInt(worldPos.y + myRoomSize.y / 2 - myWorldPosition.y)
            );
            try
            {
                switch (movementType)
                {
                    case MovementType.Walk:
                        return (walkRoomGrid[tilePos.x, tilePos.y], true);
                    case MovementType.Fly:
                        return (flyRoomGrid[tilePos.x, tilePos.y], true);
                    case MovementType.Burrow:
                        return (burrowRoomGrid[tilePos.x, tilePos.y], true);
                    default:
                        Debug.LogError("Attempted to get tile from invalid movement type");
                        return (null, false);
                }
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
                            switch (movementType)
                            {
                                case MovementType.Walk:
                                    neighbors.Add(walkRoomGrid[checkX, checkY]);
                                    break;
                                case MovementType.Fly:
                                    neighbors.Add(flyRoomGrid[checkX, checkY]);
                                    break;
                                case MovementType.Burrow:
                                    neighbors.Add(burrowRoomGrid[checkX, checkY]);
                                    break;
                            }
                        }
                        else
                        {
                            try
                            {
                                // this tile is a corner, make sure it is reachable by both of its adjacent tiles (not blocked) (prevents enemies getting stuck on corners)
                                switch (movementType)
                                {
                                    case MovementType.Walk:
                                        if (walkRoomGrid[checkX - x, checkY].allowedMovementTypes
                                                .HasFlag(MovementType.Walk) &&
                                            walkRoomGrid[checkX, checkY - y].allowedMovementTypes
                                                .HasFlag(MovementType.Walk))
                                        {
                                            neighbors.Add(walkRoomGrid[checkX, checkY]);
                                        }

                                        break;
                                    case MovementType.Fly:
                                        if (flyRoomGrid[checkX - x, checkY].allowedMovementTypes
                                                .HasFlag(MovementType.Fly) &&
                                            flyRoomGrid[checkX, checkY - y].allowedMovementTypes
                                                .HasFlag(MovementType.Fly))
                                        {
                                            neighbors.Add(flyRoomGrid[checkX, checkY]);
                                        }

                                        break;
                                    case MovementType.Burrow:
                                        if (burrowRoomGrid[checkX - x, checkY].allowedMovementTypes
                                                .HasFlag(MovementType.Burrow) &&
                                            burrowRoomGrid[checkX, checkY - y].allowedMovementTypes
                                                .HasFlag(MovementType.Burrow))
                                        {
                                            neighbors.Add(burrowRoomGrid[checkX, checkY]);
                                        }

                                        break;
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
            if ((drawWalkTiles || drawFlyTiles || drawBurrowTiles) && Application.isPlaying && walkRoomGrid != null)
            {
                for (int x = 0; x < walkRoomGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < walkRoomGrid.GetLength(1); y++)
                    {
                        var t = walkRoomGrid[x, y];

                        if (t == null && drawNullTiles)
                        {
                            Gizmos.color = Color.blue;
                            Vector2 worldPos = new Vector2(
                                x + myWorldPosition.x - myRoomSize.x / 2,
                                y + myWorldPosition.y - myRoomSize.y / 2
                            );
                            Gizmos.DrawCube(worldPos, Vector3.one);
                        }
                        else
                        {
                            if (drawWalkTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Walk)
                                    ? Color.green
                                    : Color.red;
                            }
                            else if (drawFlyTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Fly)
                                    ? Color.green
                                    : Color.red;
                            }
                            else if (drawBurrowTiles)
                            {
                                Gizmos.color = t.allowedMovementTypes.HasFlag(MovementType.Burrow)
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
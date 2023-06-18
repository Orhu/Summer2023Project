using System;
using System.Collections.Generic;
using UnityEngine;
using Tile = Cardificer.Tile;

namespace Cardificer
{
    /// <summary>
    /// Interfacing class stands as the in-between for the actual room and the pathfinding algorithm. This allows
    /// pathfinding-related alterations remain isolated to this class and not be exposed to the rest of the room ecosystem.
    /// </summary>
    public class RoomInterface : MonoBehaviour
    {
        public enum MovementType
        {
            Walk,
            Fly,
            Burrow
        }

        // the room this class represents
        private Room myRoom;

        // the size of this room, in tiles
        [HideInInspector] public Vector2Int myRoomSize;

        // the walking tile grid of this room
        [HideInInspector] public PathfindingTile[,] walkRoomGrid;

        // the flying tile grid of this room
        [HideInInspector] public PathfindingTile[,] flyRoomGrid;

        // the burrowing tile grid of this room
        [HideInInspector] public PathfindingTile[,] burrowRoomGrid;

        // the world position of this room
        private Vector2 myWorldPosition;

        // this instance
        public static RoomInterface instance;

        /// <summary>
        /// Sets the instance to this instance
        /// </summary>
        void Awake()
        {
            instance = this;
        }

        // draw debug gizmos?
        [SerializeField] private bool drawNullTiles;
        [SerializeField] private bool drawWalkTiles;
        [SerializeField] private bool drawFlyTiles;
        [SerializeField] private bool drawBurrowTiles;
        private List<Vector2> debugNullTiles = new List<Vector2>();

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
                    Cardificer.Tile curTile = inputArray[x, y];

                    // TODO must be a better way than foreach comparing to null. Doors return null currently, Mabel says she is working on it so update this when ready
                    if (curTile == null)
                    {
                        // in this case the tile was null. Make an impassable tile at this spot
                        Debug.LogWarning("Null tile at " + x + ", " + y);
                        walkRoomGrid[x, y] = new PathfindingTile(x, y);
                        flyRoomGrid[x, y] = new PathfindingTile(x, y);
                        burrowRoomGrid[x, y] = new PathfindingTile(x, y);
                        debugNullTiles.Add(new Vector2(x, y));
                    }
                    else
                    {
                        // add to appropriate lists
                        if (curTile.walkable)
                        {
                            walkRoomGrid[x, y] =
                                new PathfindingTile(curTile, curTile.walkable, curTile.walkMovementPenalty);
                        }

                        if (curTile.flyable)
                        {
                            flyRoomGrid[x, y] =
                                new PathfindingTile(curTile, curTile.flyable, curTile.flyMovementPenalty);
                        }

                        if (curTile.burrowable)
                        {
                            burrowRoomGrid[x, y] = new PathfindingTile(curTile, curTile.burrowable,
                                curTile.burrowMovementPenalty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tile at the given world position
        /// </summary>
        /// <param name="worldPos"> The world position </param>
        /// <param name="movementType"> The movement type to search for </param>
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
                                        if (walkRoomGrid[checkX - x, checkY].moveable &&
                                            walkRoomGrid[checkX, checkY - y].moveable)
                                        {
                                            neighbors.Add(walkRoomGrid[checkX, checkY]);
                                        }

                                        break;
                                    case MovementType.Fly:
                                        if (flyRoomGrid[checkX - x, checkY].moveable &&
                                            flyRoomGrid[checkX, checkY - y].moveable)
                                        {
                                            neighbors.Add(flyRoomGrid[checkX, checkY]);
                                        }

                                        break;
                                    case MovementType.Burrow:
                                        if (burrowRoomGrid[checkX - x, checkY].moveable &&
                                            burrowRoomGrid[checkX, checkY - y].moveable)
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
            if (drawNullTiles)
            {
                foreach (var v2 in debugNullTiles)
                {
                    Gizmos.color = Color.blue;
                    Vector2 worldPos = new Vector2(
                        v2.x + myWorldPosition.x - myRoomSize.x / 2,
                        v2.y + myWorldPosition.y - myRoomSize.y / 2
                    );
                    Gizmos.DrawCube(worldPos, Vector3.one);
                }
            } else if (drawWalkTiles)
            {
                foreach (var t in walkRoomGrid)
                {
                    Gizmos.color = t.moveable ? Color.green : Color.red;

                    Gizmos.DrawCube(TileToWorldPos(t), Vector3.one);
                }
            }
            else if (drawBurrowTiles)
            {
                foreach (var t in burrowRoomGrid)
                {
                    Gizmos.color = t.moveable ? Color.green : Color.red;

                    Gizmos.DrawCube(TileToWorldPos(t), Vector3.one);
                }
            }
            else if (drawFlyTiles)
            {
                foreach (var t in flyRoomGrid)
                {
                    Gizmos.color = t.moveable ? Color.green : Color.red;

                    Gizmos.DrawCube(TileToWorldPos(t), Vector3.one);
                }
            }
        }
    }
}
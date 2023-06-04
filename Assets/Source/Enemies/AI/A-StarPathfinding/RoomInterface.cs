using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfacing class stands as the in-between for the actual room and the pathfinding algorithm. This allows
/// pathfinding-related alterations remain isolated to this class and not be exposed to the rest of the room ecosystem.
/// </summary>
public class RoomInterface : MonoBehaviour
{
    /// <summary>
    /// Represents a tile for the purposes of pathfinding
    /// </summary>
    public class PathfindingTile : IHeapItem<PathfindingTile>
    {
        // is this tile walkable?
        public bool walkable;

        // how much this tile cost to walk on (higher is avoided more, lower is preferred)
        public int movementPenalty;

        // the x and y location of this tile within the 2D array grid
        public Vector2Int gridLocation;

        // cost of reaching this node from the start node, tracking cumulative cost incurred so far
        public int gCost;

        // cost of reaching this node from the end node, tracking cumulative cost incurred so far
        public int hCost;

        // we can use hCost + gCost to get the total cost of reaching this node
        public int fCost => gCost + hCost;

        // parent of this tile, as determined by pathfinding algorithm. used to retrace steps in pathfinding
        public PathfindingTile retraceStep;

        /// <summary>
        /// Constructor for a PathfindingTile from a Tile
        /// </summary>
        /// <param name="t"> Tile to construct from </param>
        public PathfindingTile(Tile t)
        {
            walkable = t.walkable;
            movementPenalty = t.movementPenalty;
            gridLocation = t.gridLocation;
            gCost = 0;
            hCost = 0;
            retraceStep = null;
        }

        /// <summary>
        /// Compare this tile to another tile
        /// </summary>
        /// <param name="other"> other tile to compare to </param>
        /// <returns> 1 if this tile has a lower fCost, -1 if this tile has a higher fCost, 0 if they are equal </returns>
        public int CompareTo(PathfindingTile other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                // tiebreakers decided based on hCost
                compare = hCost.CompareTo(other.hCost);
            }

            // heap comparison is in reverse order from int comparison
            return -compare;
        }

        // this tile's index within the heap (required for IHeapItem implementation)
        public int heapIndex { get; set; }
    }

    // the room this class represents
    private Room myRoom;

    // the size of this room, in tiles
    private Vector2Int myRoomSize;

    // the tile grid of this room
    private PathfindingTile[,] myRoomGrid;

    // the world position of this room
    private Vector2 myWorldPosition;

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
        myRoomGrid = new PathfindingTile[myRoomSize.x, myRoomSize.y];

        foreach (var tile in inputArray)
        {
            myRoomGrid[tile.gridLocation.x, tile.gridLocation.y] = new PathfindingTile(tile);
        }
    }

    /// <summary>
    /// Gets the tile at the given world position
    /// </summary>
    /// <param name="worldPos"> The world position </param>
    /// <returns> The tile </returns>
    public PathfindingTile WorldPosToTile(Vector2 worldPos)
    {
        Vector2Int tilePos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x + myRoomSize.x / 2 - myWorldPosition.x),
            Mathf.RoundToInt(worldPos.y + myRoomSize.y / 2 - myWorldPosition.y)
        );
        return myRoomGrid[tilePos.x, tilePos.y];
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
    /// <returns> The neighbors </returns>
    public List<PathfindingTile> GetNeighbors(PathfindingTile tile)
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
                    neighbors.Add(myRoomGrid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
}
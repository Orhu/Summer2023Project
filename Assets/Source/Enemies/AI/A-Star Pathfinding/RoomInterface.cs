using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInterface : MonoBehaviour
{
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

        public int heapIndex { get; set; }
    }
    
    private Room myRoom;
    private Vector2Int myRoomSize;
    private PathfindingTile[,] myRoomGrid;
    private Vector2 myWorldPosition;
    
    public void GrabCurrentRoom()
    {
        myRoom = FloorGenerator.floorGeneratorInstance.currentRoom;
        myRoomSize = myRoom.roomSize;
        myWorldPosition = myRoom.transform.position;
        DeepCopyGrid(myRoom.roomGrid);
    }

    public int GetMaxRoomSize()
    {
        return myRoomSize.x * myRoomSize.y;
    }

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

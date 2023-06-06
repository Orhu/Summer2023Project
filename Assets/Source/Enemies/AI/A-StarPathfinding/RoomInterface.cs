using System;
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
        /// Constructor for a PathfindingTile default with a gridLocation. Currently used to mitigate the null error issue with door tiles.
        /// </summary>
        /// <param name="gridX"> Grid X pos </param>
        /// <param name="gridY"> Grid Y pos </param>
        public PathfindingTile(int gridX, int gridY)
        {
            walkable = false;
            movementPenalty = 0;
            gridLocation = new Vector2Int(gridX, gridY);
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
    [HideInInspector] public Vector2Int myRoomSize;

    // the tile grid of this room
    [HideInInspector] public PathfindingTile[,] myRoomGrid;

    // the world position of this room
    private Vector2 myWorldPosition;
    
    // this instance
    public static RoomInterface instance;

    void Awake()
    {
        instance = this;
    }
    
    // draw debug gizmos?
    [SerializeField] private bool drawGizmos;

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

        for (int x = 0; x < myRoomSize.x; x++)
        {
            for (int y = 0; y < myRoomSize.y; y++)
            {
                var curTile = inputArray[x, y];

                // TODO must be a better way than foreach comparing to null. Doors return null currently, Mabel says she is working on it so update this when ready
                if (curTile == null)
                {
                    myRoomGrid[x, y] = new PathfindingTile(x, y);
                }
                else
                {
                    myRoomGrid[x, y] = new PathfindingTile(curTile);
                }
            }
        }
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
            return (myRoomGrid[tilePos.x, tilePos.y], true);
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
    /// <returns> The neighbors. </returns>
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
                    if (y == 0 || x == 0)
                    {
                        // cardinal direction, just add it!
                        neighbors.Add(myRoomGrid[checkX, checkY]);
                    }
                    else
                    {
                        try
                        {
                            // this tile is a corner, make sure it is reachable by both of its adjacent tiles (not blocked)
                            if (myRoomGrid[checkX - x, checkY].walkable && myRoomGrid[checkX, checkY - y].walkable)
                            {
                                neighbors.Add(myRoomGrid[checkX, checkY]);
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

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        foreach(var t in myRoomGrid)
        {
            if (t.walkable)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            
            Gizmos.DrawCube(TileToWorldPos(t), Vector3.one);
        }
    }
}
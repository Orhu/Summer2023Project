using Cardificer;
using UnityEngine;
using Tile = Cardificer.Tile;

/// <summary>
/// Represents a tile for the purposes of pathfinding
/// </summary>
public class PathfindingTile : IHeapItem<PathfindingTile>
{
    // allowed movement types to this tile
    [HideInInspector] public RoomInterface.MovementType allowedMovementTypes;
    
    // how much this tile cost to travel on (higher is avoided more, lower is preferred)
    [HideInInspector] public int movementPenalty;

    // the x and y location of this tile within the 2D array grid
    [HideInInspector] public Vector2Int gridLocation;

    // cost of reaching this node from the start node, tracking cumulative cost incurred so far
    [HideInInspector] public int gCost;

    // cost of reaching this node from the end node, tracking cumulative cost incurred so far
    [HideInInspector] public int hCost;

    // we can use hCost + gCost to get the total cost of reaching this node
    [HideInInspector] public int fCost => gCost + hCost;

    // parent of this tile, as determined by pathfinding algorithm. used to retrace steps in pathfinding
    [HideInInspector] public PathfindingTile retraceStep;
    


    /// <summary>
    /// Constructor for a PathfindingTile from a Tile
    /// </summary>
    /// <param name="t"> Tile to construct from </param>
    /// <param name="movementPenalty"> How much this tile costs to walk on (higher is avoided more, lower is preferred).
    /// This param is needed because Tiles have multiple movementPenalty variables for walking, flying, and burrowing </param>
    public PathfindingTile(Tile t, int movementPenalty)
    {
        allowedMovementTypes = t.allowedMovementTypes;
        this.movementPenalty = movementPenalty;
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
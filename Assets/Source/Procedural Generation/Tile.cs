using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
/// </summary>
[System.Serializable]
public class Tile : IHeapItem<Tile>
{
    [Tooltip("is this tile walkable?")]
    [SerializeField] public bool walkable = true;
    
    [Tooltip("How much this tile costs to walk on (higher is avoided more, lower is preferred)")]
    [SerializeField] public int movementPenalty = 1;
    
    // cost of reaching this node from the start node, tracking cumulative cost incurred so far
    [HideInInspector] public int gCost;

    // cost of reaching this node from the end node, tracking cumulative cost incurred so far
    [HideInInspector] public int hCost;

    // we can use hCost + gCost to get the total cost of reaching this node
    [HideInInspector] public int fCost => gCost + hCost;

    // parent of this tile, as determined by pathfinding algorithm
    [HideInInspector] public Tile parent;

    // the x and y location of this tile within the 2D array grid
    [HideInInspector] public Vector2Int gridLocation;

    // heap index of this tile
    private int _heapIndex;
    [HideInInspector] public int heapIndex
    {
        get => _heapIndex;
        set => _heapIndex = value;
    }

    /// <summary>
    /// Compare this tile to another tile
    /// </summary>
    /// <param name="other"> other tile to compare to </param>
    /// <returns> 1 if this tile has a lower fCost, -1 if this tile has a higher fCost, 0 if they are equal </returns>
    public int CompareTo(Tile other)
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

    [Tooltip("The type of this tile")]
    [SerializeField] public TileType type = TileType.None;

    [Tooltip("The game object on this tile (or to spawn on this tile)")]
    [SerializeField] public GameObject spawnedObject;

    /// <summary>
    /// Creates a shallow copy of the tile
    /// </summary>
    /// <returns> The shallow copy </returns>
    public Tile ShallowCopy()
    {
        Tile copiedTile = new Tile();
        copiedTile.walkable = walkable;
        copiedTile.parent = parent;
        copiedTile.gridLocation = gridLocation;
        copiedTile.gCost = gCost;
        copiedTile.hCost = hCost;
        copiedTile.type = type;
        copiedTile.spawnedObject = spawnedObject;
        return copiedTile;
    }
}

/// <summary>
/// The type of a tile
/// </summary>
[System.Serializable]
public enum TileType
{
    None,
    Container,
    Blocker,
    Turret,
    FloorTrap,
    Pit,
    EnemySpawner
}
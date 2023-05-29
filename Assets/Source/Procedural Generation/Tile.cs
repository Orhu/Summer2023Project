using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
/// </summary>
[System.Serializable]
public class Tile
{
    [Tooltip("is this tile walkable?")]
    [SerializeField] public bool walkable;

    // parent of this tile, as determined by pathfinding algorithm
    public Tile parent;

    // the x and y location of this tile within the 2D array grid
    public Vector2Int gridLocation;

    // cost of reaching this node from the start node, tracking cumulative cost incurred so far
    public int gCost;

    // cost of reaching this node from the end node, tracking cumulative cost incurred so far
    public int hCost;

    // we can use hCost + gCost to get the total cost of reaching this node
    public int fCost => gCost + hCost;

    [Tooltip("The type of this tile")]
    [SerializeField] public TileType type = TileType.None;

    [Tooltip("The game object on this tile (or to spawn on this tile)")]
    [SerializeField] public GameObject spawnedObject;
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
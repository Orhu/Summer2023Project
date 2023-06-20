using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer
{
    /// <summary>
    /// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewTile", menuName = "Generation/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
        [Tooltip("Movement types this tile supports")]
        [SerializeField] public MovementType allowedMovementTypes;

        [Tooltip("How much this tile costs to walk on (higher is avoided more, lower is preferred)")]
        [SerializeField] public int walkMovementPenalty;

        [Tooltip("How much this tile costs to fly over (higher is avoided more, lower is preferred)")]
        public int flyMovementPenalty;

        [Tooltip("How much this tile costs to burrow below (higher is avoided more, lower is preferred)")]
        public int burrowMovementPenalty;

        // the x and y location of this tile within the 2D array grid
        [HideInInspector] public Vector2Int gridLocation;

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
            Tile copiedTile = ScriptableObject.CreateInstance<Tile>();
            copiedTile.allowedMovementTypes = allowedMovementTypes;
            copiedTile.walkMovementPenalty = walkMovementPenalty;
            copiedTile.flyMovementPenalty = flyMovementPenalty;
            copiedTile.burrowMovementPenalty = burrowMovementPenalty;
            copiedTile.gridLocation = gridLocation;
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
        Blocker,
        Container,
        EnemySpawner,
        FloorTrap,
        Loot,
        Pit,
        Staircase,
        Turret,
    }
}
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Tiles for use in the room grid. Holds information for pathfinding and spawning the tiles
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewTile", menuName = "Generation/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
        [Tooltip("is this tile walkable?")]
        [SerializeField] public bool walkable = true;

        [Tooltip("How much this tile costs to walk on (higher is avoided more, lower is preferred)")]
        [SerializeField] public int movementPenalty;

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
            copiedTile.walkable = walkable;
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
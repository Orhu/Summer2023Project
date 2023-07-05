using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores a tile type and the tiles that are part of that tile type
    /// </summary>
    [System.Serializable] [CreateAssetMenu(fileName = "NewTileTypeTiles", menuName = "Generation/TileTypeTiles")]
    public class TileTypeTiles : ScriptableObject
    {
        [Tooltip("The tile type")]
        public TileType tileType;

        [Tooltip("The tiles that are part of that tile type")]
        public GenericWeightedThings<Tile> tiles;
    }

    /// <summary>
    /// The type of a tile
    /// </summary>
    [System.Serializable] [System.Flags]
    public enum TileType
    {
        None = 0,
        Blocker = 1,
        Container = 2,
        FloorTrap = 4,
        Loot = 8,
        Pit = 16,
        Turret = 32,
    }
}
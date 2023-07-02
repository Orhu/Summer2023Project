using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores a tile type and the tiles that are part of that tile type
    /// </summary>
    public class TileTypeTiles : ScriptableObject
    {
        [Tooltip("The tile type")]
        public TileType tileType;

        [Tooltip("The tiles that are part of that tile type")]
        public List<Tile> tiles;
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
        EnemySpawner = 4,
        FloorTrap = 8,
        Loot = 16,
        Pit = 32,
        Staircase = 64,
        Turret = 128,
    }
}
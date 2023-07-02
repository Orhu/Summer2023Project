using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores tile types and their associated tiles
    /// </summary>
    public class TileTypesToTiles : ScriptableObject
    {
        [Tooltip("Tile types and the tiles they can spawn")]
        List<TileTypeTiles> tileTypesToTiles;

        /// <summary>
        /// Gets the tiles with the associated tile types
        /// </summary>
        /// <param name="tileTypes"> The tile types to get the tiles of </param>
        /// <returns> The tiles associated with those tile types </returns>
        public HashSet<Tile> At(TileType tileTypes)
        {
            HashSet<Tile> tiles = new HashSet<Tile>();
            foreach (TileTypeTiles tileTypeTiles in tileTypesToTiles)
            {
                if (tileTypes.HasFlag(tileTypeTiles.tileType))
                {
                    foreach (Tile tile in tileTypeTiles.tiles)
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }
    }

}
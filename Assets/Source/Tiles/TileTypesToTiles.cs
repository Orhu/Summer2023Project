using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores tile types and their associated tiles
    /// </summary>
    [System.Serializable] [CreateAssetMenu(fileName = "NewTileTypesToTiles", menuName = "Generation/TileTypesToTiles")]
    public class TileTypesToTiles : ScriptableObject
    {
        [Tooltip("Tile types and the tiles they can spawn")]
        public List<TileTypeTiles> tileTypesToTiles;

        /// <summary>
        /// Gets the tiles with the associated tile types. Averages the weight if a tile appears in multiple tile types.
        /// </summary>
        /// <param name="tileTypes"> The tile types to get the tiles of </param>
        /// <returns> The tiles associated with those tile types </returns>
        public GenericWeightedThings<Tile> At(TileType tileTypes)
        {
            Dictionary<Tile, int> tileCounts = new Dictionary<Tile, int>();
            Dictionary<Tile, float> tileTotalWeights = new Dictionary<Tile, float>();
            foreach (TileTypeTiles tileTypeTiles in tileTypesToTiles)
            {
                if (tileTypes.HasFlag(tileTypeTiles.tileType))
                {
                    foreach (GenericWeightedThing<Tile> tile in tileTypeTiles.tiles.things)
                    {
                        if (tileCounts.ContainsKey(tile.thing))
                        {
                            tileCounts[tile.thing]++;
                            tileTotalWeights[tile.thing] += tile.weight;
                        }
                        else
                        {
                            tileCounts.Add(tile.thing, 1);
                            tileTotalWeights.Add(tile.thing, tile.weight);
                        }
                    }
                }
            }

            GenericWeightedThings<Tile> tiles = new GenericWeightedThings<Tile>();
            foreach (KeyValuePair<Tile, int> tileCount in tileCounts)
            {
                GenericWeightedThing<Tile> newTile = new GenericWeightedThing<Tile>(tileCount.Key, tileTotalWeights[tileCount.Key] / tileCount.Value);
                tiles.Add(newTile, true);
            }

            return tiles;
        }
    }

}
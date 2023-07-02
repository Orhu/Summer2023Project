using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Tile))]
    public class TileTypeSpawner : MonoBehaviour
    {
        [Tooltip("The tile types that this spawner can spawn")]
        public TileType tileTypes;

        /// <summary>
        /// Spawns the tile
        /// </summary>
        private void Start()
        {
            HashSet<Tile> possibleTiles = FloorGenerator.templateGenerationParameters.tileTypesToTiles.At(tileTypes);
            if (possibleTiles == null)
            {
                Debug.LogWarning("No tiles associated with tile types " + tileTypes);
            }
            Tile chosenTile = possibleTiles.ElementAt(FloorGenerator.random.Next(0, possibleTiles.Count));
            chosenTile = Instantiate(chosenTile.gameObject).GetComponent<Tile>();
            Tile currentTile = GetComponent<Tile>();
            chosenTile.transform.parent = currentTile.room.tileContainer.transform;
            chosenTile.transform.localPosition = (Vector2) currentTile.gridLocation;
            currentTile.room.roomGrid[currentTile.gridLocation.x, currentTile.gridLocation.y] = chosenTile;
            Destroy(this);
        }
    }
}
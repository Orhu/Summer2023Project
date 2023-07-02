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
            GenericWeightedThings<Tile> possibleTiles = FloorGenerator.templateParams.tileTypesToTiles.At(tileTypes);
            if (possibleTiles == null)
            {
                Debug.LogWarning("No tiles associated with tile types " + tileTypes);
                return;
            }

            Tile chosenTile = possibleTiles.GetRandomThing(FloorGenerator.random);
            chosenTile = Instantiate(chosenTile.gameObject).GetComponent<Tile>();
            Tile currentTile = GetComponent<Tile>();
            chosenTile.transform.parent = currentTile.room.tileContainer.transform;
            chosenTile.transform.localPosition = (Vector2) currentTile.gridLocation;
            currentTile.room.roomGrid[currentTile.gridLocation.x, currentTile.gridLocation.y] = chosenTile;
            Destroy(this);
        }
    }
}
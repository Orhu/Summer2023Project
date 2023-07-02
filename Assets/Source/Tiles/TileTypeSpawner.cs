using System.Collections;
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
            if (tileTypes == TileType.None)
            {
                Debug.LogError("Tile type spawner in " + GetComponent<Tile>().room.template + " has no tile types specified!");
                return;
            }

            GenericWeightedThings<Tile> possibleTiles = FloorGenerator.templateParams.tileTypesToTiles.At(tileTypes);
            if (possibleTiles == null || possibleTiles.things == null)
            {
                Debug.LogWarning("No tiles associated with tile types " + tileTypes);
                return;
            }

            Tile chosenTile = possibleTiles.GetRandomThing(FloorGenerator.random);
            string name = chosenTile.name;
            chosenTile = Instantiate(chosenTile.gameObject).GetComponent<Tile>();
            Tile currentTile = GetComponent<Tile>();
            chosenTile.transform.parent = currentTile.room.template.transform;
            chosenTile.transform.localPosition = (Vector2) currentTile.gridLocation;
            chosenTile.name = name + " (" + currentTile.gridLocation.x + ", " + currentTile.gridLocation.y + ")";
            currentTile.room.roomGrid[currentTile.gridLocation.x, currentTile.gridLocation.y] = chosenTile;
            StartCoroutine(EnableTileAfterOneFrame(chosenTile));
        }

        /// <summary>
        /// Enables the created tile after one frame to ensure creation is good
        /// </summary>
        /// <param name="tile"> The tile to enable </param>
        /// <returns> Wait one frame </returns>
        private IEnumerator EnableTileAfterOneFrame(Tile tile)
        {
            yield return null;
            tile.Enable();
            Destroy(this);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A template, to be used for generating the interior of rooms.
    /// The prefab this component is on will have several children that have tile components
    /// </summary>
    public class Template : MonoBehaviour
    {
        [Tooltip("The map cell size")]
        [SerializeField] private Vector2Int _mapCellSize = new Vector2Int(17, 11);
        public Vector2Int mapCellSize
        {
            set
            {
                _mapCellSize = value;
                roomSize = sizeMultiplier * _mapCellSize;
            }
            get { return _mapCellSize; }
        }

        [Tooltip("The size multiplier for this template")]
        [SerializeField] private Vector2Int _sizeMultiplier = new Vector2Int(1, 1);
        public Vector2Int sizeMultiplier
        {
            set
            {
                _sizeMultiplier = value;
                roomSize = _sizeMultiplier * mapCellSize;
            }
            get { return _sizeMultiplier; }
        }

        // The size of the room this template is for
        public Vector2Int roomSize
        {
            get 
            { 
                return mapCellSize * sizeMultiplier; 
            }
            private set
            {
                tiles = new List<TilesList>();
                for (int i = 0; i < roomSize.x; i++)
                {
                    tiles.Add(new TilesList());
                    for (int j = 0; j < roomSize.y; j++)
                    {
                        tiles[i].tiles.Add(null);
                    }
                }
            }
        }

        // The list of layers (first layer is always pathfinding layer)
        [HideInInspector] public List<GameObject> layers;

        /// <summary>
        /// Class that contains a list of tiles so the templates can be serialized correctly
        /// </summary>
        [System.Serializable]
        private class TilesList
        {
            [Tooltip("The list of tiles")]
            public List<Tile> tiles;

            /// <summary>
            /// Constructor that initializes the tiles
            /// </summary>
            public TilesList()
            {
                tiles = new List<Tile>();
            }

            /// <summary>
            /// Indexer
            /// </summary>
            /// <param name="i"> The index </param>
            /// <returns> The tile at that index </returns>
            public Tile this[int i]
            {
                get => tiles[i];
                set => tiles[i] = value;
            }
        }

        [Tooltip("The tiles on the template")]
        [SerializeField] private List<TilesList> tiles;

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i"> the first index </param>
        /// <param name="j"> The second index </param>
        /// <returns> The tile at those indices </returns>
        public Tile this[int i, int j]
        {
            get => tiles[i][j];
            set => tiles[i][j] = value;
        }
    }
}
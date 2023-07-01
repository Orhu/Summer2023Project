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
        private Vector2Int _roomSize;
        public Vector2Int roomSize
        {
            get { return _roomSize; }
            private set
            {
                _roomSize = value;
                tiles = new Tile[value.x, value.y];
            }
        }

        // The tiles on the template
        public Tile[,] tiles;

        // Indexer
        public Tile this[int i, int j]
        {
            get => tiles[i, j];
            set => tiles[i, j] = value;
        }
    }
}
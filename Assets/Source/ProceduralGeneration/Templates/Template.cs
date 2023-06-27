using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A template, to be used for generating the interior of rooms
    /// </summary>
    public class Template : ScriptableObject
    {
        [Tooltip("The tiles in this template")]
        public List<TilesList> tiles;

        [Tooltip("The enemy pools that this template has")]
        public EnemyPools enemyPools;

        [Tooltip("The size of the room this template is for")]
        [field: SerializeField] private Vector2Int _roomSize;
        public Vector2Int roomSize
        {
            get { return _roomSize; }
            set
            {
                _roomSize = value;
                tiles = new List<TilesList>();
                for (int i = 0; i < _roomSize.x; i++)
                {
                    tiles.Add(new TilesList());
                    for (int j = 0; j < _roomSize.y; j++)
                    {
                        tiles[i].Add(null);
                    }
                }
            }
        }

        // The enemy pool that was chosen for this template
        [HideInInspector] public EnemyPool chosenEnemyPool;
    }

    /// <summary>
    /// List wrapper so unity properly serializes a multi-dimensional list (why isn't this built in)
    /// </summary>
    [System.Serializable]
    public class TilesList
    {
        [Tooltip("A row of tiles")]
        public List<TemplateTile> tiles;

        /// <summary>
        /// Constructor that constructs the inner tiles list
        /// </summary>
        public TilesList()
        {
            tiles = new List<TemplateTile>();
        }

        /// <summary>
        /// Operator overload so the wrapper can still be used as a regular list
        /// </summary>
        /// <param name="index"> The index within the list to access </param>
        /// <returns> The tile at the given index </returns>
        public TemplateTile this[int index]
        {
            get
            {
                return tiles[index];
            }
            set
            {
                tiles[index] = value;
            }
        }

        /// <summary>
        /// Wraps the normal list with another add so it's possible to add directly to the wrapper
        /// </summary>
        /// <param name="addedTile"> The added tile </param>
        public void Add(TemplateTile addedTile)
        {
            tiles.Add(addedTile);
        }
    }
}
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

        [Tooltip("The list of layers (first layer is always pathfinding layer)")]
        [SerializeField] private List<GameObject> layers;

        [Tooltip("The tiles belonging to each layer (first index is the pathfinding tiles)")]
        [SerializeField] private List<GameObjects2DList> layerTiles;

        // The size of the room this template is for
        public Vector2Int roomSize
        {
            get 
            { 
                return mapCellSize * sizeMultiplier; 
            }
            private set
            {
                layerTiles = new List<GameObjects2DList>();

                if (layers == null)
                {
                    layers = new List<GameObject>();
                    return;
                }

                for (int i = 0; i < layers.Count; i++)
                {
                    layerTiles.Add(new GameObjects2DList());
                    for (int j = 0; j < roomSize.x; j++)
                    {
                        layerTiles[i].gameObjects.Add(new GameObjectsList());
                        for (int k = 0; k < roomSize.y; k++)
                        {
                            layerTiles[i][j].gameObjects.Add(null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Class that contiains a list of a list of game objects so the templates can be serialized correctly
        /// </summary>
        [System.Serializable]
        private class GameObjects2DList
        {
            [Tooltip("The list of game objects")]
            public List<GameObjectsList> gameObjects;

            /// <summary>
            /// Constructor that initializes the list
            /// </summary>
            public GameObjects2DList()
            {
                gameObjects = new List<GameObjectsList>();
            }

            /// <summary>
            /// Indexer
            /// </summary>
            /// <param name="i"> The index </param>
            /// <returns> The tile at that index </returns>
            public GameObjectsList this[int i]
            {
                get => gameObjects[i];
                set => gameObjects[i] = value;
            }
        }

        /// <summary>
        /// Class that contains a list of game objects so the templates can be serialized correctly
        /// </summary>
        [System.Serializable]
        private class GameObjectsList
        {
            [Tooltip("The list of game objects")]
            public List<GameObject> gameObjects;

            /// <summary>
            /// Constructor that initializes the list
            /// </summary>
            public GameObjectsList()
            {
                gameObjects = new List<GameObject>();
            }

            /// <summary>
            /// Indexer
            /// </summary>
            /// <param name="i"> The index </param>
            /// <returns> The tile at that index </returns>
            public GameObject this[int i]
            {
                get => gameObjects[i];
                set => gameObjects[i] = value;
            }
        }

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

        /// <summary>
        /// Layer Indexer
        /// </summary>
        /// <param name="i"> The layer index </param>
        /// <param name="j"> The row index </param>
        /// <param name="k"> The column index </param>
        /// <returns> The tile at those indices </returns>
        public GameObject this[int i, int j, int k]
        {
            get
            {
                try
                {
                    return layerTiles[i][j][k];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                { 
                    layerTiles[i][j][k] = value;
                }
                catch
                {
                    Debug.LogWarning(i + ", " + j + ", " + ", " + k + " doesn't exist");
                }
            }
        }

        /// <summary>
        /// Pathfinding Indexer
        /// </summary>
        /// <param name="i"> The row index </param>
        /// <param name="j"> The column index </param>
        /// <returns> The tile at those indices </returns>
        public Tile this[int i, int j]
        {
            // The first index in layer tiles is always the pathfinding layer
            get
            {
                if (layerTiles[0][i][j] == null)
                {
                    return null;
                }
                return layerTiles[0][i][j].GetComponent<Tile>();
            }
            set => layerTiles[0][i][j] = value.gameObject;
        }

        /// <summary>
        /// Adds a layer to the template
        /// </summary>
        /// <param name="newLayer"> The layer to add </param>
        public void AddLayer(GameObject newLayer)
        {
            newLayer.transform.parent = transform;
            newLayer.transform.localPosition = new Vector3(0, 0);
            layers.Add(newLayer);
            layerTiles.Add(new GameObjects2DList());
            for (int i = 0; i < roomSize.x; i++)
            {
                layerTiles[layerTiles.Count - 1].gameObjects.Add(new GameObjectsList());
                for (int j = 0; j < roomSize.y; j++)
                {
                    layerTiles[layerTiles.Count - 1][i].gameObjects.Add(null);
                }
            }
        }

        /// <summary>
        /// Removes a layer from the template
        /// </summary>
        /// <param name="removedLayer"> The layer to remove </param>
        public void RemoveLayer(int removedLayer)
        {
            if (removedLayer < 0 || removedLayer >= layerTiles.Count)
            {
                Debug.LogError("Cannot remove layer " + removedLayer + " as that is out of range of template with layer count " + layerTiles.Count);
            }
            if (removedLayer == 0)
            {
                Debug.LogError("Cannot remove layer 0 from the template, as that is the pathfinding layer!");
                return;
            }

            GameObject removedLayerObject = layers[removedLayer];
            layers.RemoveAt(removedLayer);
            layerTiles.RemoveAt(removedLayer);
            Destroy(removedLayerObject);
        }

        /// <summary>
        /// Gets the layers this template has
        /// </summary>
        /// <returns> The layers </returns>
        public List<GameObject> GetLayers()
        {
            List<GameObject> returnVal = new List<GameObject>();
            foreach (GameObject layer in layers)
            {
                returnVal.Add(layer);
            }
            return returnVal;
        }

        /// <summary>
        /// Gets a specific layer this template has
        /// </summary>
        /// <param name="layer"> The layer to get </param>
        /// <returns> The layer </returns>
        public GameObject GetLayer(int layer)
        {
            return layers[layer];
        }

        /// <summary>
        /// Checks if the template is valid yet
        /// </summary>
        /// <returns> Whether or not the template is valid </returns>
        public bool IsValid()
        {
            return layerTiles != null;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Cardificer
{
    /// <summary>
    /// Allows you to create templates
    /// </summary>
    public class TemplateCreator : MonoBehaviour
    {
        /// <summary>
        /// Tracks tile types and their associated sprites
        /// </summary>
        [System.Serializable]
        public struct TileTypeToSprite
        {
            public TileType tileType;
            public Sprite sprite;
        }

        [Tooltip("The UI for choosing the template tiles")]
        public GameObject templateTileChooser;

        [Tooltip("The camera")]
        public TemplateCreatorCamera templateCamera;

        [Tooltip("The generic tiles (to load into the tile holder")]
        public GenericTiles genericTiles;

        [Tooltip("The name to give the template")]
        public string templateName;

        [Tooltip("The sprite to use when the preferred object is null")]
        public Sprite nullSprite;

        [Tooltip("The template to load when load template is called")]
        public Template templateToLoad;

        [Tooltip("The size a singular cell in the map")]
        [SerializeField] private Vector2Int _mapCellSize = new Vector2Int(17, 11);

        public Vector2Int mapCellSize
        {
            set
            {
                _mapCellSize = value;
                roomSize = sizeMultiplier * _mapCellSize;
                Reload();
            }
            get { return _mapCellSize; }
        }

        [Tooltip("The size multiplier of this template")]
        [SerializeField] private Vector2Int _sizeMultiplier = new Vector2Int(1, 1);

        public Vector2Int sizeMultiplier
        {
            set
            {
                _sizeMultiplier = value;
                roomSize = _sizeMultiplier * mapCellSize;
                Reload();
            }
            get { return _sizeMultiplier; }
        }

        public Vector2Int roomSize { private set; get; }

        // The actual template being created
        private Template createdTemplate;

        // A container that holds all the sprite game objects
        private GameObject spriteContainer;

        // A container that holds the visual bounding box
        private GameObject visualsContainer;

        // The sprite game objects
        private GameObject[,] sprites;

        // Tracks whether start has been called or not
        private bool started;

        /// <summary>
        /// Initializes the template creator
        /// </summary>
        private void Start()
        {
            started = true;
            roomSize = sizeMultiplier * mapCellSize;
            Reload();
        }

        /// <summary>
        /// Saves the scriptable object to a file
        /// </summary>
        public void SaveTemplate()
        {
            string path = "Assets/Content/Templates/" + templateName + ".asset";

#if UNITY_EDITOR
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(createdTemplate, path);
            AssetDatabase.Refresh();

            Debug.Log("Template saved to " + path);

            Reload();
#endif
        }

        /// <summary>
        /// Updates everything when the room size is changed
        /// </summary>
        private void Reload()
        {
            Destroy(spriteContainer);
            spriteContainer = new GameObject();
            spriteContainer.transform.parent = transform;
            spriteContainer.name = "Sprite container";
            spriteContainer.transform.localPosition = new Vector3(0, 0, 0);
            sprites = new GameObject[roomSize.x, roomSize.y];

            createdTemplate = ScriptableObject.CreateInstance<Template>();
            createdTemplate.mapCellSize = mapCellSize;
            createdTemplate.sizeMultiplier = sizeMultiplier;
            transform.position = new Vector3(-roomSize.x / 2, -roomSize.y / 2);
            CreateVisualBoundingBox();
        }

        /// <summary>
        /// Loads a template into the template creator
        /// </summary>
        public void LoadTemplate()
        {
            if (templateToLoad == null)
            {
                Debug.LogWarning("To load a template, set the template to load in the template creator!");
                return;
            }
            Debug.Log("Loading template " + templateToLoad.name);
            mapCellSize = templateToLoad.mapCellSize;
            sizeMultiplier = templateToLoad.sizeMultiplier;
            for (int i = 0; i < roomSize.x; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    if (templateToLoad.tiles[i][j] != null)
                    {
                        PlaceTile(templateToLoad.tiles[i][j], new Vector2Int(i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a visual bounding box and grid
        /// </summary>
        private void CreateVisualBoundingBox()
        {
            Destroy(visualsContainer);
            visualsContainer = new GameObject();
            visualsContainer.name = "Visuals container";
            visualsContainer.transform.parent = transform;

            GameObject boundingBoxContainer = new GameObject();
            boundingBoxContainer.transform.parent = visualsContainer.transform;
            boundingBoxContainer.name = "Bounding Box";

            LineRenderer boundingBox = boundingBoxContainer.AddComponent<LineRenderer>();
            boundingBox.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            boundingBox.widthMultiplier = 0.05f;
            boundingBox.endColor = Color.white;
            boundingBox.startColor = Color.white;
            boundingBox.positionCount = 5;
            boundingBox.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f, -roomSize.y / 2 - 0.5f));
            boundingBox.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f, roomSize.y / 2 + 0.5f));
            boundingBox.SetPosition(2, new Vector3(roomSize.x / 2 + 0.5f, roomSize.y / 2 + 0.5f));
            boundingBox.SetPosition(3, new Vector3(roomSize.x / 2 + 0.5f, -roomSize.y / 2 - 0.5f));
            boundingBox.SetPosition(4, new Vector3(-roomSize.x / 2 - 0.5f, -roomSize.y / 2 - 0.5f));
            boundingBox.enabled = true;

            GameObject wallBoxContainer = new GameObject();
            wallBoxContainer.transform.parent = visualsContainer.transform;
            wallBoxContainer.name = "Wall Box";
            LineRenderer wallBoundingBox = wallBoxContainer.AddComponent<LineRenderer>();
            wallBoundingBox.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            wallBoundingBox.widthMultiplier = 0.05f;
            wallBoundingBox.endColor = Color.white;
            wallBoundingBox.startColor = Color.white;
            wallBoundingBox.positionCount = 5;
            wallBoundingBox.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f + 1, -roomSize.y / 2 - 0.5f + 1));
            wallBoundingBox.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + 1, roomSize.y / 2 + 0.5f - 1));
            wallBoundingBox.SetPosition(2, new Vector3(roomSize.x / 2 + 0.5f - 1, roomSize.y / 2 + 0.5f - 1));
            wallBoundingBox.SetPosition(3, new Vector3(roomSize.x / 2 + 0.5f - 1, -roomSize.y / 2 - 0.5f + 1));
            wallBoundingBox.SetPosition(4, new Vector3(-roomSize.x / 2 - 0.5f + 1, -roomSize.y / 2 - 0.5f + 1));
            wallBoundingBox.enabled = true;

            for (int i = 2; i < roomSize.x - 1; i++)
            {
                GameObject gridLineContainer = new GameObject();
                gridLineContainer.name = "Grid Line";
                gridLineContainer.transform.parent = visualsContainer.transform;

                LineRenderer gridLine = gridLineContainer.AddComponent<LineRenderer>();
                gridLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                gridLine.endColor = Color.gray;
                gridLine.startColor = Color.gray;
                gridLine.widthMultiplier = 0.03f;
                gridLine.positionCount = 2;
                gridLine.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f + i, -roomSize.y / 2 - 0.5f + 1));
                gridLine.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + i, roomSize.y / 2 + 0.5f - 1));
            }

            for (int j = 2; j < roomSize.y - 1; j++)
            {
                GameObject gridLineContainer = new GameObject();
                gridLineContainer.name = "Grid Line";
                gridLineContainer.transform.parent = visualsContainer.transform;

                LineRenderer gridLine = gridLineContainer.AddComponent<LineRenderer>();
                gridLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                gridLine.endColor = Color.gray;
                gridLine.startColor = Color.gray;
                gridLine.widthMultiplier = 0.03f;
                gridLine.positionCount = 2;
                gridLine.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f + 1, -roomSize.y / 2 - 0.5f + j));
                gridLine.SetPosition(1, new Vector3(roomSize.x / 2 + 0.5f - 1, -roomSize.y / 2 - 0.5f + j));
            }
        }

        /// <summary>
        /// Places a tile in the template
        /// </summary>
        /// <param name="tile"> The tile to place </param>
        /// <param name="gridPos"> The grid position to place the tile in </param>
        public void PlaceTile(TemplateTile tile, Vector2Int gridPos)
        {
            if (gridPos.x >= 1 && gridPos.x < roomSize.x - 1 && gridPos.y >= 1 && gridPos.y < roomSize.y - 1)
            {
                sprites[gridPos.x, gridPos.y] = new GameObject();
                GameObject createdTile = sprites[gridPos.x, gridPos.y];
                createdTile.transform.parent = spriteContainer.transform;
                createdTile.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
                createdTile.AddComponent<SpriteRenderer>().sprite = tile.sprite;
                TemplateTileEditor templateTileEditor = createdTile.AddComponent<TemplateTileEditor>();
                templateTileEditor.templateTile = tile.Copy();
                createdTemplate.tiles[gridPos.x][gridPos.y] = templateTileEditor.templateTile;
            }
        }

        /// <summary>
        /// Erases the tile at a given position
        /// </summary>
        /// <param name="gridPos"> The position to erase the tile at </param>
        public void EraseTile(Vector2Int gridPos)
        {
            if (gridPos.x >= 1 && gridPos.x < roomSize.x - 1 && gridPos.y >= 1 && gridPos.y < roomSize.y - 1)
            {
                Destroy(sprites[gridPos.x, gridPos.y]);
                sprites[gridPos.x, gridPos.y] = null;
                createdTemplate.tiles[gridPos.x][gridPos.y] = null;
            }
        }

        /// <summary>
        /// Gets the tile at a given position
        /// </summary>
        /// <returns> The tile </returns>
        public TemplateTile GetTile(Vector2Int gridPos)
        {
            if (IsGridPosOutsideBounds(gridPos))
            {
                return null;
            }
            return createdTemplate.tiles[gridPos.x][gridPos.y];
        }

        /// <summary>
        /// Gets the tile editor at the given grid pos
        /// </summary>
        /// <param name="gridPos"> The pos of the tile editor </param>
        /// <returns> The tile editor </returns>
        public TemplateTileEditor GetTileEditor(Vector2Int gridPos)
        {
            if (IsGridPosOutsideBounds(gridPos))
            {
                return null;
            }
            return sprites[gridPos.x, gridPos.y].GetComponent<TemplateTileEditor>();
        }

        /// <summary>
        /// Determines whether a given grid pos is outside the bounds of the template
        /// </summary>
        /// <param name="gridPos"> The grid pos</param>
        /// <returns> Whether or not the grid pos is within the bounds </returns>
        public bool IsGridPosOutsideBounds(Vector2Int gridPos)
        {
            return gridPos.x < 0 || gridPos.x >= roomSize.x || gridPos.y < 0 || gridPos.y >= roomSize.y;
        }

        /// <summary>
        /// Reloads the template if the room size has changed
        /// </summary>
        private void OnValidate()
        {
            if (started && roomSize != sizeMultiplier * mapCellSize)
            {
                // Disabling this warning because it's only relevant for on validate when it's called outside of play time
                roomSize = sizeMultiplier * mapCellSize;
                EditorApplication.delayCall += Reload;
            }
        }
    }
}
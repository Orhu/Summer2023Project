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
        [Header("Template")]

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

        [Header("Scene variables")]

        [Tooltip("The camera")]
        public TemplateCreatorCamera templateCamera;

        // The size of the room being created
        public Vector2Int roomSize { private set; get; }

        // A null sprite game object
        [HideInInspector] public GameObject nullSpriteObject;

        // The actual template being created
        private Template createdTemplate;

        // A container that holds the visual bounding box
        private GameObject visualsContainer;

        // A container for holding null sprites
        private GameObject nullSpritesContainer;

        // The null sprite game objects
        private GameObject[,] nullSprites;

        // Tracks whether start has been called or not
        private bool started;

        /// <summary>
        /// Initializes the template creator
        /// </summary>
        private void Start()
        {
            started = true;
            roomSize = sizeMultiplier * mapCellSize;
            nullSpriteObject = new GameObject();
            nullSpriteObject.name = "Null Sprite Object";
            nullSpriteObject.transform.parent = transform;
            nullSpriteObject.AddComponent<SpriteRenderer>().sprite = nullSprite;
            GetComponent<TemplateCreatorInput>().Initialize();
            Reload();
        }

        /// <summary>
        /// Saves the scriptable object to a file
        /// </summary>
        public void SaveTemplate()
        {
            string path = "Assets/Content/Templates/" + templateName + ".prefab";

#if UNITY_EDITOR
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            PrefabUtility.SaveAsPrefabAsset(createdTemplate.gameObject, path);
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
            Destroy(createdTemplate);
            createdTemplate = new GameObject().AddComponent<Template>();
            createdTemplate.name = "Created Template";
            createdTemplate.sizeMultiplier = sizeMultiplier;
            createdTemplate.mapCellSize = mapCellSize;
            createdTemplate.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2);

            Destroy(nullSpritesContainer);
            nullSpritesContainer = new GameObject();
            nullSpritesContainer.transform.parent = transform;
            nullSpritesContainer.name = "Null Sprites Container";
            transform.position = new Vector3(-roomSize.x / 2, -roomSize.y / 2);

            nullSprites = new GameObject[roomSize.x, roomSize.y];

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

            createdTemplate = Instantiate(templateToLoad);

            for (int i = 0; i < roomSize.x; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    if (createdTemplate[i, j] != null && createdTemplate[i, j].GetComponent<SpriteRenderer>() == null)
                    {
                        PlaceTile(createdTemplate[i, j], new Vector2Int(i, j));
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
        public void PlaceTile(Tile tile, Vector2Int gridPos)
        {
            if (gridPos.x >= 1 && gridPos.x < roomSize.x - 1 && gridPos.y >= 1 && gridPos.y < roomSize.y - 1)
            {
                Tile createdTile = Instantiate(tile);
                createdTile.name = tile.name;
                createdTile.transform.parent = createdTemplate.transform;
                createdTile.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
                createdTile.gridLocation = gridPos;
                createdTemplate[gridPos.x, gridPos.y] = createdTile;

                if (createdTile.GetComponent<SpriteRenderer>() == null || createdTile.GetComponent<SpriteRenderer>().sprite == null)
                {
                    GameObject createdNullSprite = Instantiate(nullSpriteObject);
                    nullSprites[gridPos.x, gridPos.y] = createdNullSprite;
                    createdNullSprite.transform.parent = nullSpritesContainer.transform;
                    createdNullSprite.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
                }
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
                Destroy(nullSprites[gridPos.x, gridPos.y]);
                nullSprites[gridPos.x, gridPos.y] = null;
                if (createdTemplate[gridPos.x, gridPos.y] != null)
                {
                    Destroy(createdTemplate[gridPos.x, gridPos.y].gameObject);
                    createdTemplate[gridPos.x, gridPos.y] = null;
                }
            }
        }

        /// <summary>
        /// Gets the tile at a given position
        /// </summary>
        /// <returns> The tile </returns>
        public Tile GetTile(Vector2Int gridPos)
        {
            if (IsGridPosOutsideBounds(gridPos))
            {
                return null;
            }
            return createdTemplate[gridPos.x, gridPos.y];
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
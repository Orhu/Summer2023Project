using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

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

    [Tooltip("The generic tiles (to load into the tile holder")]
    public GenericTiles genericTiles;

    [Tooltip("The name to give the template")]
    public string templateName;

    [Tooltip("The sprite to use when the preferred object is null")]
    public Sprite nullSprite;

    [Tooltip("The size of the room this template is for")]
    [field: SerializeField] private Vector2Int _roomSize;

    public Vector2Int roomSize
    {
        get { return _roomSize; }
        set
        {
            _roomSize = value;
            Reload();
        }
    }

    // The actual template being created
    private Template createdTemplate;

    // A container that holds all the sprite game objects
    private GameObject spriteContainer;

    // The sprite game objects
    private GameObject[,] sprites;

    // The tile being held
    private TemplateTile heldTile;

    // The socket to display the tile being held
    private GameObject heldTileSocket;

    /// <summary>
    /// Initializes the template creator
    /// </summary>
    private void Start()
    {
        templateTileChooser = Instantiate(templateTileChooser);
        templateTileChooser.transform.parent = transform;
        templateTileChooser.SetActive(true);
        InitializeHeldTileSocket();
        InitializeTileHUD();
        Reload();
    }

    /// <summary>
    /// Initializes the tile hud, where you can pick tiles to place
    /// </summary>
    private void InitializeTileHUD()
    {
        Button saveButton = templateTileChooser.GetComponentInChildren<Button>();
        saveButton.onClick.AddListener(SaveTemplate);

        VerticalLayoutGroup layout = templateTileChooser.GetComponentInChildren<VerticalLayoutGroup>();
        foreach (TileTypeToGenericTile genericTile in genericTiles.tileTypesToGenericTiles)
        {
            GameObject button = new GameObject();
            button.name = genericTile.tileType.ToString();

            // Add RectTransform component and set position and size
            RectTransform rectTransform = button.AddComponent<RectTransform>();
            rectTransform.SetParent(layout.transform, false);
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(50f, 50f);

            // Add Button component
            Button buttonComponent = button.AddComponent<Button>();
            buttonComponent.interactable = true;

            // Add Image component and set sprite
            Image imageComponent = button.AddComponent<Image>();

            // Add click listener
            TemplateTile templateTile = new TemplateTile();

            if (genericTile.genericTile.spawnedObject == null || genericTile.genericTile.spawnedObject.GetComponent<SpriteRenderer>() != null)
            {
                imageComponent.sprite = genericTile.genericTile.spawnedObject.GetComponent<SpriteRenderer>().sprite;
                templateTile.sprite = genericTile.genericTile.spawnedObject.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                imageComponent.sprite = nullSprite;
                templateTile.sprite = nullSprite;
            }

            templateTile.tileType = genericTile.tileType;

            buttonComponent.onClick.AddListener(() => OnHoldedTileChosen(templateTile));
            button.SetActive(true);
        }
    }

    /// <summary>
    /// Saves the scriptable object to a file
    /// </summary>
    private void SaveTemplate()
    {
        string path = "Assets/Content/DungeonTemplates/" + templateName + ".asset";

        #if UNITY_EDITOR

        path = AssetDatabase.GenerateUniqueAssetPath(path);


        AssetDatabase.CreateAsset(createdTemplate, path);
        AssetDatabase.Refresh();

        Debug.Log("Template saved to " + path);

        Reload();
#endif
    }

    /// <summary>
    /// Initializes the socket for displaying the held tile
    /// </summary>
    private void InitializeHeldTileSocket()
    {
        heldTileSocket = new GameObject();
        heldTileSocket.name = "Held Template Socket";
        heldTileSocket.transform.parent = transform;
        heldTileSocket.AddComponent<SpriteRenderer>();
        heldTileSocket.GetComponent<SpriteRenderer>().enabled = false;
    }

    /// <summary>
    /// Changes the tile being held
    /// </summary>
    /// <param name="tile"> The tile being held </param>
    public void OnHoldedTileChosen(TemplateTile tile)
    {
        heldTile = tile;
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
        createdTemplate.roomSize = roomSize;
        transform.position = new Vector3(-roomSize.x / 2, -roomSize.y / 2);
        CreateVisualBoundingBox();
    }

    /// <summary>
    /// Creates a visual bounding box and grid
    /// </summary>
    private void CreateVisualBoundingBox()
    {
        GameObject visualsContainer = new GameObject();
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
    /// Handles placing and removing tiles
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);
            if (heldTile == null)
            {
                // TODO: Actually implement this

                /*if (gridPos.x >= 0 && gridPos.x < roomSize.x && gridPos.y >= 0 && gridPos.y < roomSize.y)
                {
                    TemplateTile editedTile = createdTemplate.tiles[gridPos.x, gridPos.y];
                    if (editedTile != null && editedTile != tileEditing)
                    {
                        tileEditing = editedTile;
                        Destroy(propertyEditor);
                        CreateNewPropertyEditor();
                    }
                }
                else
                {
                    tileEditing = null;
                    Destroy(propertyEditor);
                }*/
            }
            else
            {
                if (gridPos.x >= 1 && gridPos.x < roomSize.x - 1 && gridPos.y >= 1 && gridPos.y < roomSize.y - 1)
                {
                    createdTemplate.tiles[gridPos.x][gridPos.y].sprite = heldTile.sprite;
                    createdTemplate.tiles[gridPos.x][gridPos.y].preferredTile = heldTile.preferredTile;
                    createdTemplate.tiles[gridPos.x][gridPos.y].tileType = heldTile.tileType;
                    sprites[gridPos.x, gridPos.y] = new GameObject();
                    sprites[gridPos.x, gridPos.y].transform.parent = spriteContainer.transform;
                    sprites[gridPos.x, gridPos.y].transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
                    sprites[gridPos.x, gridPos.y].AddComponent<SpriteRenderer>().sprite = heldTile.sprite;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);
            if (gridPos.x >= 1 && gridPos.x < roomSize.x - 1 && gridPos.y >= 1 && gridPos.y < roomSize.y - 1)
            {
                Destroy(sprites[gridPos.x, gridPos.y]);
                sprites[gridPos.x, gridPos.y] = null;
                createdTemplate.tiles[gridPos.x][gridPos.y] = new TemplateTile();
            }
        }


        if (heldTile == null)
        {
            heldTileSocket.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            heldTileSocket.GetComponent<SpriteRenderer>().enabled = true;
            Sprite sprite;
            if (heldTile.preferredTile == null)
            {
                sprite = heldTile.sprite;
            }
            else if (heldTile.preferredTile.spawnedObject.GetComponent<SpriteRenderer>().sprite != null)
            {
                sprite = heldTile.preferredTile.spawnedObject.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                sprite = nullSprite;
            }
            heldTileSocket.GetComponent<SpriteRenderer>().sprite = sprite;
            heldTileSocket.transform.position = QuantizeMousePos(Input.mousePosition);
        }
    }

    /// <summary>
    /// Converts the mouse position to world space, then rounds it to an integer
    /// </summary>
    /// <param name="mousePos"> The mouse position </param>
    /// <returns> The quantized world position </returns>
    Vector3 QuantizeMousePos(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 quantizedWorldPos = new Vector3();
        quantizedWorldPos.x = Mathf.Round(worldPos.x);
        quantizedWorldPos.y = Mathf.Round(worldPos.y);
        return quantizedWorldPos;
    }

    /// <summary>
    /// Converts a moues position to a grid position 
    /// </summary>
    /// <param name="mousePos"> The mouse position </param>
    /// <returns> The grid position </returns>
    Vector2Int MousePosToGridPos(Vector3 mousePos)
    {
        Vector3 quantizedMousePos = QuantizeMousePos(mousePos);
        Vector2Int gridPos = new Vector2Int();
        gridPos.x = (int) quantizedMousePos.x + roomSize.x / 2;
        gridPos.y = (int) quantizedMousePos.y + roomSize.y / 2;
        return gridPos;
    }
}
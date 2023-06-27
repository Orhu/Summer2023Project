using UnityEngine.UI;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Handles input and button presses for the template creator
    /// </summary>
    [RequireComponent(typeof(TemplateCreator))]
    public class TemplateCreatorInput : MonoBehaviour
    {
        // The UI for choosing the template tiles
        private GameObject templateTileChooser;

        // The tile being held
        private TemplateTile heldTile;

        // The socket to display the tile being held
        private GameObject heldTileSocket;

        // Whether or not the mouse has been moved since the last time someone held down a mouse button
        private bool mouseMoved;

        // The mouse position when right click began being held
        private Vector3 mouseStartPosition;

        // The mouse position on the last frame
        private Vector3 lastMousePosition;

        // A reference to the template creator
        private TemplateCreator templateCreator;

        // A reference to the template camera
        private TemplateCreatorCamera templateCamera;

        /// <summary>
        /// Initialize variables
        /// </summary>
        private void Start()
        {
            templateCreator = GetComponent<TemplateCreator>();
            templateCamera = templateCreator.templateCamera;
            templateTileChooser = Instantiate(templateCreator.templateTileChooser);
            templateTileChooser.transform.parent = transform;
            templateTileChooser.SetActive(true);
            InitializeHeldTileSocket();
            InitializeTileHUD();
        }

        /// <summary>
        /// Initializes the tile hud, where you can pick tiles to place
        /// </summary>
        private void InitializeTileHUD()
        {
            Button saveButton = templateTileChooser.GetComponentInChildren<Button>();
            saveButton.onClick.AddListener(templateCreator.SaveTemplate);

            VerticalLayoutGroup layout = templateTileChooser.GetComponentInChildren<VerticalLayoutGroup>();
            foreach (TileTypeToGenericTile genericTile in templateCreator.genericTiles.tileTypesToGenericTiles)
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
                    imageComponent.sprite = templateCreator.nullSprite;
                    templateTile.sprite = templateCreator.nullSprite;
                }

                templateTile.tileType = genericTile.tileType;

                buttonComponent.onClick.AddListener(() => OnHeldTileChosen(templateTile));
                button.SetActive(true);
            }
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
        public void OnHeldTileChosen(TemplateTile tile)
        {
            heldTile = tile;
        }

        /// <summary>
        /// Handles placing and removing tiles
        /// </summary>
        void Update()
        {
            if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);
                if (templateCreator.IsGridPosOutsideBounds(gridPos))
                {
                    heldTile = null;
                }
                else if (heldTile == null)
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
                    templateCreator.PlaceTile(heldTile, gridPos);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                mouseStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                if (Input.mousePosition != mouseStartPosition)
                {
                    mouseMoved = true;
                    templateCamera.Pan(Camera.main.ScreenToWorldPoint(lastMousePosition) - Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            templateCamera.Zoom(Input.mouseScrollDelta);

            if (Input.GetMouseButtonUp(1))
            {
                if (mouseMoved)
                {
                    mouseMoved = false;
                }
                else
                {
                    Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);
                    templateCreator.EraseTile(gridPos);
                }
            }

            UpdateHeldTile();
            lastMousePosition = Input.mousePosition;
        }

        /// <summary>
        /// Updates the held tile
        /// </summary>
        private void UpdateHeldTile()
        {
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
                    sprite = templateCreator.nullSprite;
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
        /// Converts a mouse position to a grid position 
        /// </summary>
        /// <param name="mousePos"> The mouse position </param>
        /// <returns> The grid position </returns>
        Vector2Int MousePosToGridPos(Vector3 mousePos)
        {
            Vector3 quantizedMousePos = QuantizeMousePos(mousePos);
            Vector2Int gridPos = new Vector2Int();
            gridPos.x = (int) quantizedMousePos.x + templateCreator.roomSize.x / 2;
            gridPos.y = (int) quantizedMousePos.y + templateCreator.roomSize.y / 2;
            return gridPos;
        }

    }
}
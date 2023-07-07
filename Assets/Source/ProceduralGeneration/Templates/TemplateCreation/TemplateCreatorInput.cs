using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cardificer
{
    /// <summary>
    /// Handles input and button presses for the template creator
    /// </summary>
    [RequireComponent(typeof(TemplateCreator))]
    public class TemplateCreatorInput : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color previewColor;

        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color selectedColor;

        // Shows the null sprite when the held tile doesn't have a sprite component
        private GameObject nullSprite;

        // The mouse position on the last frame
        private Vector3 lastMousePosition;

        // A reference to the template creator
        private TemplateCreator templateCreator;

        // A reference to the template camera
        private TemplateCreatorCamera templateCamera;

        // The last selected game object (so it doesn't print a warning 9 million times)
        private string lastSelectedObjectName = "";

        // The currently selected object
        GameObject selectedObject;

        // The tile being held
        private GameObject _heldTile;
        private GameObject heldTile
        {
            set
            {
                if (_heldTile != null)
                {
                    Destroy(_heldTile);
                }

                _heldTile = value;
                if (value != null)
                {
                    Selection.activeGameObject = _heldTile;
                }

                if (_heldTile == null) 
                {
                    nullSprite.SetActive(false);
                    return;
                }

                foreach (SpriteRenderer spriteRenderer in _heldTile.GetComponents<SpriteRenderer>())
                {
                    spriteRenderer.color = previewColor;
                    spriteRenderer.sortingOrder++;
                }

                nullSprite.SetActive(heldTile?.GetComponent<SpriteRenderer>() == null);
            }
            get => _heldTile;
        }

        // The template being held
        private GameObject _heldTemplate;
        private GameObject heldTemplate
        {
            set
            {
                if (_heldTemplate != null)
                {
                    Destroy(_heldTemplate);
                }
                if (value == null)
                {
                    Destroy(nullSpritesContainer);
                    nullSpritesContainer = null;
                    _heldTemplate = null;
                }
                else
                {
                    nullSpritesContainer = new GameObject();
                    nullSpritesContainer.name = "Null Sprites Container";
                    foreach (SpriteRenderer spriteRenderer in value.GetComponentsInChildren<SpriteRenderer>())
                    {
                        spriteRenderer.color = previewColor;
                    }

                    foreach (Tile tileComponent in value.GetComponents<Tile>())
                    {
                        if (tileComponent.GetComponent<SpriteRenderer>() == null || tileComponent.GetComponent<SpriteRenderer>().sprite == null)
                        {
                            GameObject createdNullSprite = Instantiate(nullSprite);
                            createdNullSprite.SetActive(true);
                            createdNullSprite.transform.parent = nullSpritesContainer.transform;
                            createdNullSprite.transform.localPosition = new Vector3(tileComponent.gridLocation.x, tileComponent.gridLocation.y, 0);
                            createdNullSprite.GetComponent<SpriteRenderer>().color = previewColor;
                        }
                    }

                }
                _heldTemplate = value;
            }
            get => _heldTemplate;
        }

        // The container to hold (temporary) null sprites
        GameObject nullSpritesContainer;

        /// <summary>
        /// Initialize variables
        /// </summary>
        public void Initialize()
        {
            templateCreator = GetComponent<TemplateCreator>();
            templateCamera = templateCreator.templateCamera;
            nullSprite = templateCreator.nullSpriteObject;
            nullSprite.SetActive(false);
        }

        /// <summary>
        /// Handles placing and removing tiles
        /// </summary>
        private void Update()
        {
            Vector3 view = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
            bool isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;

            Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);

            if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // Placing template
                if (heldTemplate != null && !templateCreator.IsGridPosOutsideBounds(gridPos))
                {
                    templateCreator.LoadTemplate(heldTemplate.GetComponent<Template>());
                    heldTemplate = null;
                }

                if (templateCreator.IsGridPosOutsidePathfindingBounds(gridPos) || templateCreator.GetTile(gridPos) == null)
                {
                    DeselectObject();
                    SelectObject(templateCreator.gameObject);
                    heldTile = null;
                    heldTemplate = null;

                }
                else if (heldTile == null)
                {
                    DeselectObject();
                    SelectObject(templateCreator.GetTile(gridPos).gameObject);
                }
            }

            // Placing tiles
            if (Input.GetMouseButton(0) && !isOutside)
            {
                if (heldTile != null 
                    && !templateCreator.IsGridPosOutsidePathfindingBounds(gridPos) 
                    && (templateCreator.GetTile(gridPos) == null || templateCreator.GetTile(gridPos).name != heldTile.name))
                {
                    foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = Color.white;
                        spriteRenderer.sortingOrder--;
                    }
                    templateCreator.PlaceTile(heldTile, gridPos);
                    foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = previewColor;
                        spriteRenderer.sortingOrder++;
                    }
                    DeselectObject();
                    SelectObject(templateCreator.GetTile(gridPos).gameObject);
                }
            }

            // Panning
            if ((Input.GetMouseButton(2) || (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift))) && !isOutside)
            {
                templateCamera.Pan(Camera.main.ScreenToWorldPoint(lastMousePosition) - Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            // Scrolling
            if (!isOutside)
            {
                templateCamera.Zoom(Input.mouseScrollDelta);
            }

            // Erasing
            if (Input.GetMouseButton(1) && !isOutside && templateCreator.GetTile(gridPos) != null && !Input.GetKey(KeyCode.LeftShift))
            {
                templateCreator.EraseTile(gridPos);
            }


            // Copying
            if (Input.GetKeyDown(KeyCode.Q) && !isOutside)
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject != heldTile && Selection.activeGameObject.activeInHierarchy)
                {
                    foreach (SpriteRenderer spriteRenderer in Selection.activeGameObject.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = Color.white;
                    }
                }

                Tile tile = templateCreator.GetTile(gridPos);

                heldTile = tile == null ? null : (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tile.gameObject));
                PrefabUtility.SetPropertyModifications(heldTile, PrefabUtility.GetPropertyModifications(tile));
            }

            UpdateHeldTile();
            lastMousePosition = Input.mousePosition;
        }

        /// <summary>
        /// Selects the given object
        /// </summary>
        /// <param name="selectedObject"> The selected object </param>
        private void SelectObject(GameObject selectedObject)
        {
            Selection.activeGameObject = selectedObject.gameObject;
            this.selectedObject = selectedObject;
            foreach (SpriteRenderer spriteRenderer in Selection.activeGameObject.GetComponents<SpriteRenderer>())
            {
                spriteRenderer.color = selectedColor;
            }
        }

        /// <summary>
        /// Deselects the selected object 
        /// </summary>
        public void DeselectObject()
        {
            if (selectedObject == null) { return; }

            foreach (SpriteRenderer spriteRenderer in selectedObject.GetComponents<SpriteRenderer>())
            {
                spriteRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// Updates the held tile
        /// </summary>
        private void UpdateHeldTile()
        {
            #if UNITY_EDITOR
            // Select from project tab
            if (
                Selection.activeGameObject != null 
                && !Selection.activeGameObject.activeInHierarchy 
                && (heldTile == null || Selection.activeGameObject.name != heldTile.name)
                )
            {
                GameObject selectedObject = (GameObject) PrefabUtility.InstantiatePrefab(Selection.activeGameObject);
                selectedObject.name = Selection.activeGameObject.name;

                if (selectedObject.GetComponent<Tile>() == null)
                {
                    if (selectedObject.name != lastSelectedObjectName)
                    {
                        if (selectedObject.GetComponent<Template>() == null)
                        {
                            Debug.LogWarning("The selected held object to add must have a tile component! If you are trying to load a template, it must have a template component!");
                        }
                        lastSelectedObjectName = selectedObject.name;
                        heldTile = null;
                    }
                    Destroy(selectedObject);
                }
                else
                {
                    DeselectObject();
                    heldTile = selectedObject;
                }
            }

            // Select template from project tab
            if (
                heldTile == null &&
                Selection.activeGameObject != null
                && !Selection.activeGameObject.activeInHierarchy
                && (heldTemplate == null || Selection.activeGameObject.name != heldTemplate.name)
                )
            {
                GameObject selectedObject = (GameObject) PrefabUtility.InstantiatePrefab(Selection.activeGameObject);
                selectedObject.name = Selection.activeGameObject.name;

                if (selectedObject.GetComponent<Template>() == null)
                {
                    Destroy(selectedObject);
                    heldTemplate = null;
                }
                else
                {
                    heldTemplate = selectedObject;
                }
            }
#endif

            if (heldTile == null)
            {
                return;
            }

            heldTile.transform.position = QuantizeMousePos(Input.mousePosition);
            nullSprite.transform.position = QuantizeMousePos(Input.mousePosition);
        }

        /// <summary>
        /// Converts the mouse position to world space, then rounds it to an integer
        /// </summary>
        /// <param name="mousePos"> The mouse position </param>
        /// <returns> The quantized world position </returns>
        private Vector3 QuantizeMousePos(Vector3 mousePos)
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
        private Vector2Int MousePosToGridPos(Vector3 mousePos)
        {
            Vector3 quantizedMousePos = QuantizeMousePos(mousePos);
            Vector2Int gridPos = new Vector2Int();
            gridPos.x = (int) quantizedMousePos.x + templateCreator.roomSize.x / 2;
            gridPos.y = (int) quantizedMousePos.y + templateCreator.roomSize.y / 2;
            return gridPos;
        }
#endif
    }
}
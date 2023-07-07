using UnityEngine;
using UnityEngine.InputSystem;
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
        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color previewColor;

        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color selectedColor;

#if UNITY_EDITOR
        // Shows the null sprite when the held tile doesn't have a sprite component
        private GameObject nullSprite;

        // Whether or not the select button is down
        private bool selectButtonPressed = false;

        // Whether or not the pan button is down
        private bool panButtonPressed = false;

        // Whether or not the erase button is down
        private bool eraseButtonPressed = false;

        // The mouse position on the last frame
        private Vector3 lastMousePosition;

        // A reference to the template creator
        private TemplateCreator templateCreator;

        // A reference to the template camera
        private TemplateCreatorCamera templateCamera;

        // The last selected game object (so it doesn't print a warning 9 million times)
        private string lastSelectedObjectName = "";

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
                Selection.activeGameObject = _heldTile;

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
            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;

            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);
           
            // Placing tiles
            if (selectButtonPressed && !isOutside)
            {
                if (heldTile != null 
                    && !templateCreator.IsGridPosOutsideBounds(gridPos) 
                    && (templateCreator.GetTile(gridPos) == null || templateCreator.GetTile(gridPos).name != heldTile.name))
                {
                    foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = Color.white;
                        spriteRenderer.sortingOrder--;
                    }
                    templateCreator.PlaceTile(heldTile.GetComponent<Tile>(), gridPos);
                    foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = previewColor;
                        spriteRenderer.sortingOrder++;
                    }
                }
            }

            // Panning
            if (panButtonPressed && !isOutside)
            {
                templateCamera.Pan(Camera.main.ScreenToWorldPoint(lastMousePosition) - Camera.main.ScreenToWorldPoint(Mouse.current.position.value));
            }

            // Erasing
            if (eraseButtonPressed && !isOutside && templateCreator.GetTile(gridPos) != null && !panButtonPressed)
            {
                templateCreator.EraseTile(gridPos);
            }

            UpdateHeldTile();
            lastMousePosition = Mouse.current.position.value;
        }

        /// <summary>
        /// Handles select up
        /// </summary>
        public void OnSelectUp()
        {
            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);
            if (!isOutside && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {

                if (Selection.activeGameObject != null && Selection.activeGameObject != heldTile)
                {
                    foreach (SpriteRenderer spriteRenderer in Selection.activeGameObject.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = Color.white;
                    }
                }

                // Unselecting tile
                if (templateCreator.IsGridPosOutsideBounds(gridPos))
                {
                    heldTile = null;
                }

                // Selecting tile
                else
                {
                    #if UNITY_EDITOR
                    if (templateCreator.GetTile(gridPos) != null)
                    {
                        Selection.activeGameObject = templateCreator.GetTile(gridPos).gameObject;
                        foreach (SpriteRenderer spriteRenderer in Selection.activeGameObject.GetComponents<SpriteRenderer>())
                        {
                            spriteRenderer.color = selectedColor;
                        }
                    }
                    #endif
                }
            }
        }

        /// <summary>
        /// Called when the select button is pressed or released
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnSelect(InputValue input)
        {
            selectButtonPressed = input.isPressed;
            if (!input.isPressed)
            {
                OnSelectUp();
            }
        }

        /// <summary>
        /// Called when the pan button is pressed or released
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnPan(InputValue input)
        {
            panButtonPressed = input.isPressed;
        }

        /// <summary>
        /// Called when the erase button is pressed or released
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnErase(InputValue input)
        {
            eraseButtonPressed = input.isPressed;
        }

        /// <summary>
        /// Handles zoom input
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnZoom(InputValue input)
        {
            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            // Scrolling
            if (!isOutside)
            {
                templateCamera.Zoom(new Vector2(0, input.Get<float>() * Time.deltaTime));
            }
        }

        /// <summary>
        /// Called when the copy button is pressed
        /// </summary>
        public void OnCopy()
        {
            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);

            if (!isOutside)
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject != heldTile)
                {
                    foreach (SpriteRenderer spriteRenderer in Selection.activeGameObject.GetComponents<SpriteRenderer>())
                    {
                        spriteRenderer.color = Color.white;
                    }
                }

                Tile tile = templateCreator.GetTile(gridPos);
                heldTile = tile == null ? null : Instantiate(tile.gameObject);
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
                GameObject selectedObject = Instantiate(Selection.activeGameObject);
                selectedObject.name = Selection.activeGameObject.name;


                if (selectedObject.GetComponent<Tile>() == null)
                {
                    if (selectedObject.name != lastSelectedObjectName)
                    {
                        lastSelectedObjectName = selectedObject.name;
                        Debug.LogWarning("The object placed in a template must have a tile component!");
                        heldTile = null;
                    }
                    Destroy(selectedObject);
                }
                else
                {
                    heldTile = selectedObject;
                }
            }
#endif

            if (heldTile == null)
            {
                return;
            }

            //heldTile.transform.position = QuantizeMousePos(Input.mousePosition);
            //nullSprite.transform.position = QuantizeMousePos(Input.mousePosition);
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
            gridPos.x = (int)quantizedMousePos.x + templateCreator.roomSize.x / 2;
            gridPos.y = (int)quantizedMousePos.y + templateCreator.roomSize.y / 2;
            return gridPos;
        }
#endif
    }
}
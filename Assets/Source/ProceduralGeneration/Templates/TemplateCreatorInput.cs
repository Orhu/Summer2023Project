using UnityEngine.UI;
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
        // The tile being held
        private GameObject heldTile;

        // Shows the null sprite when the held tile doesn't have a sprite component
        private GameObject nullSprite;

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

        // Whether or not we are in placing mode
        bool inPlacingMode;

        /// <summary>
        /// Toggles whether tiles are being placed or not
        /// </summary>
        public void TogglePlacingMode()
        {
            inPlacingMode = !inPlacingMode;
        }

        /// <summary>
        /// Initialize variables
        /// </summary>
        private void Start()
        {
            templateCreator = GetComponent<TemplateCreator>();
            templateCamera = templateCreator.templateCamera;
            nullSprite = templateCreator.nullSpriteObject;
        }

        /// <summary>
        /// Handles placing and removing tiles
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Vector2Int gridPos = MousePosToGridPos(Input.mousePosition);

                // Unselecting tile
                if (templateCreator.IsGridPosOutsideBounds(gridPos))
                {
                    heldTile = null;
                    #if UNITY_EDITOR
                    Selection.activeGameObject = templateCreator.gameObject;
                    #endif
                }

                // Placing tile
                if (inPlacingMode)
                {
                    if (heldTile != null)
                    {
                        templateCreator.PlaceTile(heldTile.GetComponent<Tile>(), gridPos);
                    }
                }

                // Selecting tile
                else
                {
                    #if UNITY_EDITOR
                    if (templateCreator.GetTile(gridPos) != null)
                    {
                        Selection.activeGameObject = templateCreator.GetTile(gridPos).gameObject;
                    }
                    #endif
                }
            }

            // Panning
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

            // Scrolling
            templateCamera.Zoom(Input.mouseScrollDelta);


            // Erasing
            if (Input.GetMouseButtonUp(1))
            {
                if (mouseMoved)
                {
                    mouseMoved = false;
                }
                else if (inPlacingMode)
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
            if (!inPlacingMode)
            {
                Destroy(heldTile);
                heldTile = null;
                return;
            }

            #if UNITY_EDITOR
            if (Selection.activeGameObject != null && Selection.activeGameObject != templateCreator)
            {
                if (Selection.activeGameObject.GetComponent<Tile>() == null)
                {
                    Debug.LogWarning("The object placed in a template must have a tile component!");
                    Destroy(heldTile);
                    heldTile = null;
                }
                else
                {
                    heldTile = Selection.activeGameObject;
                    if (heldTile.GetComponent<SpriteRenderer>() == null)
                    {
                        nullSprite.SetActive(true);
                    }
                }
            }
            #endif

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
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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

        #region Initialization

        // A reference to the template creator
        private TemplateCreator templateCreator;

        // A reference to the template camera
        private TemplateCreatorCamera templateCamera;

        /// <summary>
        /// Initialize variables
        /// </summary>
        public void Initialize()
        {
            templateCreator = GetComponent<TemplateCreator>();
            templateCamera = templateCreator.templateCamera;
            nullSprite = Instantiate(templateCreator.nullSpriteObject);
            nullSprite.SetActive(false);
            nullSprite.GetComponent<SpriteRenderer>().color = previewColor;

            undoHistory = new Stack<List<UndoRedoAction>>();
            redoHistory = new Stack<List<UndoRedoAction>>();
            currentAction = new List<UndoRedoAction>();
            tempObjectsContainer = new GameObject();
            tempObjectsContainer.SetActive(false);
            tempObjectsContainer.name = "Temp Objects Container";
            ClearRedoHistory();
            ClearUndoHistory();
        }

        #endregion

        #region Input parsing

        // Whether or not the select button is down
        private bool selectButtonPressed = false;

        // Whether or not the pan button is down
        private bool panButtonPressed = false;

        // Whether or not the erase button is down
        private bool eraseButtonPressed = false;

        // Whether or not the mouse is currently over the template creator UI
        private bool mouseOverUI = false;

        // The last selected game object (so it doesn't print a warning 9 million times)
        private string lastSelectedObjectName = "";

        // The currently selected object
        private GameObject selectedObject;

        /// <summary>
        /// Handles placing and removing tiles
        /// </summary>
        private void Update()
        {
            mouseOverUI = EventSystem.current.IsPointerOverGameObject();

            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;

            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);

            // Placing tiles
            if (selectButtonPressed && !isOutside && !mouseOverUI)
            {
                if (heldTile != null
                    && !templateCreator.IsGridPosOutsidePathfindingBounds(gridPos)
                    && (templateCreator.GetObject(gridPos) == null || templateCreator.GetObject(gridPos).name != heldTile.name))
                {
                    PlaceTile(heldTile, gridPos);
                }
            }

            // Panning
            if (panButtonPressed && !isOutside && !mouseOverUI)
            {
                templateCamera.Pan(Camera.main.ScreenToWorldPoint(lastMousePosition) - Camera.main.ScreenToWorldPoint(Mouse.current.position.value));
            }

            // Erasing
            if (eraseButtonPressed && !isOutside && templateCreator.GetObject(gridPos) != null && !panButtonPressed && !mouseOverUI)
            {
                EraseTile(gridPos);
            }

            UpdateHeldTile();
            lastMousePosition = Mouse.current.position.value;
        }

        /// <summary>
        /// Handles select up
        /// </summary>
        public void OnSelectUp()
        {
            if (!templateCreator.IsValid()) { return; }

            if (currentAction.Count > 0)
            {
                List<UndoRedoAction> completedAction = new List<UndoRedoAction>();
                foreach (UndoRedoAction action in currentAction)
                {
                    completedAction.Add(action);
                    foreach (GameObject relevantObject in action.relevantObjects)
                    {
                        relevantObject.transform.parent = undoObjectsContainer.transform;
                    }
                }
                undoHistory.Push(completedAction);
                currentAction.Clear();
            }

            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);

            if (!isOutside && !mouseOverUI)
            {
                // Placing template
                if (heldTemplate != null && !templateCreator.IsGridPosOutsideBounds(gridPos))
                {
                    templateCreator.LoadTemplate(heldTemplate.GetComponent<Template>());
                    DeselectObject();
                    SelectObject(templateCreator.gameObject);
                    heldTile = null;
                    heldTemplate = null;
                    ClearUndoHistory();
                    ClearRedoHistory();
                    return;
                }

                if (templateCreator.IsGridPosOutsidePathfindingBounds(gridPos) || templateCreator.GetObject(gridPos) == null)
                {
                    DeselectObject();
                    SelectObject(templateCreator.gameObject);
                    heldTile = null;
                    heldTemplate = null;

                }
                else if (heldTile == null)
                {
                    DeselectObject();
                    SelectObject(templateCreator.GetObject(gridPos).gameObject);
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

            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);

            if (!mouseOverUI && !isOutside && input.isPressed 
                && !templateCreator.IsGridPosOutsidePathfindingBounds(gridPos)
                && heldTile != null)
            {
                ClearRedoHistory();
            }

            if (mouseOverUI)
            {
                heldTemplate = null;
            }
        }

        /// <summary>
        /// Called when the erase button is pressed or released
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnErase(InputValue input)
        {
            heldTemplate = null;
            eraseButtonPressed = input.isPressed;

            if (!input.isPressed)
            {
                if (currentAction.Count > 0)
                {
                    List<UndoRedoAction> completedAction = new List<UndoRedoAction>();
                    foreach (UndoRedoAction action in currentAction)
                    {
                        completedAction.Add(action);
                        foreach (GameObject relevantObject in action.relevantObjects)
                        {
                            relevantObject.transform.parent = undoObjectsContainer.transform;
                        }
                    }
                    undoHistory.Push(completedAction);
                    currentAction.Clear();
                }
            }

            Vector3 mouseViewportPos = templateCamera.GetComponent<Camera>().ScreenToViewportPoint(Mouse.current.position.value);
            bool isOutside = mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1;
            Vector2Int gridPos = MousePosToGridPos(Mouse.current.position.value);

            if (!mouseOverUI && !isOutside && input.isPressed
                && !templateCreator.IsGridPosOutsidePathfindingBounds(gridPos)
                && heldTile != null)
            {
                ClearRedoHistory();
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
                if (Selection.activeGameObject != null && Selection.activeGameObject != heldTile && Selection.activeGameObject.activeInHierarchy)
                {
                    DeselectObject();
                }

                GameObject tile = templateCreator.GetObject(gridPos);             

                GameObject newTile = tile == null ? null : (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tile));
                PrefabUtility.SetPropertyModifications(newTile, PrefabUtility.GetPropertyModifications(tile));
                heldTile = newTile;
                if (heldTile == null) { return; }

                foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
                {
                    spriteRenderer.color = previewColor;
                    spriteRenderer.sortingOrder++;
                }
            }
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

        #endregion

        #region Placing and Erasing

        [Header("Placing and Erasing")]

        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color previewColor;

        [Tooltip("The color of the tile placement preview")]
        [SerializeField] private Color selectedColor;

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

                foreach (MonoBehaviour component in _heldTile.GetComponents<MonoBehaviour>())
                {
                    component.enabled = false;
                }

                if (_heldTile.GetComponent<Rigidbody2D>() != null)
                {
                    _heldTile.GetComponent<Rigidbody2D>().simulated = false;
                }

                if (heldTile == null) { return; }

                if (heldTile.GetComponent<SpriteRenderer>() == null)
                {
                    nullSprite.SetActive(true);
                }
                else
                {
                    heldTile.GetComponent<SpriteRenderer>().enabled = true;
                }
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
                    nullSpritesContainer.transform.localPosition = (Vector2) (-value.GetComponent<Template>().roomSize / 2);
                    foreach (SpriteRenderer spriteRenderer in value.GetComponentsInChildren<SpriteRenderer>())
                    {
                        spriteRenderer.color = previewColor;
                    }

                    for (int i = 0; i < value.GetComponent<Template>().GetLayers().Count; i++)
                    {
                        for (int j = 0; j < value.GetComponent<Template>().roomSize.x; j++)
                        {
                            for (int k = 0; k < value.GetComponent<Template>().roomSize.y; k++)
                            {
                                if (value.GetComponent<Template>()[i, j, k] == null) { continue; }

                                foreach (MonoBehaviour component in value.GetComponent<Template>()[i, j, k].GetComponents<MonoBehaviour>())
                                {
                                    component.enabled = false;
                                }

                                if (value.GetComponent<Template>()[i, j, k].GetComponent<Rigidbody2D>() != null)
                                {
                                    value.GetComponent<Template>()[i, j, k].GetComponent<Rigidbody2D>().simulated = false;
                                }

                                if (value.GetComponent<Template>()[i, j, k].GetComponent<SpriteRenderer>() != null && value.GetComponent<Template>()[i, j, k].GetComponent<SpriteRenderer>().sprite != null)
                                {
                                    value.GetComponent<Template>()[i, j, k].GetComponent<SpriteRenderer>().enabled = true;
                                    continue;
                                }

                                GameObject createdNullSprite = Instantiate(nullSprite);
                                createdNullSprite.SetActive(true);
                                createdNullSprite.transform.parent = nullSpritesContainer.transform;
                                createdNullSprite.transform.localPosition = new Vector3(j, k, 0);
                                createdNullSprite.GetComponent<SpriteRenderer>().color = previewColor;
                            }
                        }
                    }
                }
                _heldTemplate = value;
            }
            get => _heldTemplate;
        }

        // The container to hold (temporary) null sprites
        private GameObject nullSpritesContainer;

        // Shows the null sprite when the held tile doesn't have a sprite component
        private GameObject nullSprite;

        /// <summary>
        /// Erases the tile at the given position
        /// </summary>
        /// <param name="gridPos"> The position </param>
        /// <param name="addToUndoHistory"> Whether or not to add this action to the undo history; Adds it to the redo history instead if false </param>
        public void EraseTile(Vector2Int gridPos)
        {
            GameObject tile = templateCreator.GetObject(gridPos);
            UndoRedoAction action = new UndoRedoAction();
            GameObject undoActionTile = (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tile));
            PrefabUtility.SetPropertyModifications(undoActionTile, PrefabUtility.GetPropertyModifications(tile));

            if (!templateCreator.EraseTile(gridPos)) 
            {
                Destroy(undoActionTile);
                return; 
            }

            foreach (MonoBehaviour component in undoActionTile.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }

            if (undoActionTile.GetComponent<Rigidbody2D>() != null)
            {
                undoActionTile.GetComponent<Rigidbody2D>().simulated = false;
            }

            if (undoActionTile.GetComponent<SpriteRenderer>() != null)
            {
                undoActionTile.GetComponent<SpriteRenderer>().enabled = true;
            }

            undoActionTile.name = tile.name;
            undoActionTile.transform.parent = tempObjectsContainer.transform;
            action.redoAction = () => EraseTile(gridPos);
            action.undoAction = () => PlaceTile(undoActionTile, gridPos);
            action.relevantObjects.Add(undoActionTile);
            currentAction.Add(action);
        }

        /// <summary>
        /// Places a tile at the given position
        /// </summary>
        /// <param name="tile"> The tile to place </param>
        /// <param name="gridPos"> The grid pos </param>
        public void PlaceTile(GameObject tile, Vector2Int gridPos)
        {
            foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
            {
                spriteRenderer.color = Color.white;
                spriteRenderer.sortingOrder--;
            }

            if (!templateCreator.PlaceTile(tile, gridPos)) { return; }

            UndoRedoAction action = new UndoRedoAction();
            GameObject undoActionTile = (GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tile));
            PrefabUtility.SetPropertyModifications(undoActionTile, PrefabUtility.GetPropertyModifications(tile));

            foreach (MonoBehaviour component in undoActionTile.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }

            if (undoActionTile.GetComponent<Rigidbody2D>() != null)
            {
                undoActionTile.GetComponent<Rigidbody2D>().simulated = false;
            }

            if (undoActionTile.GetComponent<SpriteRenderer>() != null)
            {
                undoActionTile.GetComponent<SpriteRenderer>().enabled = true;
            }

            undoActionTile.name = tile.name;
            undoActionTile.transform.parent = tempObjectsContainer.transform;
            action.redoAction = () => PlaceTile(undoActionTile, gridPos);
            action.undoAction = () => EraseTile(gridPos);
            action.relevantObjects.Add(undoActionTile);
            currentAction.Add(action);

            foreach (SpriteRenderer spriteRenderer in heldTile.GetComponents<SpriteRenderer>())
            {
                spriteRenderer.color = previewColor;
                spriteRenderer.sortingOrder++;
            }
            DeselectObject();
            SelectObject(templateCreator.GetObject(gridPos).gameObject);
        }

        /// <summary>
        /// Clears the template
        /// </summary>
        public void Clear()
        {
            templateCreator.Reload();
            ClearUndoHistory();
            ClearRedoHistory();
        }

        /// <summary>
        /// Updates the held tile
        /// </summary>
        private void UpdateHeldTile()
        {
            // Select from project tab
            if (
                Selection.activeGameObject != null
                && !Selection.activeGameObject.activeInHierarchy
                && (heldTile == null || Selection.activeGameObject.name != heldTile.name)
                )
            {
                GameObject selectedObject = (GameObject) PrefabUtility.InstantiatePrefab(Selection.activeGameObject);
                selectedObject.name = Selection.activeGameObject.name;

                if (selectedObject.GetComponent<Template>() == null)
                {
                    if (templateCreator.activeLayer == 0 && selectedObject.GetComponent<Tile>() == null)
                    {
                        if (selectedObject.name != lastSelectedObjectName)
                        {
                            Debug.LogWarning("The selected held object to add must have a tile component when trying to add to the pathfinding layer! If you are trying to load a template, it must have a template component!");
                            lastSelectedObjectName = selectedObject.name;
                            heldTile = null;
                        }
                        selectedObject.SetActive(false);
                        Destroy(selectedObject);
                    }
                    else
                    {
                        DeselectObject();
                        heldTile = selectedObject;
                        heldTemplate = null;
                    }
                }
                else
                {
                    DeselectObject();
                    selectedObject.SetActive(false);
                    Destroy(selectedObject);
                    heldTile = null;
                }
            }

            // Select template from project tab
            if (
                Selection.activeGameObject != null
                && !Selection.activeGameObject.activeInHierarchy
                && (heldTemplate == null || Selection.activeGameObject.name != heldTemplate.name)
                )
            {
                GameObject selectedObject = (GameObject) PrefabUtility.InstantiatePrefab(Selection.activeGameObject);
                if (selectedObject != null)
                {
                    selectedObject.name = Selection.activeGameObject.name;

                    if (selectedObject.GetComponent<Template>() == null)
                    {
                        selectedObject.SetActive(false);
                        Destroy(selectedObject);
                        heldTemplate = null;
                    }
                    else
                    {
                        heldTemplate = selectedObject;
                        heldTile = null;
                    }
                }
            }

            if (heldTile == null)
            {
                return;
            }

            heldTile.transform.position = QuantizeMousePos(Mouse.current.position.value);
            nullSprite.transform.position = QuantizeMousePos(Mouse.current.position.value);
        }

        #endregion

        #region Layers

        [Tooltip("The layer UI")]
        [SerializeField] private GameObject layerUI;

        [Tooltip("The layer UI container")]
        [SerializeField] private VerticalLayoutGroup layerUIContainer;

        // The list of layer UIs
        private List<GameObject> layerUIs;

        /// <summary>
        /// Adds a new layer
        /// </summary>
        public void AddLayer()
        {
            templateCreator.AddLayer("");
            AddLayerUI();
            ClearUndoHistory();
            ClearRedoHistory();
        }

        /// <summary>
        /// Removes the given layer 
        /// </summary>
        /// <param name="layer"> The layer to remove </param>
        public void RemoveLayer(int layer)
        {
            if (layer == templateCreator.activeLayer)
            {
                layerUIs[0].GetComponent<TemplateLayerUI>().Activate();
                ActivateLayer(0);
            }

            GameObject removedLayerUI = layerUIs[layer];
            layerUIs.RemoveAt(layer);
            Destroy(removedLayerUI);

            for (int i = 0; i < layerUIs.Count; i++)
            {
                // Lambdas be weird
                int currentIndex = i;
                TemplateLayerUI layerUIComponent = layerUIs[i].GetComponent<TemplateLayerUI>();
                layerUIComponent.onLayerActivated = () => ActivateLayer(currentIndex);
                layerUIComponent.onLayerHiddenToggled = () => templateCreator.ToggleLayerVisibility(currentIndex);
                layerUIComponent.onLayerNamed = (string name) => templateCreator.RenameLayer(currentIndex, name);
                layerUIComponent.onLayerRemoved = () => RemoveLayer(currentIndex);
            }

            templateCreator.RemoveLayer(layer);

            StartCoroutine(UpdateLayout());
            ClearUndoHistory();
            ClearRedoHistory();
        }

        /// <summary>
        /// Activates the given layer 
        /// </summary>
        /// <param name="layer"> The layer to activate </param>
        public void ActivateLayer(int layer)
        {
            if (templateCreator.activeLayer < layerUIs.Count)
            {
                layerUIs[templateCreator.activeLayer].GetComponent<TemplateLayerUI>().Deactivate(); 
            }
            templateCreator.activeLayer = layer;
            DeselectObject();
            heldTile = null;
            Selection.activeObject = null;
            ClearUndoHistory();
            ClearRedoHistory();
        }

        /// <summary>
        /// Sets the layer UI to be unhidden
        /// </summary>
        /// <param name="layer"> The layer </param>
        public void UnhideLayerUI(int layer)
        {
            layerUIs[layer].GetComponent<TemplateLayerUI>().Unhide();
        }

        /// <summary>
        /// Resets the list of layer UIs
        /// </summary>
        public void ResetLayerUIs()
        {
            if (layerUIs != null)
            {
                foreach (GameObject layerUI in layerUIs)
                {
                    Destroy(layerUI);
                }
            }
            layerUIs = new List<GameObject>();
            StartCoroutine(UpdateLayout());
        }

        /// <summary>
        /// Adds a layer to the UI
        /// </summary>
        /// <param name="name"> The name of the layer UI </param>
        public void AddLayerUI(string name = "")
        {
            GameObject newLayerUI = Instantiate(layerUI);
            if (layerUIs == null)
            {
                layerUIs = new List<GameObject>();
            }

            if (layerUIs.Count == 0)
            {
                newLayerUI.GetComponent<TemplateLayerUI>().removable = false;
                name = "Pathfinding Layer";
            }
            else
            {
                newLayerUI.GetComponent<TemplateLayerUI>().Deactivate();
            }

            if (name != "")
            {
                newLayerUI.GetComponent<TemplateLayerUI>().Name(name);
            }
            layerUIs.Add(newLayerUI);
            newLayerUI.transform.SetParent(layerUIContainer.transform, false);
            StartCoroutine(UpdateLayout());
            TemplateLayerUI layerUIComponent = newLayerUI.GetComponent<TemplateLayerUI>();
            int currentCount = layerUIs.Count;
            layerUIComponent.onLayerActivated = () => ActivateLayer(currentCount - 1);
            layerUIComponent.onLayerHiddenToggled = () => templateCreator.ToggleLayerVisibility(currentCount - 1);
            layerUIComponent.onLayerNamed = (string name) => templateCreator.RenameLayer(currentCount - 1, name);
            layerUIComponent.onLayerRemoved = () => RemoveLayer(currentCount - 1);
            templateCreator.RenameLayer(currentCount - 1, name);
        }

        /// <summary>
        /// Updates the layouts after a frame, waiting for the children to be fully born
        /// </summary>
        /// <returns> Waits for a frame </returns>
        public IEnumerator UpdateLayout()
        {
            yield return null;
            layerUIContainer.CalculateLayoutInputVertical();
            layerUIContainer.SetLayoutVertical();
            layerUIContainer.transform.parent.gameObject.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            layerUIContainer.transform.parent.gameObject.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }

        #endregion

        #region Moving

        // The mouse position on the last frame
        private Vector3 lastMousePosition;

        /// <summary>
        /// Called when the pan button is pressed or released
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnPan(InputValue input)
        {
            panButtonPressed = input.isPressed;
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

        #endregion

        #region Undo and Redo

        // The undo history
        private Stack<List<UndoRedoAction>> undoHistory;

        // The redo history
        private Stack<List<UndoRedoAction>> redoHistory;

        // The current action being performed
        private List<UndoRedoAction> currentAction;

        // The container to hold undo objects
        private GameObject undoObjectsContainer;

        // The container to hold any redo objects
        private GameObject redoObjectsContainer;

        // The container to hold objects where it's unkown whether they will be undo objects or redo objects
        private GameObject tempObjectsContainer;

        /// <summary>
        /// Un-does the last action
        /// </summary>
        public void OnUndo()
        {
            if (undoHistory.Count <= 0) { return; }

            List<UndoRedoAction> lastAction = undoHistory.Peek();
            foreach (UndoRedoAction action in lastAction)
            {
                action.undoAction?.Invoke();
                foreach (GameObject relevantObject in action.relevantObjects)
                {
                    relevantObject.transform.SetParent(redoObjectsContainer.transform);
                }
            }
            redoHistory.Push(lastAction);
            undoHistory.Pop();

            foreach (UndoRedoAction action in currentAction)
            {
                foreach (GameObject relevantObject in action.relevantObjects)
                {
                    Destroy(relevantObject);
                }
            }
            currentAction.Clear();
        }

        /// <summary>
        /// Re-does the last undone action
        /// </summary>
        public void OnRedo()
        {
            if (redoHistory.Count <= 0) { return; }

            List<UndoRedoAction> lastAction = redoHistory.Peek();
            foreach (UndoRedoAction action in lastAction)
            {
                action.redoAction?.Invoke();
                foreach (GameObject relevantObject in action.relevantObjects)
                {
                    relevantObject.transform.parent = undoObjectsContainer.transform;
                }
            }
            undoHistory.Push(lastAction);
            redoHistory.Pop();

            foreach (UndoRedoAction action in currentAction)
            {
                foreach (GameObject relevantObject in action.relevantObjects)
                {
                    Destroy(relevantObject);
                }
            }
            currentAction.Clear();
        }

        /// <summary>
        /// Clears the redo history (getting rid of any stray objects)
        /// </summary>
        public void ClearRedoHistory()
        {
            Destroy(redoObjectsContainer);
            redoObjectsContainer = new GameObject();
            redoObjectsContainer.name = "Redo Objects container";
            redoObjectsContainer.SetActive(false);
            redoHistory.Clear();
        }

        /// <summary>
        /// Clears the undo history (getting rid of any stray objects)
        /// </summary>
        public void ClearUndoHistory()
        {
            Destroy(undoObjectsContainer);
            undoObjectsContainer = new GameObject();
            undoObjectsContainer.name = "Undo Objects Container";
            undoObjectsContainer.SetActive(false);
            undoHistory.Clear();
        }

        #endregion

#endif
    }
}
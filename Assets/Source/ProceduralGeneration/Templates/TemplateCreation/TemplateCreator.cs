using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

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

#if UNITY_EDITOR
        [Header("Template")]

        [Tooltip("The name to give the template")]
        [SerializeField] private string _templateName;
        public string templateName
        {
            set
            {
                _templateName = value;
                onTemplateNameChange?.Invoke(_templateName);
            }
            get => _templateName;
        }

        [Tooltip("Called when the template name is edited")]
        public UnityEvent<string> onTemplateNameChange;

        [Tooltip("The sprite to use when the preferred object is null")]
        public Sprite nullSprite;

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
                onSizeMultiplierChangedX?.Invoke(value.x.ToString());
                onSizeMultiplierChangedY?.Invoke(value.y.ToString());
                Reload();
            }
            get { return _sizeMultiplier; }
        }

        //The x axis size multiplier.
        public string sizeMultiplierX
        {
            set
            {
                _sizeMultiplier.x = int.Parse(value); 
                roomSize = _sizeMultiplier * mapCellSize;
                Reload();
            }
            get => sizeMultiplier.x.ToString();
        }

        //The y axis size multiplier.
        public string sizeMultiplierY
        {
            set
            {
                _sizeMultiplier.y = int.Parse(value);
                roomSize = _sizeMultiplier * mapCellSize;
                Reload();
            }
            get => sizeMultiplier.x.ToString();
        }

        [Tooltip("Called when the sizeMultiplierX is edited")]
        public UnityEvent<string> onSizeMultiplierChangedX;

        [Tooltip("Called when the sizeMultiplierY is edited")]
        public UnityEvent<string> onSizeMultiplierChangedY;

        [Header("Scene variables")]

        [Tooltip("The camera")]
        public TemplateCreatorCamera templateCamera;

        // The size of the room being created
        public Vector2Int roomSize { private set; get; }

        // A null sprite game object
        [HideInInspector] public GameObject nullSpriteObject;

        // The active layer
        [HideInInspector] public int activeLayer = 0;

        // Tracks whether start has been called or not
        private bool started;

        // The actual template being created
        private Template createdTemplate;

        // A container that holds the visual bounding box
        private GameObject visualsContainer;

        // A container for holding null sprites
        private GameObject nullSpritesContainer;

        // The null sprite game objects (organized by layer)
        private List<GameObject[,]> nullSprites;

        /// <summary>
        /// Initializes the template creator
        /// </summary>
        private void Start()
        {
            started = true;
            onTemplateNameChange?.Invoke(_templateName);
            roomSize = sizeMultiplier * mapCellSize;
            nullSpriteObject = new GameObject();
            nullSpriteObject.name = "Null Sprite Object";
            nullSpriteObject.transform.parent = transform;
            nullSpriteObject.AddComponent<SpriteRenderer>().sprite = nullSprite;
            nullSpriteObject.SetActive(false);
            GetComponent<TemplateCreatorInput>().Initialize();
            Reload();
        }

        #region Initialization and saving

        /// <summary>
        /// Saves the scriptable object to a file
        /// </summary>
        public void SaveTemplate()
        {
            if (templateName.Length == 0)
            {
                Debug.LogWarning("Please enter a file name");
                return;
            }

            string path = "Assets/Content/Templates/" + templateName + ".prefab";
            GetComponent<TemplateCreatorInput>().DeselectObject();

            List<GameObject> layers = createdTemplate.GetLayers();
            TemplateCreatorInput input = GetComponent<TemplateCreatorInput>();
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].SetActive(true);
                input.UnhideLayerUI(i);

                for (int j = 0; j < roomSize.x; j++)
                {
                    for (int k = 0; k < roomSize.y; k++)
                    {
                        if (createdTemplate[i, j, k] == null) { continue; }

                        foreach (MonoBehaviour component in createdTemplate[i, j, k].GetComponents<MonoBehaviour>())
                        {
                            MonoBehaviour originalComponent = PrefabUtility.GetCorrespondingObjectFromSource(component);
                            if (originalComponent == null)
                            {
                                // Enable by default (this would happen if you added a component to a tile during template creation)
                                component.enabled = true;
                            }
                            component.enabled = originalComponent.enabled;
                        }

                        if (createdTemplate[i, j, k].GetComponent<Rigidbody2D>() != null)
                        {
                            createdTemplate[i, j, k].GetComponent<Rigidbody2D>().simulated = true;
                        }
                    }
                }
            }

            PrefabUtility.SaveAsPrefabAsset(createdTemplate.gameObject, path);
            AssetDatabase.Refresh();

            Debug.Log("Template saved to " + path);

            // Redisable all the components
            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < roomSize.x; j++)
                {
                    for (int k = 0; k < roomSize.y; k++)
                    {
                        if (createdTemplate[i, j, k] == null) { continue; }

                        foreach (MonoBehaviour component in createdTemplate[i, j, k].GetComponents<MonoBehaviour>())
                        {
                            component.enabled = false;
                        }

                        if (createdTemplate[i, j, k].GetComponent<Rigidbody2D>() != null)
                        {
                            createdTemplate[i, j, k].GetComponent<Rigidbody2D>().simulated = false;
                        }

                        if (createdTemplate[i, j, k].GetComponent<SpriteRenderer>() != null)
                        {
                            createdTemplate[i, j, k].GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates everything when the room size is changed
        /// </summary>
        public void Reload()
        {
            if (createdTemplate != null)
            {
                Destroy(createdTemplate.gameObject);
            }

            TemplateCreatorInput input = GetComponent<TemplateCreatorInput>();
            input.ResetLayerUIs();

            createdTemplate = new GameObject().AddComponent<Template>();
            createdTemplate.name = "Created Template";
            createdTemplate.sizeMultiplier = sizeMultiplier;
            createdTemplate.mapCellSize = mapCellSize;
            createdTemplate.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2);

            GameObject pathfindingLayer = new GameObject();
            createdTemplate.AddLayer(pathfindingLayer);
            input.AddLayerUI();
            activeLayer = 0;

            Destroy(nullSpritesContainer);
            nullSpritesContainer = new GameObject();
            nullSpritesContainer.transform.parent = transform;
            nullSpritesContainer.name = "Null Sprites Container";
            transform.position = new Vector3(-roomSize.x / 2, -roomSize.y / 2);
            nullSpritesContainer.transform.localPosition = new Vector3(0, 0);

            nullSprites = new List<GameObject[,]>();
            nullSprites.Add(new GameObject[roomSize.x, roomSize.y]);

            CreateVisualBoundingBox();
        }

        /// <summary>
        /// Loads a template into the template creator
        /// </summary>
        /// <param name="template"> The template to load </param>
        public void LoadTemplate(Template template)
        {

            Debug.Log("Loading template " + template.name);

            Debug.Log(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(template.gameObject)));
            templateName = template.name;
            string[] splitAssetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(template.gameObject)).Split(char.Parse("/"));

            if (splitAssetPath.Length <= 3 || splitAssetPath[0] != "Assets" || splitAssetPath[1] != "Content" || splitAssetPath[2] != "Templates")
            {
                Debug.LogWarning("Templates should be saved in the Assets/Content/Templates folder! This template will be saved in the templates folder as " + template.name);
                templateName = template.name;
            }
            else
            {
                string newName = "";

                // 3 to skip Assets, Content, and Templates, - 1 so template.name can be added at the end (to skip the extra / and the .prefab)
                for (int i = 3; i < splitAssetPath.Length - 1; i++)
                {
                    newName += splitAssetPath[i] + "/";
                }
                newName += template.name;
                templateName = newName;
            }

            mapCellSize = template.mapCellSize;
            sizeMultiplier = template.sizeMultiplier;
            Destroy(createdTemplate.gameObject);
            createdTemplate = ((GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(template.gameObject))).GetComponent<Template>();
            PrefabUtility.UnpackPrefabInstance(createdTemplate.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            List<GameObject> layers = createdTemplate.GetLayers();

            TemplateCreatorInput input = GetComponent<TemplateCreatorInput>();
            input.ResetLayerUIs();
            nullSprites = new List<GameObject[,]>();
            for (int i = 0; i < layers.Count; i++)
            {
                input.AddLayerUI(layers[i].name);
                nullSprites.Add(new GameObject[roomSize.x, roomSize.y]);
                for (int j = 0; j < roomSize.x; j++)
                {
                    for (int k = 0; k < roomSize.y; k++)
                    {
                        if (createdTemplate[i, j, k] == null) { continue; }

                        if (createdTemplate[i, j, k].GetComponent<SpriteRenderer>() == null || createdTemplate[i, j, k].GetComponent<SpriteRenderer>().sprite == null)
                        {
                            GameObject createdNullSprite = Instantiate(nullSpriteObject);
                            createdNullSprite.SetActive(true);
                            nullSprites[i][j, k] = createdNullSprite;
                            createdNullSprite.transform.parent = nullSpritesContainer.transform;
                            createdNullSprite.transform.localPosition = new Vector3(j, k, 0);
                        }

                        foreach (MonoBehaviour component in createdTemplate[i, j, k].GetComponents<MonoBehaviour>())
                        {
                            component.enabled = false;
                        }

                        if (createdTemplate[i, j, k].GetComponent<Rigidbody2D>() != null)
                        {
                            createdTemplate[i, j, k].GetComponent<Rigidbody2D>().simulated = false;
                        }

                        if (createdTemplate[i, j, k].GetComponent<SpriteRenderer>() != null)
                        {
                            createdTemplate[i, j, k].GetComponent<SpriteRenderer>().enabled = true;
                        }
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
            Vector2Int edgeOffset = new Vector2Int();
            edgeOffset.x = -1 * System.Convert.ToInt32(roomSize.x % 2 == 0);
            edgeOffset.y = -1 * System.Convert.ToInt32(roomSize.y % 2 == 0);
            boundingBox.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f, -roomSize.y / 2 - 0.5f));
            boundingBox.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f, roomSize.y / 2 + 0.5f + edgeOffset.y));
            boundingBox.SetPosition(2, new Vector3(roomSize.x / 2 + 0.5f + edgeOffset.x, roomSize.y / 2 + 0.5f + edgeOffset.y));
            boundingBox.SetPosition(3, new Vector3(roomSize.x / 2 + 0.5f + edgeOffset.x, -roomSize.y / 2 - 0.5f));
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
            wallBoundingBox.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + 1, roomSize.y / 2 + 0.5f - 1 + edgeOffset.y));
            wallBoundingBox.SetPosition(2, new Vector3(roomSize.x / 2 + 0.5f - 1 + edgeOffset.x, roomSize.y / 2 + 0.5f - 1 + edgeOffset.y));
            wallBoundingBox.SetPosition(3, new Vector3(roomSize.x / 2 + 0.5f - 1 + edgeOffset.x, -roomSize.y / 2 - 0.5f + 1));
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
                gridLine.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + i, roomSize.y / 2 + 0.5f - 1 + edgeOffset.y));

                if ((i % mapCellSize.x) == mapCellSize.x / 2 || (i % mapCellSize.x) == mapCellSize.x / 2 + 1)
                {
                    GameObject doorLineContainer = new GameObject();
                    doorLineContainer.name = "Door Line";
                    doorLineContainer.transform.parent = visualsContainer.transform;

                    LineRenderer doorLine = doorLineContainer.AddComponent<LineRenderer>();
                    doorLine.material = gridLine.material;
                    doorLine.endColor = Color.white;
                    doorLine.startColor = Color.white;
                    doorLine.widthMultiplier = 0.05f;
                    doorLine.positionCount = 2;
                    doorLine.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f + i, -roomSize.y / 2 - 0.5f));
                    doorLine.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + i, -roomSize.y / 2 - 0.5f + 1));

                    doorLineContainer = new GameObject();
                    doorLineContainer.name = "Door Line";
                    doorLineContainer.transform.parent = visualsContainer.transform;

                    doorLine = doorLineContainer.AddComponent<LineRenderer>();
                    doorLine.material = gridLine.material;
                    doorLine.endColor = Color.white;
                    doorLine.startColor = Color.white;
                    doorLine.widthMultiplier = 0.05f;
                    doorLine.positionCount = 2;
                    doorLine.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f + i, roomSize.y / 2 + 0.5f + edgeOffset.y));
                    doorLine.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + i, roomSize.y / 2 + 0.5f - 1 + edgeOffset.y));
                }
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
                gridLine.SetPosition(1, new Vector3(roomSize.x / 2 + 0.5f - 1 + edgeOffset.x, -roomSize.y / 2 - 0.5f + j));

                if ((j % mapCellSize.y) == mapCellSize.y / 2 || (j % mapCellSize.y) == mapCellSize.y / 2 + 1)
                {
                    GameObject doorLineContainer = new GameObject();
                    doorLineContainer.name = "Door Line";
                    doorLineContainer.transform.parent = visualsContainer.transform;

                    LineRenderer doorLine = doorLineContainer.AddComponent<LineRenderer>();
                    doorLine.material = gridLine.material;
                    doorLine.endColor = Color.white;
                    doorLine.startColor = Color.white;
                    doorLine.widthMultiplier = 0.05f;
                    doorLine.positionCount = 2;
                    doorLine.SetPosition(0, new Vector3(-roomSize.x / 2 - 0.5f, -roomSize.y / 2 - 0.5f + j));
                    doorLine.SetPosition(1, new Vector3(-roomSize.x / 2 - 0.5f + 1, -roomSize.y / 2 - 0.5f + j));

                    doorLineContainer = new GameObject();
                    doorLineContainer.name = "Door Line";
                    doorLineContainer.transform.parent = visualsContainer.transform;

                    doorLine = doorLineContainer.AddComponent<LineRenderer>();
                    doorLine.material = gridLine.material;
                    doorLine.endColor = Color.white;
                    doorLine.startColor = Color.white;
                    doorLine.widthMultiplier = 0.05f;
                    doorLine.positionCount = 2;
                    doorLine.SetPosition(0, new Vector3(roomSize.x / 2 + 0.5f + edgeOffset.x, -roomSize.y / 2 - 0.5f + j));
                    doorLine.SetPosition(1, new Vector3(roomSize.x / 2 + 0.5f - 1 + edgeOffset.x, -roomSize.y / 2 - 0.5f + j));
                }
            }
        }

        #endregion

        #region Editing

        /// <summary>
        /// Places a tile in the template in the current layer
        /// </summary>
        /// <param name="tilePrefab"> The tile to place </param>
        /// <param name="gridPos"> The grid position to place the tile in </param>
        public bool PlaceTile(GameObject tilePrefab, Vector2Int gridPos)
        {
            if (IsGridPosOutsidePathfindingBounds(gridPos) || !(createdTemplate.GetLayer(activeLayer).activeSelf)) { return false; }

            EraseTile(gridPos);
            GameObject layer = createdTemplate.GetLayer(activeLayer);
            GameObject createdTile = ((GameObject) PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(tilePrefab), layer.transform));

            PrefabUtility.SetPropertyModifications(createdTile, PrefabUtility.GetPropertyModifications(tilePrefab));

            foreach (MonoBehaviour component in createdTile.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }

            if (createdTile.GetComponent<Rigidbody2D>())
            {
                createdTile.GetComponent<Rigidbody2D>().simulated = false;
            }

            if (createdTile.GetComponent<SpriteRenderer>() != null)
            {
                createdTile.GetComponent<SpriteRenderer>().enabled = true;
            }

            createdTile.name = tilePrefab.name;
            createdTile.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);

            // Layer 0 is pathfinding layer, set the grid location if in pathfinding layer
            if (activeLayer == 0)
            {
                createdTile.GetComponent<Tile>().gridLocation = gridPos;
                createdTemplate[gridPos.x, gridPos.y] = createdTile.GetComponent<Tile>();
            }
            else
            {
                createdTemplate[activeLayer, gridPos.x, gridPos.y] = createdTile;
            }

            if (createdTile.GetComponent<SpriteRenderer>() == null || createdTile.GetComponent<SpriteRenderer>().sprite == null)
            {
                GameObject createdNullSprite = Instantiate(nullSpriteObject);
                createdNullSprite.SetActive(true);
                nullSprites[activeLayer][gridPos.x, gridPos.y] = createdNullSprite;
                createdNullSprite.transform.parent = nullSpritesContainer.transform;
                createdNullSprite.transform.localPosition = new Vector3(gridPos.x, gridPos.y, 0);
            }

            return true;
        }

        /// <summary>
        /// Erases the tile at a given position
        /// </summary>
        /// <param name="gridPos"> The position to erase the tile at </param>
        public bool EraseTile(Vector2Int gridPos)
        {
            if (IsGridPosOutsidePathfindingBounds(gridPos) || !(createdTemplate.GetLayer(activeLayer).activeSelf))
            {
                return false;
            }

            Destroy(nullSprites[activeLayer][gridPos.x, gridPos.y]);
            nullSprites[activeLayer][gridPos.x, gridPos.y] = null;
            if (createdTemplate[activeLayer, gridPos.x, gridPos.y] != null)
            {
                Destroy(createdTemplate[activeLayer, gridPos.x, gridPos.y].gameObject);
                createdTemplate[activeLayer, gridPos.x, gridPos.y] = null;
            }

            return true;
        }

        /// <summary>
        /// Gets the pathfinding tile at a given position
        /// </summary>
        /// <param name="gridPos"> The grid position </param>
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
        /// Gets the game object at the given grid position 
        /// </summary>
        /// <param name="gridPos"> The grid position </param>
        /// <returns> The object </returns>
        public GameObject GetObject(Vector2Int gridPos)
        {
            if (IsGridPosOutsideBounds(gridPos))
            {
                return null;
            }
            return createdTemplate[activeLayer, gridPos.x, gridPos.y];
        }

        #endregion

        #region Bounds checking

        /// <summary>
        /// Determines whether a given grid pos is outside the bounds of the template
        /// </summary>
        /// <param name="gridPos"> The grid pos </param>
        /// <returns> Whether or not the grid pos is within the bounds </returns>
        public bool IsGridPosOutsideBounds(Vector2Int gridPos)
        {
            return gridPos.x < 0 || gridPos.x >= roomSize.x || gridPos.y < 0 || gridPos.y >= roomSize.y;
        }

        /// <summary>
        /// Determines whether a given grid pos is outside the pathfinding bounds of the template
        /// </summary>
        /// <param name="gridPos"> The grid pos </param>
        /// <returns> Whether or not the grid pos is within the bounds </returns>
        public bool IsGridPosOutsidePathfindingBounds(Vector2Int gridPos)
        {
            return gridPos.x < 1 || gridPos.x >= roomSize.x - 1 || gridPos.y < 1 || gridPos.y >= roomSize.y - 1;
        }

        /// <summary>
        /// Checks if the template creator is valid yet
        /// </summary>
        /// <returns> Whether or not the template creator is valid </returns>
        public bool IsValid()
        {
            return started && createdTemplate.IsValid();
        }

        #endregion

        #region Layers

        /// <summary>
        /// Adds a layer
        /// </summary>
        public void AddLayer(string layerName)
        {
            GameObject newLayer = new GameObject();
            newLayer.name = layerName;
            createdTemplate.AddLayer(newLayer);
            nullSprites.Add(new GameObject[roomSize.x, roomSize.y]);
        }

        /// <summary>
        /// Removes a layer
        /// </summary>
        /// <param name="layer"> The layer to remove </param>
        public void RemoveLayer(int layer)
        {
            createdTemplate.RemoveLayer(layer);
            for (int i = 0; i < roomSize.x; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    Destroy(nullSprites[layer][i, j]);
                }
            }
            nullSprites.RemoveAt(layer);
        }


        /// <summary>
        /// Renames a layer
        /// </summary>
        /// <param name="layer"> The layer to rename </param>
        /// <param name="name"> The name of the layer </param>
        public void RenameLayer(int layer, string name)
        {
            createdTemplate.GetLayer(layer).name = name;
        }

        /// <summary>
        /// Toggles the given layer's visibility
        /// </summary>
        /// <param name="layer"> The layer to toggle </param>
        public void ToggleLayerVisibility(int layer)
        {
            GameObject toggledLayer = createdTemplate.GetLayer(layer);
            toggledLayer.SetActive(!toggledLayer.activeSelf);
        }

        #endregion

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
#endif
    }
}
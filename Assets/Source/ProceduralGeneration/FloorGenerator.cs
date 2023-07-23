using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Skaillz.EditInline;

namespace Cardificer
{
    /// <summary>
    /// Handles generation for an entire floor: The layout and the external rooms. 
    /// The rooms handle their own internal generation, but they will use the Params on this generator (the templates).
    /// </summary>
    public class FloorGenerator : MonoBehaviour
    {
        [Header("Seed Params")]
        [Tooltip("The seed to use for generation")]
        [SerializeField] private int _seed = 0;
        static public int seed
        {
            get => instance._seed;
            private set => instance._seed = value;
        }

        [Tooltip("Whether or not to randomize the seed on start")]
        [SerializeField] private bool _randomizeSeed;
        static public bool randomizeSeed => instance._randomizeSeed;

        [Header("General Params")]

        [Tooltip("The file to save the generation settings in")]
        [SerializeField] private string _generationSettingsFileName = "GenerationSettings";
        static public string generationSettingsFileName => instance._generationSettingsFileName;

        [Tooltip("The size of a map cell on this floor")]
        [SerializeField] private Vector2Int _cellSize;
        static public Vector2Int cellSize => instance._cellSize;

        // Event called when the room is changed
        [SerializeField] static public System.Action onRoomChange;

        // Whether or not the generation should use a predefined map
        [SerializeField] private bool _usePredefinedMap;
        static public bool usePredefinedMap
        {
            set => instance._usePredefinedMap = value;
            get => instance._usePredefinedMap;
        }

        // The predefined map
        [SerializeField] private PredefinedMap _predefinedMap;
        static public PredefinedMap predefinedMap
        {
            set => instance._predefinedMap = value;
            get => instance._predefinedMap;
        }

        // The start location in the predefined map
        [SerializeField] private Vector2Int _predefinedMapStartLoc;
        static public Vector2Int predefinedMapStartLoc
        {
            set => instance._predefinedMapStartLoc = value;
            get => instance._predefinedMapStartLoc;
        }

        [Header("Specific Params")]

        [Tooltip("The floor generator params")]
        [SerializeField] private FloorParams _floorParams;
        static public FloorParams floorParams
        {
            private set => instance._floorParams = value;
            get => instance._floorParams;
        }

        // The random instance
        [HideInInspector] static public System.Random random;

        // A reference to the generated map
        [HideInInspector] static public Map map;

        // Called when the floor has been generated.
        public static System.Action onGenerated;

        // The room the player is currently in
        private Room _currentRoom;
        static public Room currentRoom
        {
            get { return instance._currentRoom; }
            set
            {
                instance._currentRoom = value;
                instance.StartCoroutine(instance.InvokeRoomChangeAfterFrame());
            }
        }

        // The instance
        private static FloorGenerator instance;

        /// <summary>
        /// Wrapper class for a list of map cells
        /// </summary>
        [System.Serializable]
        public class PredefinedMap
        {
            /// <summary>
            /// Inner wrapper class for a list of map cells
            /// </summary>
            [System.Serializable]
            public class InnerPredefinedMap
            {
                public PredefinedMapCell[] innerArray;
            }
            /// The list of map cells
            public InnerPredefinedMap[] predefinedMap;

            /// <summary>
            /// Indexer 
            /// </summary>
            /// <param name="i"> first index </param>
            /// <param name="j"> second index </param>
            /// <returns> The predefined map cell at this index </returns>
            public PredefinedMapCell this[int i, int j]
            {
                set => predefinedMap[i].innerArray[j] = value;
                get => predefinedMap[i].innerArray[j];
            }
        }

        /// <summary>
        /// Sets up the singleton
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }


        /// <summary>
        /// Generates the layout and exteriors of the room
        /// </summary>
        private void Start()
        {
            if (SaveManager.autosaveExists)
            {
                seed = SaveManager.savedFloorSeed + FloorSceneManager.currentFloor;
            }
            else if (randomizeSeed)
            {
                seed = Random.Range(0, System.Int32.MaxValue) + FloorSceneManager.currentFloor;
            }

            random = new System.Random(seed);

            floorParams = Instantiate(floorParams);
            floorParams.ParseParams();
            LayoutParams layoutParams = floorParams.layoutParams;
            TemplateParams templateParams = floorParams.templateParams;
            RoomTypesToRoomExteriorParams roomTypesToExteriorParams = floorParams.exteriorParams;

            Dictionary<RoomType, int> templateCounts = new Dictionary<RoomType, int>();
            if (layoutParams != null)
            {
                foreach (RoomTypeToLayoutParams roomType in layoutParams.roomTypesToLayoutParams.roomTypesToLayoutParams)
                {
                    templateCounts.Add(roomType.roomType, 0);
                    try
                    {
                        foreach (DifficultyToTemplates difficultyToTemplates in templateParams.templatesPool.At(roomType.roomType).difficultiesToTemplates)
                        {
                            templateCounts[roomType.roomType] += difficultyToTemplates.templates.Count;
                        }
                    }
                    catch
                    {
                        Debug.LogError("No templates associated with room type " + roomType.roomType);
                    }
                }
            }

            if (usePredefinedMap)
            {
                map = ParsePredefinedMap();
            }
            else
            {
                map = GetComponent<LayoutGenerator>().Generate(layoutParams, templateCounts);
            }
            GetComponent<RoomExteriorGenerator>().Generate(roomTypesToExteriorParams, map, cellSize);
            SaveLayoutGenerationSettings();

            // Autosave loading
            if (!SaveManager.autosaveExists) 
            {
                Room startRoom = map.startRoom.GetComponent<Room>();
                startRoom.Enter(Direction.None, callCleared: false);
                onGenerated?.Invoke();
                return; 
            }

            List<Vector2Int> visitedRooms = SaveManager.savedVisitedRooms;
            Room lastRoom = map.startRoom.GetComponent<Room>();
            foreach (Vector2Int visitedRoom in visitedRooms)
            {
                if (visitedRoom.x >= map.mapSize.x || visitedRoom.y >= map.mapSize.y || map.map[visitedRoom.x, visitedRoom.y].room == null)
                {
                    SaveManager.AutosaveCorrupted("Invalid room index");
                    return;
                }

                lastRoom.Exit();
                lastRoom = map.map[visitedRoom.x, visitedRoom.y].room.GetComponent<Room>();
                lastRoom.Generate(false);
            }
            lastRoom.Enter(callCleared: false);

            onGenerated?.Invoke();
        }

        /// <summary>
        /// Takes the predefined map and makes a normal map out of it
        /// </summary>
        /// <returns> The map created from the predefined map </returns>
        private Map ParsePredefinedMap()
        {
            GameObject roomContainer = new GameObject();
            roomContainer.transform.parent = transform;
            roomContainer.name = "Room Container";

            MapCell[,] genMap = new MapCell[predefinedMap.predefinedMap.Length, predefinedMap.predefinedMap[0].innerArray.Length];

            // Initialize the gen map
            for (int i = 0; i < predefinedMap.predefinedMap.Length; i++)
            {
                for (int j = 0; j < predefinedMap.predefinedMap[0].innerArray.Length; j++)
                {
                    genMap[i, j] = new MapCell();
                    genMap[i, j].location = new Vector2Int(i, j);
                    genMap[i, j].direction = predefinedMap[i, j].direction;
                }
            }

            // Generate the room types in the predefined map (assuming the room types are only placed in their bottom left locations
            for (int i = 0; i < predefinedMap.predefinedMap.Length; i++)
            {
                for (int j = 0; j < predefinedMap.predefinedMap[0].innerArray.Length; j++)
                {
                    if (predefinedMap[i, j].roomType != null)
                    {
                        RoomType roomType = predefinedMap[i, j].roomType;
                        GameObject room = new GameObject();
                        room.name = roomType.displayName + " Room";
                        room.transform.parent = roomContainer.transform;
                        room.AddComponent<TemplateGenerator>(); // Should probably make a prefab but whatevs
                        Room roomComponent = room.AddComponent<Room>();
                        roomComponent.roomType = roomType;
                        roomComponent.startLocation = predefinedMapStartLoc;
                        roomComponent.roomLocation = new Vector2Int(i, j);

                        for (int k = 0; k < roomComponent.roomType.sizeMultiplier.x; k++)
                        {
                            for (int l = 0; l < roomComponent.roomType.sizeMultiplier.y; l++)
                            {
                                MapCell cell = genMap[k + roomComponent.roomLocation.x, l + roomComponent.roomLocation.y];
                                cell.room = roomComponent;
                                cell.visited = true;
                            }
                        }
                    }
                }
            }

            Map map = new Map();
            map.map = genMap;
            map.mapSize = new Vector2Int(predefinedMap.predefinedMap.Length, predefinedMap.predefinedMap[0].innerArray.Length);
            map.startRoom = genMap[predefinedMapStartLoc.x, predefinedMapStartLoc.y].room;
            return map;
        }

        /// <summary>
        /// Checks if the instance is initialized yet
        /// </summary>
        /// <returns> Whether or not the instance is initialized </returns>
        static public bool IsValid()
        {
            return instance != null;
        }

        /// <summary>
        /// Transforms a map location to a world location
        /// </summary>
        /// <param name="mapLocation"> The location to transform </param>
        /// <param name="startLocation"> The start location (aka midpoint of the map) </param>
        /// <param name="cellSize"> The size of a cell in the map </param>
        /// <returns> The world location </returns>
        static public Vector2 TransformMapToWorld(Vector2Int mapLocation, Vector2Int startLocation, Vector2Int cellSize)
        {
            return new Vector2((mapLocation.x - startLocation.x) * cellSize.x, (mapLocation.y - startLocation.y) * cellSize.y);
        }

        /// <summary>
        /// Sets all the rooms to active for debugging purposes
        /// </summary>
        static public void ShowLayout(bool showUnvisited = true)
        {
            for (int i = 0; i < instance.transform.GetChild(0).childCount; i++)
            {
                for (int j = 0; j < instance.transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    GameObject room = instance.transform.GetChild(0).GetChild(i).GetChild(j).gameObject;
                    room.SetActive(showUnvisited || room.transform.GetComponentInParent<Room>().generated);
                }
            }
        }

        /// <summary>
        /// Reverses ShowLayout
        /// </summary>
        static public void HideLayout()
        {
            for (int i = 0; i < instance.transform.GetChild(0).childCount; i++)
            {
                for (int j = 0; j < instance.transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    GameObject room = instance.transform.GetChild(0).GetChild(i).GetChild(j).gameObject;
                    room.SetActive(currentRoom.gameObject == room.transform.parent.gameObject);
                }
            }
        }

        /// <summary>
        /// Saves the layout generation settings to a file for later reference
        /// </summary>
        static public void SaveLayoutGenerationSettings()
        {
            if (!Application.isEditor || floorParams.layoutParams == null) { return; }

            string fileText = "Seed: " + seed.ToString() + "\n\n";
            fileText += "Room Types and their layout\n\n";
            
            foreach(RoomTypeToLayoutParams roomTypeToLayoutParams in floorParams.layoutParams.roomTypesToLayoutParams.roomTypesToLayoutParams)
            {
                fileText += "==============================================\n";
                RoomType roomType = roomTypeToLayoutParams.roomType;
                fileText += "Room Type: " + roomType.displayName + "\n";
                fileText += "Is start room: " + roomType.startRoom.ToString() + "\n";
                fileText += "Size multiplier: " + roomType.sizeMultiplier.ToString() + "\n";
                fileText += "Is dead end: " + roomType.deadEnd.ToString() + "\n";
                if (roomType.attachedRoom == null)
                {
                    fileText += "Attached room: null" + "\n";
                }
                else
                {
                    fileText += "-------------------------------------\n";
                    fileText += "Attached room: " + roomType.attachedRoom.displayName + "\n";
                    fileText += "Attachement location: " + roomType.attachmentLocation.ToString() + "\n";
                    fileText += "Is start room: " + roomType.attachedRoom.startRoom.ToString() + "\n";
                    fileText += "Size multiplier: " + roomType.attachedRoom.sizeMultiplier.ToString() + "\n";
                    fileText += "Use random offset: " + roomType.attachedRoom.useRandomOffset.ToString() + "\n";
                    if (roomType.attachedRoom.useRandomOffset)
                    {
                        fileText += "Offset: " + roomType.attachedRoom.offset.ToString() + "\n";
                    }
                    fileText += "Use difficulty: " + roomType.attachedRoom.useDifficulty.ToString() + "\n";
                    fileText += "-------------------------------------\n";
                }
                fileText += "Use random offset: " + roomType.useRandomOffset.ToString() + "\n";
                if (roomType.useRandomOffset)
                {
                    fileText += "Offset: " + roomType.offset.ToString() + "\n";
                }
                fileText += "Use difficulty: " + roomType.useDifficulty.ToString() + "\n";
                fileText += "Emergency room: " + roomType.emergencyRoom.ToString() + "\n";
                fileText += "Number of rooms: " + roomTypeToLayoutParams.layoutParams.numRooms.ToString() + "\n";
                fileText += "Number of rooms variance: " + roomTypeToLayoutParams.layoutParams.numRoomsVariance.ToString() + "\n";
            }

            fileText += "==============================================\n";
            File.WriteAllText("logs/" + generationSettingsFileName + ".log", fileText);
        }

        /// <summary>
        /// Invokes room change after 2 frames. This allows for room change to be called *after* the new room is set to active and *after* all the tiles are enabled
        /// (including tiles created through tile type spawners).
        /// </summary>
        /// <returns> Waits 2 frames before invoking </returns>
        private IEnumerator InvokeRoomChangeAfterFrame()
        {
            yield return null; // wait two frames
            yield return null;
            onRoomChange?.Invoke();
        }
    }
}
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
    /// The rooms handle their own internal generation, but they will use the parameters on this generator (the templates).
    /// </summary>
    public class FloorGenerator : MonoBehaviour
    {
        [Tooltip("The generation parameters for the layout of this floor")]
        [SerializeField] private LayoutGenerationParameters _layoutGenerationParameters;
        static public LayoutGenerationParameters layoutGenerationParameters => instance._layoutGenerationParameters;


        [Tooltip("The template generation parameters for this floor")] [EditInline]
        [SerializeField] private TemplateGenerationParameters _templateGenerationParameters;
        static public TemplateGenerationParameters templateGenerationParameters
        {
            get => instance._templateGenerationParameters;
            private set => instance._templateGenerationParameters = value;
        }

        [Tooltip("The size of a map cell on this floor")]
        [SerializeField] private Vector2Int _cellSize;
        static public Vector2Int cellSize => instance._cellSize;

        [Tooltip("A dictionary that holds room types and their associated exterior generation parameters for this floor")]
        [SerializeField] private RoomTypesToRoomExteriorGenerationParameters _roomTypesToExteriorGenerationParameters;
        static public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters => instance._roomTypesToExteriorGenerationParameters;

        [Tooltip("The seed to use for generation")]
        [SerializeField] private int _seed = 0;
        static public int seed
        {
            get => instance._seed;
            set => instance._seed = value;
        }

        [Tooltip("Whether or not to randomize the seed on start")]
        [SerializeField] private bool _randomizeSeed;
        static public bool randomizeSeed => instance._randomizeSeed;

        [Tooltip("The file to save the generation settings in")]
        [SerializeField] private string _generationSettingsFileName = "GenerationSettings";
        static public string generationSettingsFileName => instance._generationSettingsFileName;

        // Event called when the room is changed
        [SerializeField] private UnityEvent _onRoomChange;
        static public UnityEvent onRoomChange
        {
            get => instance._onRoomChange;
            set => instance._onRoomChange = value;
        }

        // The random instance
        [HideInInspector] static public System.Random random;

        // A reference to the generated map
        [HideInInspector] static public Map map;

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
                seed = SaveManager.savedFloorSeed;
            }
            else if (randomizeSeed)
            {
                seed = Random.Range(0, System.Int32.MaxValue);
            }

            random = new System.Random(seed);

            Dictionary<RoomType, int> templateCounts = new Dictionary<RoomType, int>();
            foreach (RoomTypeToLayoutParameters roomType in layoutGenerationParameters.roomTypesToLayoutParameters.roomTypesToLayoutParameters)
            {
                templateCounts.Add(roomType.roomType, 0);
                try
                {
                    foreach (DifficultyToTemplates difficultyToTemplates in templateGenerationParameters.templatesPool.At(roomType.roomType).difficultiesToTemplates)
                    {
                        templateCounts[roomType.roomType] += difficultyToTemplates.templates.Count;
                    }
                }
                catch
                {
                    Debug.LogError("No templates associated with room type " + roomType.roomType);
                }
            }
            map = GetComponent<LayoutGenerator>().Generate(layoutGenerationParameters, templateCounts);
            GetComponent<RoomExteriorGenerator>().Generate(roomTypesToExteriorGenerationParameters, map, cellSize);
            SaveLayoutGenerationSettings();

            templateGenerationParameters = Instantiate(templateGenerationParameters);

            // Autosave loading
            if (!SaveManager.autosaveExists) 
            {
                Room startRoom = map.startRoom.GetComponent<Room>();
                startRoom.Enter(Direction.None, callCleared: false);
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
        static public void ShowLayout()
        {
            for (int i = 0; i < instance.transform.GetChild(0).childCount; i++)
            {
                for (int j = 0; j < instance.transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    instance.transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Saves the layout generation settings to a file for later reference
        /// </summary>
        static public void SaveLayoutGenerationSettings()
        {
            string fileText = "Seed: " + seed.ToString() + "\n\n";
            fileText += "Room Types and their layout parameters: \n\n";
            
            foreach(RoomTypeToLayoutParameters roomTypeToLayoutParameters in layoutGenerationParameters.roomTypesToLayoutParameters.roomTypesToLayoutParameters)
            {
                fileText += "==============================================\n";
                RoomType roomType = roomTypeToLayoutParameters.roomType;
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
                fileText += "Number of rooms: " + roomTypeToLayoutParameters.numRooms.ToString() + "\n";
                fileText += "Number of rooms variance: " + roomTypeToLayoutParameters.numRoomsVariance.ToString() + "\n";
            }

            fileText += "==============================================\n";
            Debug.Log("Saved to file " + generationSettingsFileName + ".log");
            File.WriteAllText("logs/" + generationSettingsFileName + ".log", fileText);
        }

        /// <summary>
        /// Invokes room change after 1 frame. This allows for room change to be called *after* the new room is set to active.
        /// </summary>
        /// <returns> Waits 1 frame before invoking </returns>
        private IEnumerator InvokeRoomChangeAfterFrame()
        {
            yield return null; // wait one frame
            onRoomChange?.Invoke();
        }
    }
}
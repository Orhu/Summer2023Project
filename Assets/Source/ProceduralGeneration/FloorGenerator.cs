using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// Handles generation for an entire floor: The layout and the external rooms. 
    /// The rooms handle their own internal generation, but they will use the parameters on this generator (the templates).
    /// </summary>
    public class FloorGenerator : MonoBehaviour
    {
        [Tooltip("The generation parameters for the layout of this floor")]
        public LayoutGenerationParameters layoutGenerationParameters;

        [Tooltip("The template generation parameters for this floor")]
        [SerializeField] private TemplateGenerationParameters templateGenerationParameters;

        // The copy of the template generation parameters for this floor
        [System.NonSerialized] public TemplateGenerationParameters templateGenerationParametersInstance;

        [Tooltip("The size of a map cell on this floor")]
        public Vector2Int cellSize;

        [Tooltip("A dictionary that holds room types and their associated exterior generation parameters for this floor")]
        public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters;

        [Tooltip("The seed to use for generation")]
        public int seed = 0;

        [Tooltip("Whether or not to randomize the seed on start")]
        public bool randomizeSeed;

        [Tooltip("The file to save the generation settings in")]
        public string generationSettingsFileName = "GenerationSettings";

        // Event called when the room is changed
        [HideInInspector] public UnityEvent onRoomChange;

        // The random instance
        [HideInInspector] static public System.Random random;

        // A reference to the generated map
        [HideInInspector] public Map map;

        // The room the player is currently in
        private Room _currentRoom;
        [HideInInspector]
        public Room currentRoom
        {
            get { return _currentRoom; }
            set
            {
                _currentRoom = value;
                StartCoroutine(InvokeRoomChangeAfterFrame());
            }
        }
        
        // The instance
        public static FloorGenerator floorGeneratorInstance { get; private set; }

        /// <summary>
        /// Sets up the singleton
        /// </summary>
        void Awake()
        {
            if (floorGeneratorInstance != null && floorGeneratorInstance != this)
            {
                Destroy(gameObject);
                return;
            }
            floorGeneratorInstance = this;
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

            templateGenerationParametersInstance = templateGenerationParameters.Copy();

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
        public void ShowLayout()
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                for (int j = 0; j < transform.GetChild(0).GetChild(i).childCount; j++)
                {
                    transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }
        
        /// <summary>
        /// Invokes room change after 1 frame. This allows for room change to be called *after* the new room is set to active.
        /// </summary>
        /// <returns> Waits 1 frame before invoking </returns>
        IEnumerator InvokeRoomChangeAfterFrame()
        {
            yield return null; // wait one frame
            onRoomChange?.Invoke();
        }

        /// <summary>
        /// Saves the layout generation settings to a file for later reference
        /// </summary>
        public void SaveLayoutGenerationSettings()
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
    }
}
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
        public TemplateGenerationParameters templateGenerationParameters;

        [Tooltip("The size of a map cell on this floor")]
        public Vector2Int cellSize;

        [Tooltip("A dictionary that holds room types and their associated exterior generation parameters for this floor")]
        public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters;

        [Tooltip("The seed to use for generation")]
        public int seed = 0;

        [Tooltip("Whether or not to randomize the seed on start")]
        public bool randomizeSeed;

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
                onRoomChange?.Invoke();
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

            map = GetComponent<LayoutGenerator>().Generate(layoutGenerationParameters);
            GetComponent<RoomExteriorGenerator>().Generate(roomTypesToExteriorGenerationParameters, map, cellSize);

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
        /// <param name="roomSize"> The room size </param>
        /// <returns> The world location </returns>
        static public Vector2 TransformMapToWorld(Vector2Int mapLocation, Vector2Int startLocation, Vector2Int roomSize)
        {
            return new Vector2((mapLocation.x - startLocation.x) * roomSize.x, (mapLocation.y - startLocation.y) * roomSize.y);
        }
    }
}
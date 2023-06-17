using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Allows access and modification to all of data saved to disk.
/// 
/// To use just add a new property and SaveData backing field of the type you want to save. NOTE: Type must be serializable.
/// Inside the constructor of the save data is the name of the save file, and whether or not it should persist through save clears.
/// </summary>
/// <example>
/// // The player's position as it is saved to the disk.
/// private static SaveData<Vector2> _savedPlayerPosition = new SaveData<Vector2>("PlayerData", false);
/// public static Vector2 savedPlayerPosition
/// {
///     get => _savedPlayerPosition.data;
///     set => _savedPlayerPosition.data = value;
/// }
/// </example>
public static class SaveManager
{
    // The currently saved player deck. Saving handled by autosaves. Use autosaveExists to check if data Valid.
    public static Deck.State savedPlayerDeck
    {
        get
        {
            if (!autosaveExists) { return null; }
            return autosaver.latestAutosave.deckState;
        }
    }

    // The currently saved player health. Saving handled by autosaves. Use autosaveExists to check if data Valid.
    public static int savedPlayerHealth
    {
        get
        {
            if (!autosaveExists) { return 0; }
            return autosaver.latestAutosave.playerHealth;
        }
    }

    // The currently saved floor seed. Saving handled by autosaves. Use autosaveExists to check if data Valid.
    public static int savedFloorSeed
    {
        get
        {
            if (!autosaveExists) { return 0; }
            return autosaver.latestAutosave.floorSeed;
        }
    }

    // The currently saved visited room data. X,Y = room location, Z = size of deck at the time of clearing. Saving handled by autosaves. Use autosaveExists to check if data Valid.
    public static List<Vector3Int> savedVisitedRooms
    {
        get
        {
            if (!autosaveExists) { return null; }
            return autosaver.latestAutosave.visedRooms;
        }
    }

    // The currently saved player position. Saving handled by autosaves. Use autosaveExists to check if data Valid.
    public static Vector2 savedPlayerPosition
    {
        get
        {
            if (!autosaveExists) { return Vector2.zero; }
            return autosaver.latestAutosave.playerPos;
        }
    }



    /// <summary>
    /// Class for storing any kinda of data persistently, and handling loading and storing of that data as needed.
    /// </summary>
    /// <typeparam name="T"> The type of data to store. Must be Serializable. </typeparam>
    [System.Serializable]
    private class SaveData<T>
    {
        // The currently saved data, set this value to override the old save file.
        [SerializeField] private T _data;
        public T data
        {
            get
            {
                if (!EqualityComparer<T>.Default.Equals(_data, default)) { return _data; }
                if (!File.Exists(filePath)) { return default; }

                JsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), this);
                return _data;
            }
            set
            {
                _data = value;
                File.WriteAllText(filePath, JsonUtility.ToJson(this, true));
            }
        }

        // The filename for the player deck.
        private readonly string fileName;

        // The file path this is saved at.
        private string filePath => System.IO.Path.Combine(Application.persistentDataPath, fileName);

        // The initial value this file will hold if never set.
        private T initialValue;



        /// <summary>
        /// Creates a new save data.
        /// </summary>
        /// <param name="fileName"> The filename of the save to store. </param>
        /// <param name="persistent"> Whether or not this will ignore clear data requests. </param>
        /// <param name="initialValue"> The value returned if no save file exists. </param>
        public SaveData(string fileName, bool persistent, T initialValue = default)
        {
            this.fileName = fileName;

            if (!persistent)
            {
                clearData += ClearData;
            }

            this.initialValue = initialValue;
            _data = initialValue;
        }

        /// <summary>
        /// Clears the saved data.
        /// </summary>
        private void ClearData()
        {
            _data = initialValue;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }



    #region Autosaves
    // The number of autosaves to keep.
    private const int NUMBER_OF_AUTOSAVES = 10;


    // Whether or not an autosave currently exists.
    public static bool autosaveExists
    {
        get => autosaver.latestAutosave != null;
    }

    // Stores a reference to the current autosaver
    private static Autosaver _autosaver;
    private static Autosaver autosaver
    {
        get
        {
            if (_autosaver != null) { return _autosaver; }
            _autosaver = new GameObject().AddComponent<Autosaver>();
            return _autosaver;
        }
    }

    /// <summary>
    /// Class for managing storing and loading autosaves.
    /// </summary>
    private class Autosaver : MonoBehaviour
    {
        // Stores references to all the autosaves
        private SaveData<AutosaveData>[] autosaves;

        // The currently active autosave.
        private SaveData<int> _latestAutosaveIndex = new SaveData<int>("AutosaveIndex", false);
        public AutosaveData latestAutosave
        {
            get
            {
                return autosaves[_latestAutosaveIndex.data].data;
            }
            set
            {
                _latestAutosaveIndex.data = (_latestAutosaveIndex.data + 1) % NUMBER_OF_AUTOSAVES;
                autosaves[_latestAutosaveIndex.data].data = value;
            }
        }



        /// <summary>
        /// All the data stored in a single autosave.
        /// </summary>
        [System.Serializable]
        public class AutosaveData
        {
            // The seed of the current floor.
            public int floorSeed;

            // The last position of the player.
            public Vector2 playerPos;

            // The last health of the player.
            public int playerHealth;

            // The locations and current card count of visited rooms
            public List<Vector3Int> visedRooms = new List<Vector3Int>();

            // The current state of the deck.
            public Deck.State deckState;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public AutosaveData() { }

            /// <summary>
            /// Copy constructor.
            /// </summary>
            /// <param name="other"> The instance to copy. </param>
            public AutosaveData(AutosaveData other)
            {
                floorSeed = other.floorSeed;
                playerPos = other.playerPos;
                visedRooms = other.visedRooms;
                deckState = other.deckState;
            }
        }



        /// <summary>
        /// Initializes listeners, and the save data structure.
        /// </summary>
        private void Awake()
        {
            FloorGenerator.floorGeneratorInstance.onRoomChange.AddListener(() =>
            {
                FloorGenerator.floorGeneratorInstance.currentRoom.onCleared += Autosave;
            });

            Player.Get().GetComponent<Health>().onDeath.AddListener(ClearTransientSaves);

            autosaves = new SaveData<AutosaveData>[NUMBER_OF_AUTOSAVES];
            for (int i = 0; i < NUMBER_OF_AUTOSAVES; i++)
            {
                autosaves[i] = new SaveData<AutosaveData>("Autosave_" + i, false);
            }
        }

        /// <summary>
        /// Creates an autosave if non exist.
        /// </summary>
        private void Start()
        {
            if (autosaveExists) { return; }
            Invoke("Autosave", 0.5f);
        }

        /// <summary>
        /// Saves all data needed to reload the game after a room was cleared.
        /// </summary>
        private void Autosave()
        {
            if (!gameObject.scene.isLoaded) { return; }

            AutosaveData saveData = latestAutosave == null ? new AutosaveData() : latestAutosave;
            saveData.playerPos = Player.Get().transform.position;
            saveData.playerHealth = Player.Get().GetComponent<Health>().currentHealth;
            saveData.deckState = new Deck.State(Deck.playerDeck);
            saveData.floorSeed = FloorGenerator.floorGeneratorInstance.seed;
            Vector2Int loc = FloorGenerator.floorGeneratorInstance.currentRoom.roomLocation;
            saveData.visedRooms.Add(new Vector3Int(loc.x, loc.y, Deck.playerDeck.cards.Count));

            latestAutosave = saveData;
        }
    }
    #endregion

    #region Save Clearing
    // Called when a save clear is requested.
    private static System.Action clearData;



    /// <summary>
    /// Clears all non persistent save data.
    /// </summary>
    public static void ClearTransientSaves()
    {
        clearData?.Invoke();
    }
    #endregion
}


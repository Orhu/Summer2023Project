using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    /// <summary>
    /// Allows access and modification to all of data saved to disk.
    /// 
    /// To use just add a new property and SaveData backing field of the type you want to save. NOTE: Type must be serializable.
    /// Inside the constructor of the save data is the name of the save file, and whether or not it should persist through save clears.
    /// 
    /// For more info on autosaves see the inner class Autosaver.
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
        // Whether or not the player has completed the tutorial.
        private static SaveData<bool> _savedPlayerPosition = new SaveData<bool>("TutorialCompleated", true);
        public static bool tutorialCompleted
        {
            get => _savedPlayerPosition.data;
            set => _savedPlayerPosition.data = value;
        }



        // When the autosave was last played.
        public static DateTime lastAutosaveTime
            {
                get
                {
                    if (!autosaveExists) { return DateTime.Now; }
                    return DateTime.FromFileTime(autosaver.latestAutosave.saveTime);
                }
            }

        // How long the current play session is in seconds.
        public static TimeSpan savedPlaytime
        {
            get
            {
                if (!autosaveExists) { return default; }
                return TimeSpan.FromSeconds(autosaver.latestAutosave.playtime);
            }
        }

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

        // The currently saved player money. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static int savedPlayerMoney
        {
            get
            {
                if (!autosaveExists) { return 0; }
                return autosaver.latestAutosave.playerMoney;
            }
        }
        // The currently saved player max health. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static int savedPlayerMaxHealth
        {
            get
            {
                if (!autosaveExists) { return 0; }
                return autosaver.latestAutosave.playerMaxHealth;
            }
        }

        // The currently saved player max speed. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static float savedPlayerSpeed
        {
            get
            {
                if (!autosaveExists) { return 0; }
                return autosaver.latestAutosave.playerSpeed;
            }
        }

        // The currently saved player damage multiplier. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static float savedPlayerDamage
        {
            get
            {
                if (!autosaveExists) { return 0; }
                return autosaver.latestAutosave.playerDamage;
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

        // The currently saved floor number. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static int savedCurrentFloor
        {
            get
            {
                if (!autosaveExists) { return 0; }
                return autosaver.latestAutosave.currentFloor;
            }
        }

        // The currently saved visited room data. X,Y = room location, Z = size of deck at the time of clearing. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static List<Vector2Int> savedVisitedRooms
        {
            get
            {
                if (!autosaveExists) { return null; }
                return autosaver.latestAutosave.visitedRooms;
            }
        }

        // The currently saved visited room data. X,Y = room location, Z = size of deck at the time of clearing. Saving handled by autosaves. Use autosaveExists to check if data Valid.
        public static Vector3Int[] savedRemainingShopBuys
        {
            get
            {
                if (!autosaveExists) { return null; }
                return autosaver.latestAutosave.remainingShopBuys;
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

        // The saved list of destroyed tile world positions
        public static List<Vector2> savedDestroyedTiles
        {
            get
            {
                if (!autosaveExists) { return new List<Vector2>(); }
                return autosaver.latestAutosave.destroyedTiles;
            }
        }


        // The saved list of destroyed tile world positions
        public static List<Boon.BoonToPickCountEntry> savedBoonsToPickCounts
        {
            get
            {
                if (!autosaveExists) { return new List<Boon.BoonToPickCountEntry>(); }
                return autosaver.latestAutosave.boonsToPickCounts;
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

                    // Check for corruption
                    if (File.ReadAllText(filePath) != JsonUtility.ToJson(this, true)) 
                    { 
                        throw new FileLoadException("Failed to load save data", fileName); 
                    }

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
            public void ClearData()
            {
                _data = initialValue;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            /// <summary>
            /// Whether or not this save data exists on disc.
            /// </summary>
            /// <returns> True if the file exists. </returns>
            public bool Exists()
            {
                return File.Exists(filePath);
            }

            /// <summary>
            /// When this save file was last saved to.
            /// </summary>
            /// <returns> The date and time of the last save. </returns>
            public System.DateTime LastSaveTime()
            {
                return File.GetLastWriteTimeUtc(filePath);
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

        /// <summary>
        /// Notifies the save system that an autosave has been corrupted, and attempts a revert.
        /// </summary>
        /// <param name="message"> Text describing what data is corrupted. </param>
        public static void AutosaveCorrupted(string message)
        {
            Debug.LogWarning("Autosave contains invalid data: " + message + ", reverting to previous autosave.");
            autosaver.latestAutosave = null;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        /// <summary>
        /// Class for managing and storing autosaves.
        /// </summary>
        /// <example>
        /// To use save more data during an autosave:
        /// Add a new field to the AutosaveData class, and retrieve and set any needed data inside the Autosave() function.
        /// Then create a new property in the following format:
        /// 
        /// public static Vector2 savedPlayerPosition
        /// {
        ///     get
        ///     {
        ///         if (!autosaveExists) { return Vector2.zero; }
        ///         return autosaver.latestAutosave.playerPos;
        ///     }
        /// }
        /// 
        /// This property then can be queried anywhere in the project to access to most up to date, saved data.
        /// 
        /// NOTE: Everything within the #region Save Management can be safely ignored.
        /// </example>
        private class Autosaver : MonoBehaviour
        {

            /// <summary>
            /// All the data stored in a single autosave.
            /// </summary>
            [System.Serializable]
            public class AutosaveData
            {
                // The time this was saved at.
                public long saveTime;

                // The amount of time this game has been running in seconds.
                public float playtime;

                // The seed of the current floor.
                public int floorSeed;

                // The current floor number
                public int currentFloor;

                // The random state at the time of the autosave.
                public UnityEngine.Random.State randomState;

                // The last position of the player.
                public Vector2 playerPos;

                // The last health of the player.
                public int playerHealth;

                // The last amount of money the player had.
                public int playerMoney;

                // The last max health of the player.
                public int playerMaxHealth;

                // The last max speed of the player.
                public float playerSpeed;

                // The last damage multiplier of the player.
                public float playerDamage;

                // The last cooldown multiplier of the player.
                public float playerCooldownReduction;

                // The locations and current card count of visited rooms
                public List<Vector2Int> visitedRooms = new List<Vector2Int>();

                // The locations and current card count of visited rooms
                public Vector3Int[] remainingShopBuys;

                // The current state of the deck.
                public Deck.State deckState;
                
                // The current destroyed tiles world positions
                public List<Vector2> destroyedTiles;

                // How many times each boon has been picked.
                public List<Boon.BoonToPickCountEntry> boonsToPickCounts;

                /// <summary>
                /// Default constructor.
                /// </summary>
                public AutosaveData() { }
            }

            /// <summary>
            /// Saves all data needed to reload the game after a room was cleared.
            /// </summary>
            private void Autosave()
            {
                if (!gameObject.scene.isLoaded || SceneManager.sceneCount > 1 || Player.Get() == null) { return; }

                AutosaveData saveData = latestAutosave == null ? new AutosaveData() : latestAutosave;

                // Add new save data Here:
                saveData.saveTime = DateTime.Now.ToFileTime();
                saveData.playtime = Mathf.Round(Player.Get().GetComponent<PlayerController>().playtime);
                saveData.playerPos = Player.Get().transform.position;
                saveData.playerHealth = Player.health.currentHealth;
                saveData.playerMoney = Player.GetMoney();
                saveData.playerMaxHealth = Player.health.maxHealth;
                saveData.playerSpeed = Player.Get().GetComponent<SimpleMovement>().maxSpeed;
                saveData.playerDamage = Player.Get().GetComponent<PlayerController>().damageMultiplier;
                saveData.playerCooldownReduction = Deck.playerDeck.cooldownReduction;
                saveData.deckState = new Deck.State(Deck.playerDeck);
                saveData.floorSeed = FloorGenerator.seed;
                saveData.currentFloor = FloorSceneManager.currentFloor;
                saveData.visitedRooms.Add(FloorGenerator.currentRoom.roomLocation);
                saveData.destroyedTiles = DestroyableTile.destroyedTiles?.ToList();
                saveData.remainingShopBuys = ShopSlot.savableRemainingShopBuys;
                saveData.boonsToPickCounts = Boon.BoonToPickCountEntry.GetAll();

                latestAutosave = saveData;
            }

            #region Save Management
            // Stores references to all the autosaves
            private SaveData<AutosaveData>[] autosaves;

            // Whether or not an error has been logged by this while loading.
            private bool errorLogged = false;

            // The currently active autosave.
            private SaveData<int> _latestAutosaveIndex = new SaveData<int>("AutosaveIndex", false);
            public AutosaveData latestAutosave
            {
                get
                {
                    // Find most recent autosave
                    if (!_latestAutosaveIndex.Exists())
                    {
                        bool autosaveFound = false;
                        int autosaveIndex = 0;
                        for (int i = 0; i < NUMBER_OF_AUTOSAVES; i++)
                        {
                            if (!autosaves[i].Exists() || autosaves[i].LastSaveTime() < autosaves[autosaveIndex].LastSaveTime()) { continue; }
                            autosaveIndex = i;
                            autosaveFound = true;
                        }

                        if (autosaveFound)
                        {
                            if (!errorLogged)
                            {
                                Debug.LogWarning("AutosaveIndex missing, regenerating data");
                            }
                            _latestAutosaveIndex.data = autosaveIndex;
                        }
                        else
                        {
                            // No valid autosaves
                            errorLogged = false;
                            return null;
                        }
                    }

                    // Try loading autosaves in order
                    int startingIndex = _latestAutosaveIndex.data;
                    int index = startingIndex;
                    do
                    {
                        // Check for deleted autosaves
                        if (!autosaves[index].Exists())
                        {
                            if (!errorLogged)
                            {
                                Debug.LogWarning("Autosave missing, reverting to previous autosave");
                                errorLogged = true;
                            }
                            index = (index > 0 ? index : NUMBER_OF_AUTOSAVES) - 1;
                            continue;
                        }

                        // Check for corrupted autosaves
                        try
                        {
                            _latestAutosaveIndex.data = index;
                            errorLogged = false;
                            return autosaves[index].data;
                        }
                        catch
                        {
                            if (!errorLogged)
                            {
                                Debug.LogWarning("Autosave corrupted, reverting to previous autosave");
                                errorLogged = true;
                            }
                            autosaves[index].ClearData();
                            index = (index > 0 ? index : NUMBER_OF_AUTOSAVES) - 1;
                        }
                    } while (index != startingIndex);

                    // No valid autosaves
                    Debug.LogWarning("No valid autosaves found, deleting corrupt data");
                    _latestAutosaveIndex.ClearData();
                    errorLogged = false;
                    return null;
                }
                set
                {
                    if (value == null)
                    {
                        autosaves[_latestAutosaveIndex.data].ClearData();
                        _latestAutosaveIndex.ClearData();
                        errorLogged = true;
                    }
                    else
                    {
                        _latestAutosaveIndex.data = (_latestAutosaveIndex.data + 1) % NUMBER_OF_AUTOSAVES;
                        autosaves[_latestAutosaveIndex.data].data = value;
                    }
                }
            }



            /// <summary>
            /// Initializes listeners, and the save data structure.
            /// </summary>
            private void Awake()
            {
                autosaves = new SaveData<AutosaveData>[NUMBER_OF_AUTOSAVES];
                for (int i = 0; i < NUMBER_OF_AUTOSAVES; i++)
                {
                    autosaves[i] = new SaveData<AutosaveData>("Autosave_" + i, false);
                }
            }

            /// <summary>
            /// Creates an autosave if none exist.
            /// </summary>
            private void Start()
            {
                if (!FloorGenerator.IsValid()) { return; }


                FloorGenerator.onRoomChange += () => FloorGenerator.currentRoom.onCleared += Autosave;

                // Start courotine so it's invoked on the next frame (leaving time for everything else that sets its saves up on start to start)
                if (FloorGenerator.hasGenerated)
                {
                    StartCoroutine(nameof(AutosaveAfterFrame));
                }
                else
                {
                    FloorGenerator.onGenerated += () => StartCoroutine(nameof(AutosaveAfterFrame));
                }

                Player.health.onDeath.AddListener(
                    () =>
                    {
                        ClearTransientSaves();
                        CancelInvoke();
                    });

                FloorSceneManager.onFloorLoaded += HandleFloorLoad;
            }

            /// <summary>
            /// Unbinds cleared
            /// </summary>
            private void OnDestroy()
            {
                FloorSceneManager.onFloorLoaded -= HandleFloorLoad;
            }

            /// <summary>
            /// Autosaves after a frame
            /// </summary>
            /// <returns> Waits one frame </returns>
            private System.Collections.IEnumerator AutosaveAfterFrame()
            {
                yield return new WaitForSeconds(Time.deltaTime);
                Autosave();
            }

            /// <summary>
            /// Handles the saving and loading when a floor is loaded
            /// </summary>
            private void HandleFloorLoad()
            {
                // Update everything except the current floor number (this is to ensure the floor generator understands that's it's not being loaded
                // from an autosave load, the current floor number will be updated when the autosave happens)
                AutosaveData saveData = latestAutosave;
                if (saveData != null)
                {
                    // Handle floor load autosave
                    Autosave();
                    saveData.visitedRooms.Clear();
                    saveData.playerPos = new Vector2(0, 0);
                    if (saveData.destroyedTiles != null)
                    {
                        saveData.destroyedTiles.Clear();
                    }
                    latestAutosave = saveData;
                }
            }
            #endregion
        }

        
        // Stores a reference to the current autosaver
        private static Autosaver _autosaver;
        private static Autosaver autosaver
        {
            get
            {
                if (_autosaver != null) { return _autosaver; }
                _autosaver = new GameObject("Autosaver").AddComponent<Autosaver>();
                return _autosaver;
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
}
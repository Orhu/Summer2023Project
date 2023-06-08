using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Allows access and modification to all of data saved to disk.
/// 
/// To use just add a new property and SaveData backing field of the type you want to save. NOTE: Type must be serializable.
/// Inside the constructor of the save data is the name of the save file, and whether or not it should persist through save clears.
/// </summary>
public static class SaveManager
{
    // The player's deck as it is saved to disk.
    private static SaveData<SavableList<Card>> _savedPlayerDeck = new SaveData<SavableList<Card>>("PlayerDeck", false);
    public static List<Card> savedPlayerDeck
    {
        get
        {
            if (_savedPlayerDeck.data == null) { return null; }
            return _savedPlayerDeck.data.value;
        }
        set
        {
            _savedPlayerDeck.data = new SavableList<Card>(value);
        }
    }

    // The player's position as it is saved to the disk.
    private static SaveData<Vector2> _savedPlayerPosition = new SaveData<Vector2>("PlayerData", false);
    public static Vector2 savedPlayerPosition
    {
        get => _savedPlayerPosition.data;
        set => _savedPlayerPosition.data = value;
    }

    #region Save Clearing
    // Called when a save clear is requested.
    private static System.Action ClearData;

    /// <summary>
    /// Clears all non persistent save data.
    /// </summary>
    public static void ClearTransientSaves()
    {
        ClearData?.Invoke();
    }
    #endregion

    /// <summary>
    /// Class for storing any kinda of data, and handling loading and storing of that data as needed.
    /// </summary>
    /// <typeparam name="T"> The type of data to load. Must be Serializable. </typeparam>
    private class SaveData<T> 
    {
        // The currently saved data, set this value to override the old save file.
        private T _data = default;
        public T data
        {
            get
            {
                if (!EqualityComparer<T>.Default.Equals(_data, default)) { return _data; }
                if (!File.Exists(filePath)) { return default; }

                _data = JsonUtility.FromJson<T>(File.ReadAllText(filePath));
                return _data;
            }
            set
            {
                _data = value;
                File.WriteAllText(filePath, JsonUtility.ToJson(_data, true));
            }
        }

        // The filename for the player deck.
        private readonly string fileName;

        // The file path this is saved at.
        private string filePath => System.IO.Path.Combine(Application.persistentDataPath, fileName);

        /// <summary>
        /// Creates a new save data.
        /// </summary>
        /// <param name="fileName"> The filename of the save to store. </param>
        /// <param name="persistent"> Whether or not this will ignore clear data requests. </param>
        public SaveData(string fileName, bool persistent)
        {
            this.fileName = fileName;

            if (!persistent)
            {
                SaveManager.ClearData += ClearData;
            }
        }

        /// <summary>
        /// Clears the saved data.
        /// </summary>
        private void ClearData()
        {
            _data = default;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    #region The dumb savable primitive squad
    /// <summary>
    /// A dumb class for saving a single int.
    /// </summary>
    [System.Serializable]
    private class SavableList<T>
    {
        [Tooltip("The dumb value of the dumb savable list")]
        public List<T> value;

        public SavableList(List<T> value)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// A dumb class for saving a single int.
    /// </summary>
    [System.Serializable]
    private class SavableInt
    {
        [Tooltip("The dumb value of the dumb savable int")]
        public int value;
    }

    /// <summary>
    /// A dumb class for saving a single float.
    /// </summary>
    [System.Serializable]
    private class SavableFloat
    {
        [Tooltip("The dumb value of the dumb savable float")]
        public float value;
    }

    /// <summary>
    /// A dumb class for saving single bool.
    /// </summary>
    [System.Serializable]
    private class SavableBool
    {
        [Tooltip("The dumb value of the dumb savable bool")]
        public bool value;
    }

    /// <summary>
    /// A dumb class for saving a single string.
    /// </summary>
    [System.Serializable]
    private class SavableString
    {
        [Tooltip("The dumb value of the dumb savable string")]
        public string value;
    }
    #endregion
}
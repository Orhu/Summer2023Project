using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Allows access and modification to all of data saved to disk.
/// </summary>
public static class SaveManager
{
    // The player's deck as it is saved to disk.
    private static SaveData<List<Card>> _savedPlayerDeck = new SaveData<List<Card>>("PlayerDeck");
    public static List<Card> savedPlayerDeck
    {
        get => _savedPlayerDeck.data;
        set => _savedPlayerDeck.data = value;
    }

    private static SaveData<Vector2> _savedPlayerPosition = new SaveData<Vector2>("PlayerData");
    public static Vector2 savedPlayerPosition
    {
        get => _savedPlayerPosition.data;
        set => _savedPlayerPosition.data = value;
    }


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
                File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
            }
        }

        /// <summary>
        /// Creates a new save data.
        /// </summary>
        /// <param name="fileName"> The filename of the save to store. </param>
        public SaveData(string fileName)
        {
            this.fileName = fileName;
        }

        // The filename for the player deck.
        private readonly string fileName;

        // The file path this is saved at.
        private string filePath => System.IO.Path.Combine(Application.persistentDataPath, fileName);
    }
}
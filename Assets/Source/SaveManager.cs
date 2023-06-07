using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    #region Player Deck
    // The currently saved player deck.
    private static List<Card> _savedPlayerDeck;
    public static List<Card> savedPlayerDeck
    {
        get
        {
            if (!reloadSavedPlayerDeck) { return _savedPlayerDeck; }

            string filepath = GetFilepath(PLAYER_DECK_FILENAME);

            if (!File.Exists(filepath)) { return null; }

            return JsonUtility.FromJson<List<Card>>(File.ReadAllText(filepath));
        }
    }

    // The filename for the player deck.
    private const string PLAYER_DECK_FILENAME = "PlayerDeck";

    // Whether or not the player deck needs to be reloaded
    private static bool reloadSavedPlayerDeck = true;
    #endregion

    #region Player Position
    // The currently saved player Position.
    private static Vector2 _savedPlayerPosition;
    public static Vector2 savedPlayerPosition
    {
        get
        {
            if (!reloadSavedPlayerPosition) { return _savedPlayerPosition; }

            string filepath = GetFilepath(PLAYER_POSITION_FILENAME);

            if (!File.Exists(filepath)) { return Vector2.zero; }

            return JsonUtility.FromJson<Vector2>(File.ReadAllText(filepath));
        }
    }

    // The filename for the player Position.
    private const string PLAYER_POSITION_FILENAME = "PlayerPosition";

    // Whether or not the player Position needs to be reloaded
    private static bool reloadSavedPlayerPosition = true;
    #endregion

    public static void SaveGame()
    {
        reloadSavedPlayerDeck = true;
        SaveAsJson(, PLAYER_POSITION_FILENAME);
        reloadSavedPlayerPosition = true;
        SaveAsJson(Deck.playerDeck.cards, PLAYER_DECK_FILENAME);
    }

    private static void SaveAsJson(object objectToSave, string filename)
    {
        File.WriteAllText(GetFilepath(filename), JsonUtility.ToJson(objectToSave, true));
    }

    private static string GetFilepath(string filename)
    {
        return System.IO.Path.Combine(Application.persistentDataPath, filename);
    }
}

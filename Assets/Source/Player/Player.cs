using UnityEngine;

/// <summary>
/// Static utility for getting a reference to the player game object
/// </summary>
public static class Player
{
    // Stores a ref to the player.
    private static GameObject player;

    /// <summary>
    /// Gets the player.
    /// </summary>
    /// <returns> The root game object of the player. </returns>
    public static GameObject Get()
    {
        if (player != null) { return player; }

        player = GameObject.FindGameObjectWithTag("Player");

        //player.transform.position = SaveManager.savedPlayerPosition;
        //FloorGenerator.floorGeneratorInstance.onRoomChange.AddListener(() => { SaveManager.savedPlayerPosition = player.transform.position; });

        return player;
    }

}

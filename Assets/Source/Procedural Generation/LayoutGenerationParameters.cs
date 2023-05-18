using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The parameters that affect the layout generation
/// </summary>
[System.Serializable]
public class LayoutGenerationParameters
{
    [Tooltip("The size (in rooms) of the map")]
    public Vector2Int mapSize;

    [Tooltip("The size (in tiles) of a room")]
    public Vector2Int roomSize;

    [Tooltip("The special rooms that will appear")]
    public List<GameObject> specialRooms;

    [Tooltip("The normal room")]
    public GameObject normalRoom;

    [Tooltip("The boss room")]
    public GameObject bossRoom;

    [Tooltip("The start room")]
    public GameObject startRoom;

    /// <summary>
    /// Adds a special room to the list of special rooms to spawn
    /// </summary>
    /// <param name="specialRoom"> The special room to add </param>
    void AddSpecialRoom(GameObject specialRoom)
    {
        specialRooms.Add(specialRoom);
    }

    /// <summary>
    /// Removes a special room from the list of special rooms to spawn
    /// </summary>
    /// <param name="specialRoom"> The special room to remove </param>
    void RemoveSpecialRoom(GameObject specialRoom)
    {
        specialRooms.Remove(specialRoom);
    }
}

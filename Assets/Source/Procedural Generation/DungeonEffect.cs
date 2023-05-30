using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for storing data a specific effect a card can have on a dungeon.
/// </summary>
[CreateAssetMenu(fileName = "NewDungeonEffect", menuName = "Cards/DungeonEffect", order = 1)]
public class DungeonEffect : ScriptableObject
{
    //[Tooltip("The room generation parameters that are added to the current parameters (e.g. if current numEnemies is one, and the added numEnemies is also one, the new current is 2")]
   // public RoomGenerationParameters addedRoomGenerationParameters;

    //[Tooltip("The room generation parameters that are removed from the current parameters (e.g. if current numEnemies is one, and the removed numEnemies is also one, the new current is 0")]
    //public RoomGenerationParameters removedRoomGenerationParameters;

    [Tooltip("The description of the dungeon effect")]
    public string description = "";

    public string GetFormattedDescription()
    {
        return description;
    }
}

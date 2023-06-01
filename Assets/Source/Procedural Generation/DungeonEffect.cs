using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for storing data a specific effect a card can have on a dungeon.
/// </summary>
[CreateAssetMenu(fileName = "NewDungeonEffect", menuName = "Cards/DungeonEffect", order = 1)]
public class DungeonEffect : ScriptableObject
{
    [Tooltip("The description of the dungeon effect")]
    public string description = "";

    [Tooltip("The special rooms that this card will add (to the next floor)")]
    public List<Template> specialRooms;

    [Tooltip("The tiles that this card will add")]
    public List<Tile> tiles;

    /// <summary>
    /// Returns the formatted description for display
    /// </summary>
    /// <returns> The formatted description </returns>
    public string GetFormattedDescription()
    {
        return description;
    }
}

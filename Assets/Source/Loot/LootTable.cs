using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Struct representing a loot item
/// with a weight of how likely it is to drop
/// </summary>
[System.Serializable]
public struct WeightedLootItems<T>
{
    // Item being stored in the table
    public T lootItem;
    // The RNG weight of that item
    public float weight;
}
/// <summary>
/// Generic class for storing loot and their weighted RNG
/// Create a new child script to create a new loot table of type T
/// </summary>
public class LootTable<T> : ScriptableObject
{
    
    // List of all weighted loot
    [SerializeField] List<WeightedLootItems<T>> weightedLoot;

    /// <summary>
    /// Pull a single item randomly from the table based on weight
    /// </summary>
    /// <returns>A single item from the table</returns>
    public T PullFromTable()
    {
        return weightedLoot[0].lootItem;
    }

    /// <summary>
    /// Pulls multiple items randomly from the table based on weight
    /// </summary>
    /// <param name="pullCount">Number of items pulled from the table</param>
    /// <returns>List of items pulled from the table</returns>
    public List<T> PullMultipleFromTable(int pullCount)
    {
        return new List<T>();
    }
}

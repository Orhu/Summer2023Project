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

    public WeightedLootItems<T> Clone(T lootItem, float weight)
    {
        var clone = new WeightedLootItems<T>();
        clone.lootItem = lootItem;
        clone.weight = weight;
        return clone;
    }
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
        float totalWeight = 0;
        T selected = default(T);
        if(weightedLoot.Count > 0)
        {
            foreach (var loot in weightedLoot)
            {
                float weight = loot.weight;
                float rand = Random.Range(0, totalWeight + weight);
                if (rand >= totalWeight)
                {
                    selected = loot.lootItem;
                }
                totalWeight += weight;
            }
            return selected;
        }
        else
        {
            throw new System.Exception("Table does not have any items in it.");
        } 
    }

    /// <summary>
    /// Using a copy of the weightedLootList, randomly select cards to return and delete
    /// </summary>
    /// <param name="copyOfWeightedLoot">A copy of the weighted loot list</param>
    /// <returns>A random card</returns>
    public T PullAndRemoveFromTable(List<WeightedLootItems<T>> copyOfWeightedLoot)
    {
        float totalWeight = 0;
        T selected = default(T);
        int index = 0;
        if (copyOfWeightedLoot.Count > 0)
        {
            for(int i = 0; i < copyOfWeightedLoot.Count; i++)
            {
                float weight = copyOfWeightedLoot[i].weight;
                float rand = Random.Range(0, totalWeight + weight);
                if (rand >= totalWeight)
                {
                    selected = copyOfWeightedLoot[i].lootItem;
                    index = i;
                }
                totalWeight += weight;
            }
            copyOfWeightedLoot.RemoveAt(index);
            return selected;
        }
        else
        {
            return default(T);
        }
    }
    /// <summary>
    /// Pulls multiple items randomly from the table based on weight
    /// </summary>
    /// <param name="pullCount">Number of items pulled from the table</param>
    /// <returns>List of items pulled from the table</returns>
    public List<T> PullMultipleFromTable(int pullCount)
    {
        if (pullCount <= weightedLoot.Count)
        {
            List<T> itemList = new List<T>();
            List<WeightedLootItems<T>> backup = weightedLoot.ConvertAll(weightedLootItem => weightedLootItem.Clone(weightedLootItem.lootItem, weightedLootItem.weight));
            for (int i = 0; i < pullCount; i++)
            {
                // Pull and remove an item from the list of items
                T item = PullAndRemoveFromTable(backup);
                // If it's already in the list we will be returning,
                // keep pulling and deleting from the list until we encounter a non dupe
                while (itemList.Contains(item) && backup.Count > 0)
                {
                    item = PullAndRemoveFromTable(backup);
                }

                // We find a non dupe
                if(item is { })
                {
                    itemList.Add(item);
                }
                // The list has run out
                else
                {
                    // If we have the same size list as what we were looking for,
                    // we're fine
                    if (itemList.Count == pullCount)
                    {
                        return itemList;
                    }
                    // If not, we have an imbalance
                    else
                    {
                        throw new System.Exception("Too many duplicates for pullCount");
                    }     
                }
                
            }
            return itemList;
        }
        else
        {
            throw new System.Exception("Number of items pulled exceeds table size");
        }
    }
}

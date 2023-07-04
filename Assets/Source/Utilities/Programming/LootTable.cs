using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Generic class for storing loot and their weighted RNG
    /// Create a new child script to create a new loot table of type T
    /// </summary>
    public class LootTable<T> : ScriptableObject
    {
        [Tooltip("The list of weighted loot")]
        public GenericWeightedThings<T> weightedLoot;
    }
}
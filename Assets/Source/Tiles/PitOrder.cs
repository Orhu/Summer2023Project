using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Maps ints to other ints
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewPitOrder", menuName = "Pits/PitOrder")]
    public class PitOrder : ScriptableObject
    {
        // The a list of ints that will map one int to another int (values of -1 are unused)
        public List<int> order;

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i"> The index </param>
        /// <returns> The int at that index </returns>
        public int this[int i]
        {
            set => order[i] = value;
            get => order[i];
        }
    }
}
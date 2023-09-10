using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A boon that can upgrade the player in some way.
    /// </summary>
    public abstract class Boon : ScriptableObject
    {
        // The number of times each boon has been picked up.
        private static Dictionary<System.Type, int> boonsToPickCounts;

        // The number of times this boon has been picked up.
        public int pickCount
        {
            get
            {
                if (boonsToPickCounts == null) 
                {
                    boonsToPickCounts = SaveManager.savedBoonsToPickCounts?.ToDictionary(BoonToPickCountEntry.ToKey, BoonToPickCountEntry.ToValue);
                }
                if (boonsToPickCounts != null && boonsToPickCounts.TryGetValue(GetType(), out int value)) { return value; }
                return 0;
            }
            protected set
            {
                if (boonsToPickCounts == null)
                {
                    boonsToPickCounts = SaveManager.savedBoonsToPickCounts.ToDictionary(BoonToPickCountEntry.ToKey, BoonToPickCountEntry.ToValue);

                    FloorSceneManager.onFloorLoaded += ClearPickCount;

                    void ClearPickCount()
                    {
                        FloorSceneManager.onFloorLoaded -= ClearPickCount;
                        boonsToPickCounts = null;
                    }
                }

                boonsToPickCounts[GetType()] = value;
            }
        }

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// A serializable dictionary entry
        /// </summary>
        [System.Serializable]
        public class BoonToPickCountEntry
        {
            // The boon who's pick count is being serialized.
            [SerializeField, HideInInspector] private string boonPath;

            // The pick count.
            [SerializeField, HideInInspector] private int pickCount;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="boon"> The boon who's pick count is being serialized. </param>
            /// <param name="pickCount"> The pick count. </param>
            private BoonToPickCountEntry(KeyValuePair<System.Type, int> keyValuePair)
            {
                boonPath = keyValuePair.Key.FullName;
                pickCount = keyValuePair.Value;
            }

            /// <summary>
            /// Gets the key of this.
            /// </summary>
            /// <param name="entry"> The entry who's key to get. </param>
            /// <returns> The key. </returns>
            public static System.Type ToKey(BoonToPickCountEntry entry)
            {
                return System.Type.GetType(entry.boonPath);
            }

            /// <summary>
            /// Gets the value of this.
            /// </summary>
            /// <param name="entry"> The entry who's value to get. </param>
            /// <returns> The value. </returns>
            public static int ToValue(BoonToPickCountEntry entry)
            {
                return entry.pickCount;
            }

            /// <summary>
            /// Gets all of the BoonToPickCountEntry entries.
            /// </summary>
            /// <returns> A list of all the boons mapped to their pick counts. </returns>
            public static List<BoonToPickCountEntry> GetAll()
            {
                if (boonsToPickCounts == null)
                {
                    boonsToPickCounts = SaveManager.savedBoonsToPickCounts?.ToDictionary(BoonToPickCountEntry.ToKey, BoonToPickCountEntry.ToValue);
                }
                return boonsToPickCounts?.Select((kvp) => new BoonToPickCountEntry(kvp)).ToList();
            }
        }
    }
}
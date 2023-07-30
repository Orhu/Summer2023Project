using UnityEngine;

namespace Cardificer
{
    [CreateAssetMenu(menuName = "Test")]
    public class DraftSettings : ScriptableObject
    {
        [Tooltip("The initial selection in the play's deck")]
        public Card[] playerDeck;

        [Tooltip("Cards that will be guaranteed to be added to the draft pool.")]
        public Card[] guaranteedOptions;

        [Tooltip("The loot table used to fill the draft bool.")]
        public CardLootTable draftPoolLootTable;
        
        [Tooltip("The size of the draft pool. Ignores guaranteedOptions.")] [Min(0)]
        public int draftPoolSize = 3;
        
        [Tooltip("The minimum size the player can make their deck.")] [Min(1)]
        public int minPlayerDeckSize = 8;

        [Tooltip("The maximum size the player can make their deck.")] [Min(1)]
        public int maxPlayerDeckSize = 8;

        /// <summary>
        /// Gets the current draft settings.
        /// </summary>
        /// <returns> A reference to the draft settings asset. </returns>
        public static DraftSettings Get()
        {
            return Resources.Load<DraftSettings>("DraftSettings");
        }
    }
}
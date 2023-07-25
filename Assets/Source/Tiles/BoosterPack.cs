using UnityEngine;

namespace Cardificer
{

    /// <summary>
    /// Handles collision logic for booster pack prefabs
    /// </summary>
    public class BoosterPack : MonoBehaviour
    {
        [Tooltip("Number of cards that this booster pack will produce. Between 1 and 10")]
        [Range(1, 10)]
        public int numCards;
        [Tooltip("Card Table to get probability of card drop")]
        public CardLootTable lootTable;

        /// <summary>
        /// Instantiates the loot table so all the booster packs don't share an instance
        /// </summary>
        private void Start()
        {
            lootTable = Instantiate(lootTable);
        }

        /// <summary>
        /// When the player enters the trigger zone, open booster pack menu,
        /// populate and display cards on the menu
        /// </summary>
        /// <param name="collision">Whatever is colliding with the booster pack prefab</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (lootTable != null)
                {
                    MenuManager.Open<BoosterPackMenu>().boosterPackObject = this;
                }
                else
                {
                    throw new System.Exception("Loot table not assigned to BoosterPack prefab");
                }
            }
        }
    }
}
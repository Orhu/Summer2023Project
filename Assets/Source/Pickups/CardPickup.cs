using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A pickup that will give the player a new card.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CardPickup : MonoBehaviour
    {
        [Tooltip("The loot table to draw from when choosing the card to be.")]
        [SerializeField] private CardLootTable lootTable;


        // The card this will give.
        private Card card;

        /// <summary>
        /// Gets card and sets visuals.
        /// </summary>
        private void Start()
        {
            card = lootTable.weightedLoot.GetRandomThing(transform.position);
            GetComponentInChildren<CardRenderer>(true).card = card;
            GetComponentInChildren<SpriteRenderer>().sprite = card.runeImage;
        }

        /// <summary>
        /// Give the player the card
        /// </summary>
        /// <param name="collision"> The thing that could be the player. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) { return; }

            Deck.playerDeck.AddCard(card, Deck.AddCardLocation.TopOfDrawPile);
            Destroy(gameObject);
        }
    }
}

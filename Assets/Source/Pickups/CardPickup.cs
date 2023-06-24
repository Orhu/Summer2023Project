using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    // A pickup that will give a player a card.
    [RequireComponent(typeof(Collider2D))]
    public class CardPickup : MonoBehaviour
    {
        [Tooltip("The loot table to draw from when choosing the card to be.")]
        [SerializeField] private CardLootTable lootTable;


        // The card this will give.
        private Card card;

        // Start is called before the first frame update
        void Start()
        {
            card = lootTable.PullFromTable(transform.position);
            GetComponentInChildren<CardRenderer>(true).card = card;
            GetComponentInChildren<SpriteRenderer>().sprite = card.runeImage;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) { return; }

            Deck.playerDeck.AddCard(card, Deck.AddCardLocation.TopOfDrawPile);
            Destroy(gameObject);
        }
    }
}

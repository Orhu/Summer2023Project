using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    /// <summary>
    /// Handles creation and maintenance of Booster Pack UI
    /// </summary>
    public class BoosterPackMenu : MonoBehaviour
    {
        // Card gets populated by card buttons (selected in UI)
        public Card selectedCard { get; set; }

        [Tooltip("Link to the card layout area game object")]
        public List<CardRenderer> cardRenderers = new List<CardRenderer>();

        public CardLootTable lootTable;

        /// <summary>
        /// Optionally populate the cards on start
        /// </summary>
        private void Start()
        {
            PopulateBoosterPackCards(3, lootTable);
        }

        /// <summary>
        /// Set card containers to be active and give them cards randomly from packCards
        /// </summary>
        /// <param name="numCards">Number of cards to spawn in pack</param>
        public void PopulateBoosterPackCards(int numCards, LootTable<Card> table)
        {
            List<Card> packCards = lootTable.PullMultipleFromTable(numCards);
            // Loop through all cardRenderers
            for (int i = 0; i < cardRenderers.Count; i++)
            {
                // If the current cardRenderer falls within
                // the amount of cards we want to spawn:
                if (i < numCards)
                {
                    // Choose a card at random from packCards
                    Card tempCard = packCards[Random.Range(0, packCards.Count-1)];
                    if (tempCard != null)
                    {
                        // Set the cardRenderer to active
                        cardRenderers[i].gameObject.SetActive(true);
                        // Assign it the random card
                        cardRenderers[i].card = tempCard;
                    }
                }
                else
                {
                    // If the cardRenderer is not within the number of spawned cards,
                    // set it to inactive
                    cardRenderers[i].gameObject.SetActive(false);
                }
                
            }
        }

        /// <summary>
        /// Setter used to set our selected card
        /// </summary>
        /// <param name="theCard">Card used for setting</param>
        public void SelectCard(CardRenderer cardRenderer)
        {
            selectedCard = cardRenderer.card;
        }

        /// <summary>
        /// Called after confirming the card selected. Add's card to deck
        /// </summary>
        public void AddCard()
        {
            Deck.playerDeck.AddCard(selectedCard, Deck.AddCardLocation.TopOfDrawPile);
        }
    }
}


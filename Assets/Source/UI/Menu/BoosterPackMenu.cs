using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    public class BoosterPackMenu : MonoBehaviour
    {
        // Card gets populated by card buttons (selected in UI)
        public Card selectedCard { get; set; }

        [Tooltip("Cards to be displayed in UI - will be replaced with a call to a loot table")]
        public List<Card> packCards = new List<Card> ();

        [Tooltip("Link to the card layout area game object")]
        public GameObject cardLayoutArea;

        private void Start()
        {
            PopulateBoosterPackCards(3);
        }

        /// <summary>
        /// Initally populate card UI's for player to see choices
        /// </summary>
        public void PopulateBoosterPackCards(int numCards)
        {
            for (int i = 0; i < cardLayoutArea.transform.childCount; i++)
            {
                GameObject tempUICardContainer = cardLayoutArea.transform.GetChild(i).gameObject;
                if (i < numCards)
                {
                    Card tempCard = packCards[Random.Range(0, packCards.Count)];
                    if (tempCard != null)
                    {
                        tempUICardContainer.SetActive(true);
                        tempUICardContainer.GetComponent<CardRenderer>().Card = tempCard;
                    }
                }
                else
                {
                    tempUICardContainer.SetActive(false);
                }
                
            }
        }

        /// <summary>
        /// Setter used to set our selected card
        /// </summary>
        /// <param name="theCard">Card used for setting</param>
        public void SelectCard(CardRenderer cardRenderer)
        {
            selectedCard = cardRenderer.Card;
        }

        /// <summary>
        /// Called after confirming the card selected.
        /// </summary>
        public void AddCard()
        {
            Deck.playerDeck.AddCard(selectedCard, Deck.AddCardLocation.TopOfDrawPile);
        }
    }
}


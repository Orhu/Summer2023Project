using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Class for managing the starter decks and initializing the player's deck.
    /// </summary>
    public class StarterDeckManager : MonoBehaviour
    {
        // Singleton for the starter deck manager
        private static StarterDeckManager instance;

        [Tooltip("List of all starter decks")]
        [SerializeField] public List<StarterDeck> deckList;

        // Selected deck number
        private int deckNumber = 0;

        // Indicates whether or not to allow selected starter deck to be delivered on request
        private bool deckDelivered = false; 
            // Note: once the deck is delivered to the player, this will be automatically set to true, 
            // preventing it from being delivered again.

        /// <summary>
        /// Assign singleton variable, sets deckDelivered to false if it's instance
        /// </summary>
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Fills the player's deck with the selected starter deck if this is the first time being asked.
        /// </summary>
        public static void FillDeck() {
            if (!instance.deckDelivered) 
            { 
                foreach(Card card in instance.deckList[instance.deckNumber].cards)
                {
                    Deck.playerDeck.AddCard(card, Deck.AddCardLocation.BottomOfDrawPile);
                } 
            }
        }

        /// <summary>
        /// Changes deckNumber (should only be used for deck selection menu).
        /// </summary>
        /// <param name="newSelection"> The new value for deckNum </param>
        /// <returns> The starter deck at index deckNumber in deckList. </returns>
        public static void SetSelectedDeck(int newSelection) {
            instance.deckNumber = newSelection;
        }
    }
}

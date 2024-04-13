using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Script to go on the confirm screen UI in the card printer subscreens
    /// handles logic and rendering.
    /// </summary>
    public class ConfirmScreenScript : MonoBehaviour
    {
        [Tooltip("Renderer for the selected card we are confirming")]
        [SerializeField] private CardRenderer selectedCardRenderer;

        [Tooltip("Confirm button that will have the price of copying or shredding card.")]
        [SerializeField] private Button continueButton;

        // Reference to the CardPrinterMenu script for screen traversal 
        private CardPrinterMenu cardPrinterMenu;

        /// <summary>
        /// Assign variables
        /// </summary>
        private void Awake()
        {
            cardPrinterMenu = gameObject.GetComponentInParent<CardPrinterMenu>();
        }

        /// <summary>
        /// Set the card renderer of the Copier Confirm Screen. Shows the copy price
        /// </summary>
        /// <param name="card">Card to be displayed in the card renderer</param>
        public void SetCopyCardRendererCard(Card card)
        {
            selectedCardRenderer.card = card;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (-" + card.copyPrice + "g)";
        }

        /// <summary>
        /// Set the card renderer of the Shredder Confirm Screen. Shows the shred price
        /// </summary>
        /// <param name="card">Card to be displayed in the card renderer</param>
        public void SetShredCardRendererCard(Card card)
        {
            selectedCardRenderer.card = card;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (-" + card.shredPrice + "g)";
        }

        /// <summary>
        /// Called to process the transaction of copying. Will error out if
        /// insufficient funds, else a card is added and money is removed.
        /// </summary>
        public void ProcessCopyTransaction()
        {
            if (Player.GetMoney() - selectedCardRenderer.card.copyPrice < 0)
            {
                cardPrinterMenu.MoveToErrorScreen("ERROR: INSUFFICIENT FUNDS. ABORTING.");
            }
            else
            {
                // Add selected card to the deck
                Deck.playerDeck.AddCard(selectedCardRenderer.card, Deck.AddCardLocation.TopOfDrawPile);
                Player.AddMoney(-selectedCardRenderer.card.copyPrice);
                cardPrinterMenu.MoveToErrorScreen("Success!");
            }
        }

        /// <summary>
        /// Called to process the transaction of copying. Will error out if
        /// insufficient funds, else a card is added and money is removed.
        /// </summary>
        public void ProcessShredTransaction()
        {
            if (Player.GetMoney() - selectedCardRenderer.card.copyPrice < 0)
            {
                cardPrinterMenu.MoveToErrorScreen("ERROR: INSUFFICIENT FUNDS. ABORTING.");
            }
            else
            {
                // Remove selected card from the deck
                DraftSettings deckLimitSettings = DraftSettings.Get();
                if (deckLimitSettings.minPlayerDeckSize < Deck.playerDeck.cards.Count)
                {
                    Deck.playerDeck.RemoveCard(selectedCardRenderer.card);
                    Player.AddMoney(-selectedCardRenderer.card.shredPrice);
                    cardPrinterMenu.MoveToErrorScreen("Success!");
                }
                else
                {
                    cardPrinterMenu.MoveToErrorScreen("" +
                    "WARNING: ATTEMPTED DECK SIZE VIOLATION DETECTED. ABORTING. \n\n " +
                    "Shredding this card will cause user's deck to fall below minimum deck size requirement. Please try again later.");
                }
            }
        }
    }
}


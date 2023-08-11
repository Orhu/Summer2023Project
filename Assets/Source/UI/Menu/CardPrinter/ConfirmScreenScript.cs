using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer
{
    public class ConfirmScreenScript : MonoBehaviour
    {
        [Tooltip("Renderer for the selected card we are confirming")]
        [SerializeField] private CardRenderer selectedCardRenderer;

        [Tooltip("Confirm button that will have the price of copying or shredding card.")]
        [SerializeField] private Button continueButton;

        private CardPrinterMenu cardPrinterMenu;


        private void Awake()
        {
            cardPrinterMenu = gameObject.GetComponentInParent<CardPrinterMenu>();
        }

        public void SetCopyCardRendererCard(Card card)
        {
            selectedCardRenderer.card = card;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (-" + card.copyPrice + "g)";
        }
        public void SetShredCardRendererCard(Card card)
        {
            selectedCardRenderer.card = card;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (-" + card.shredPrice + "g)";
        }

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
            }
        }

        public void ProcessShredTransaction()
        {
            if (Player.GetMoney() - selectedCardRenderer.card.copyPrice < 0)
            {
                cardPrinterMenu.MoveToErrorScreen("" +
                    "WARNING: ATTEMPTED DECK SIZE VIOLATION DETECTED. ABORTING. \n\n " +
                    "Shredding this card will cause user's deck to fall below minimum deck size requirement. Please try again later.");
            }
            //else if (Deck.playerDeck.cards.Count - 1 < Deck.playerDeck.)
            //{
            //    print("At")
            //}
            else
            {
                // Add selected card to the deck
                Deck.playerDeck.RemoveCard(selectedCardRenderer.card);
                Player.AddMoney(-selectedCardRenderer.card.shredPrice);
                cardPrinterMenu.MoveToErrorScreen("Success!");
            }
        }
    }
}


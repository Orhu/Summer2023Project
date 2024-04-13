using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Script to handle the logic and rendering of the select
    /// screen for both the copier and the shredder in the card printer UI object.
    /// </summary>
    public class SelectScreenScript : MonoBehaviour
    {
        [Tooltip("Reference to the card renderer container")]
        [SerializeField] private GridLayoutGroup cardContainer;

        [Tooltip("Up arrow for the card container")]
        [SerializeField] private Button upArrow;

        [Tooltip("Down arrow for the card container")]
        [SerializeField] private Button downArrow;

        // The low end index of the player's deck (starts at 0)
        private int lowIndex = 0;
        // The high end index of the player's deck (the 6th card of the deck)
        private int highIndex = 6;

        [Tooltip("The prefab used to draw cards")]
        [SerializeField] private GameObject cardRendererPrefab;

        private CardPrinterMenu cardPrinterMenu;

        [Tooltip("Whether or not this is the copier screen")]
        public bool isCopierScreen;

        // To keep track of what card we are either shredding or copying
        public Card selectedCard;

        /// <summary>
        /// Assign variables
        /// </summary>
        private void Awake()
        {
            cardPrinterMenu = gameObject.GetComponentInParent<CardPrinterMenu>();
        }

        /// <summary>
        /// Reset to defaults when we reenable
        /// </summary>
        private void OnEnable()
        {
            lowIndex = 0;
            highIndex = 6;
            PopulateCardList();
            EnableAndDisableArrows();
        }

        /// <summary>
        /// Populates the screen with cards that can
        /// be either shredded or copied. Clicking on one
        /// takes the user to the confirmation screen
        /// </summary>
        private void PopulateCardList()
        {

            // Delete all the children from cardlist layout
            ResetCardList();

            // Populate 6 card slots
            for (int i = lowIndex; i < highIndex; i++)
            {
                GameObject tempCardRendererGameObject = Instantiate(cardRendererPrefab, cardContainer.transform);

                // Assign the game object a card.
                tempCardRendererGameObject.GetComponent<CardRenderer>().card = Deck.playerDeck.cards[i];

                // Give the cardRenderer Game Object's toggle some listeners
                tempCardRendererGameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                {
                    // Sets the selected card to the cardRenderer
                    SelectCard(tempCardRendererGameObject.GetComponent<CardRenderer>());
                    cardPrinterMenu.SelectScreenToConfirmScreen(tempCardRendererGameObject.GetComponent<CardRenderer>().card, isCopierScreen);
                });

                if (isCopierScreen)
                {
                    tempCardRendererGameObject.GetComponent<CardRenderer>().links.priceText.text = tempCardRendererGameObject.GetComponent<CardRenderer>().card.copyPrice+"g";
                }
                else
                {
                    tempCardRendererGameObject.GetComponent<CardRenderer>().links.priceText.text = tempCardRendererGameObject.GetComponent<CardRenderer>().card.shredPrice+"g";
                }

            }
        }

        /// <summary>
        /// Destroy all of the cards in the card list
        /// </summary>
        private void ResetCardList()
        {
            // Delete all the children from cardlist layout
            foreach (Transform child in cardContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Setter used to set our selected card
        /// </summary>
        /// <param name="theCard">Card used for setting</param>
        public void SelectCard(CardRenderer cardRenderer)
        {
            selectedCard = cardRenderer.card;
            //continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue (" + cardRenderer.card.copyPrice + "g)";

            // Delete all the children from cardlist layout
            foreach (Transform child in cardContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Scrolls the card list down one row
        /// </summary>
        public void ScrollDownOneRow()
        {
            lowIndex = Mathf.Min(lowIndex + 6, Deck.playerDeck.cards.Count - 6);
            highIndex = Mathf.Min(highIndex + 6, Deck.playerDeck.cards.Count);

            PopulateCardList();
            EnableAndDisableArrows();
        }

        /// <summary>
        /// Scrolls the card list up one row
        /// </summary>
        public void ScrollUpOneRow()
        {
            lowIndex = Mathf.Max(lowIndex - 6, 0);
            highIndex = Mathf.Max(highIndex - 6, 6);

            PopulateCardList();
            EnableAndDisableArrows();
        }

        /// <summary>
        /// Enables or disables arrows depending on the low and high indexes.
        /// </summary>
        private void EnableAndDisableArrows()
        {
            if(lowIndex <= 0)
            {
                upArrow.gameObject.SetActive(false);
            }
            else
            {
                upArrow.gameObject.SetActive(true);
            }

            if (highIndex >= Deck.playerDeck.cards.Count)
            {
                downArrow.gameObject.SetActive(false);
            }
            else
            {
                downArrow.gameObject.SetActive(true);
            }

        }


        public void MoveToMainScreen()
        {
            StartCoroutine(cardPrinterMenu.ReturnBackToMainScreen(0f));
        }
    }
}
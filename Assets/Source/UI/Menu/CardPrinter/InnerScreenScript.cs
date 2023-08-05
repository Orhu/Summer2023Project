using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    public class InnerScreenScript : MonoBehaviour
    {

        public GameObject SelectCardScreen;
        public GameObject ConfirmScreen;

        // The low end index of the player's deck (starts at 0)
        private int lowIndex = 0;
        // The high end index of the player's deck (the 6th card of the deck)
        private int highIndex = 6;

        public GridLayoutGroup cardListLayout;

        [Tooltip("The prefab used to draw cards")]
        [SerializeField] private GameObject cardRendererPrefab;


        private void OnEnable()
        {
            PopulateCardList();
        }

        private void PopulateCardList()
        {

            // Delete all the children from cardlist layout
            foreach (Transform child in cardListLayout.transform)
            {
                Destroy(child.gameObject);
            }

            // Populate 6 card slots
            for (int i = lowIndex; i < highIndex; i++)
            {
                GameObject tempCardRendererGameObject = Instantiate(cardRendererPrefab, cardListLayout.transform);

                // Assign the game object a card.
                tempCardRendererGameObject.GetComponent<CardRenderer>().card = Deck.playerDeck.cards[i];

                // Give the cardRenderer Game Object's toggle some listeners
                tempCardRendererGameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                {
                    // Sets the selected card to the cardRenderer
                    SelectCard(tempCardRendererGameObject.GetComponent<CardRenderer>());
                });

            }
        }


        /// <summary>
        /// Setter used to set our selected card
        /// </summary>
        /// <param name="theCard">Card used for setting</param>
        public void SelectCard(CardRenderer cardRenderer)
        {
            SelectCardScreen.SetActive(false);

            ConfirmScreen.GetComponentInChildren<CardRenderer>().card = cardRenderer.card;
            ConfirmScreen.SetActive(true);
        }

        public void CancelTransaction()
        {
            
            ConfirmScreen.SetActive(false);
            ConfirmScreen.GetComponentInChildren<CardRenderer>().card = null;

            SelectCardScreen.SetActive(true);
        }

        public void ScrollDownOneRow()
        {
            lowIndex = Mathf.Min(lowIndex + 6, Deck.playerDeck.cards.Count - 6);
            highIndex = Mathf.Min(highIndex + 6, Deck.playerDeck.cards.Count);

            PopulateCardList();
        }

        public void ScrollUpOneRow()
        {
            lowIndex = Mathf.Max(lowIndex - 6, 0);
            highIndex = Mathf.Max(highIndex - 6, 6);

            PopulateCardList();
        }
    }
}
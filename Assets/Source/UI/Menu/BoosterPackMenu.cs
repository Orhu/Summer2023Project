using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Handles creation and maintenance of Booster Pack UI
    /// </summary>
    public class BoosterPackMenu : Menu
    {
        // Card gets populated by card buttons (selected in UI)
        public Card selectedCard { get; set; }

        [Tooltip("List of cardRender UI elements for displaying booster pack cards")]
        public List<CardRenderer> cardRenderers = new List<CardRenderer>();

        // UI area for adding cardRenderer UI objects
        [SerializeField] private GameObject cardLayoutArea;

        // The card renderer prefab to instantiate.
        public CardRenderer cardRendererTemplate;

        // Local reference to the game world booster pack object that the player collides with
        private BoosterPack _boosterPackObject;
        public BoosterPack boosterPackObject
        {
            set
            {
                _boosterPackObject = value;
                PopulateBoosterPackCards(boosterPackObject.numCards, boosterPackObject.lootTable);
            }
            get { return _boosterPackObject; }
        }
        // Reference to card confirmation window
        public GameObject confirmationWindow;

        /// <summary>
        /// Set card containers to be active and give them cards randomly from packCards
        /// </summary>
        /// <param name="numCards">Number of cards to spawn in pack</param>
        private void PopulateBoosterPackCards(int numCards, LootTable<Card> table)
        {
            if (table != null)
            {
                // List of cards to be displayed in UI     
                List<Card> packCards = table.weightedLoot.GetRandomThings(numCards, boosterPackObject.transform.position);

                // Loop through total number of spawned cards
                for (int i = 0; i < numCards; i++)
                {
                    // Add more cardRenderers
                    if (cardRenderers.Count < numCards)
                    {
                        // Instantiate the cardRenderer Game Object in the cardLayout area
                        GameObject tempCardRendererGameObject = Instantiate(cardRendererTemplate.gameObject, cardLayoutArea.transform);

                        // Set the CardRenderer's scaling factor for its scaling animation
                        tempCardRendererGameObject.GetComponent<CardRenderer>().scaleFactor = 1.5f;

                        // Set the CardRenderer's scaling duration for its scaling animation
                        tempCardRendererGameObject.GetComponent<CardRenderer>().scaleDuration = 0.25f;
                        // Give the cardRenderer Game Object's toggle some listeners
                        tempCardRendererGameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                        {
                            // Sets the selected card to the cardRenderer
                            SelectCard(tempCardRendererGameObject.GetComponent<CardRenderer>());
                            // Turns on or off the confirmation button
                            confirmationWindow.SetActive(tempCardRendererGameObject.GetComponent<Toggle>().isOn);
                        });
                        // Allows for toggling in groups of card renderers
                        tempCardRendererGameObject.GetComponent<Toggle>().group = cardLayoutArea.GetComponent<ToggleGroup>();
                        // Adds the cardRenderer to the list of cardRenderers
                        cardRenderers.Add(tempCardRendererGameObject.GetComponent<CardRenderer>());
                    }

                    // Pick one of the pack cards
                    Card tempCard = packCards[i];
                    if (tempCard != null)
                    {
                        // Set the cardRenderer to active
                        cardRenderers[i].gameObject.SetActive(true);
                        // Assign it the random card
                        cardRenderers[i].card = tempCard;
                    }

                    initialSelection = cardLayoutArea.transform.GetChild(0).gameObject;
                }
            }
            else
            {
                throw new System.Exception("No loot table to populate Booster Pack UI");
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
            // Add selected card to the deck
            Deck.playerDeck.AddCard(selectedCard, Deck.AddCardLocation.TopOfDrawPile);
            // Destroy the game world booster pack object
            Destroy(_boosterPackObject.gameObject);
            // Remove the reference to the game world booster pack object
            _boosterPackObject = null;

            MenuManager.Close<BoosterPackMenu>();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cardificer
{
    public class DraftMenu : MonoBehaviour
    {
        [Tooltip("The area to add draftable cards to.")]
        [SerializeField] private GameObject draftContainer;

        [Tooltip("The area to add the chosen cards to.")]
        [SerializeField] private GameObject deckContainer;

        [Tooltip("The prefab to use when rendering cards.")]
        [SerializeField] private CardRenderer cardRendererPrefab;

        [Tooltip("Called when the deck has been validated.")]
        public UnityEvent deckValidated;

        [Tooltip("Called when the deck has been invalidated.")]
        public UnityEvent deckInvalidated_SizeTooSmall;

        [Tooltip("Called when the deck has been invalidated.")]
        public UnityEvent deckInvalidated_SizeTooLarge;



        // The settings that determine how this acts.
        private DraftSettings settings;

        /// <summary>
        /// Adds the drafted cards to the player deck.
        /// </summary>
        public void ConfirmDeck()
        {
            foreach (CardRenderer renderer in deckContainer.GetComponentsInChildren<CardRenderer>())
            {
                Deck.playerDeck.AddCard(renderer.card, Deck.AddCardLocation.BottomOfDrawPile);
            }

            MenuManager.Close<DraftMenu>(true);
        }

        /// <summary>
        /// Initializes card renders and handles their movement when clicked.
        /// </summary>
        private void OnEnable()
        {
            settings = DraftSettings.Get();
            settings = Instantiate(settings);

            for (int i = 0; i < draftContainer.transform.childCount; i++)
            {
                Destroy(draftContainer.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < deckContainer.transform.childCount; i++)
            {
                Destroy(deckContainer.transform.GetChild(i).gameObject);
            }

            // Get random draft pool from draft loot table.

            System.Random random = new System.Random();
            IEnumerable<Card> draftpool = settings.draftPoolLootTable.weightedLoot.GetRandomThings(settings.draftPoolSize, random);
            // Add guaranteed items.
            draftpool = draftpool.Concat(settings.guaranteedOptions);

            if (settings.randomInitialDeckSize > 0)
            {
                Card[] initialCards = new Card[settings.initialDeck.Length + settings.randomInitialDeckSize];

                for (int i = 0; i < settings.initialDeck.Length; i++)
                {
                    initialCards[i] = settings.initialDeck[i];
                }

                for (int i = 0; i < settings.randomInitialDeckSize; i++)
                {
                    initialCards[i + settings.initialDeck.Length] = settings.draftPoolLootTable.weightedLoot.GetRandomThing(random);
                }

                settings.initialDeck = initialCards;
            }

            // Initializes the draft pool
            foreach (Card card in draftpool)
            {
                CardRenderer renderer = Instantiate(cardRendererPrefab.gameObject).GetComponent<CardRenderer>();
                renderer.transform.SetParent(draftContainer.transform, false);
                renderer.card = card;

                Toggle rendererToggle = renderer.GetComponent<Toggle>();
                rendererToggle.onValueChanged.AddListener(AddToDeck);

                // Moves the renderer if it is selected.
                void AddToDeck(bool shouldAdd)
                {
                    if (shouldAdd)
                    {
                        renderer.transform.SetParent(deckContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDeck);
                        rendererToggle.onValueChanged.AddListener(AddToDraft);
                        rendererToggle.isOn = false;
                        CheckDeckValidity();
                    }
                }
                void AddToDraft(bool shouldAdd)
                {
                    if (shouldAdd)
                    {
                        renderer.transform.SetParent(draftContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDraft);
                        rendererToggle.onValueChanged.AddListener(AddToDeck);
                        rendererToggle.isOn = false;
                        CheckDeckValidity();
                    }
                }
            }

            // Initializes the default deck
            foreach (Card card in settings.initialDeck)
            {
                CardRenderer renderer = Instantiate(cardRendererPrefab.gameObject).GetComponent<CardRenderer>();
                renderer.transform.SetParent(deckContainer.transform, false);
                renderer.card = card;

                Toggle rendererToggle = renderer.GetComponent<Toggle>();
                rendererToggle.onValueChanged.AddListener(AddToDraft);

                // Moves the renderer if it is selected.
                void AddToDeck(bool shouldAdd)
                {
                    if (shouldAdd)
                    {
                        renderer.transform.SetParent(deckContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDeck);
                        rendererToggle.onValueChanged.AddListener(AddToDraft);
                        rendererToggle.isOn = false;
                        CheckDeckValidity();
                    }
                }
                void AddToDraft(bool shouldAdd)
                {
                    if (shouldAdd && settings.allowRemovalFromInitialDeck)
                    {
                        renderer.transform.SetParent(draftContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDraft);
                        rendererToggle.onValueChanged.AddListener(AddToDeck);
                        CheckDeckValidity();
                    }
                    rendererToggle.isOn = false;
                }
            }

            CheckDeckValidity();
        }

        /// <summary>
        /// Calls the appropriate validity event for the current deck.
        /// </summary>
        private void CheckDeckValidity()
        {
            int deckSize = deckContainer.transform.childCount;
            if (settings.maxPlayerDeckSize < deckSize)
            {
                deckInvalidated_SizeTooLarge?.Invoke();
            }
            else if (settings.minPlayerDeckSize > deckSize)
            {
                deckInvalidated_SizeTooSmall?.Invoke();
            }
            else
            {
                deckValidated?.Invoke();
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cardificer
{
    public class DraftMenu : Menu
    {
        [Tooltip("The area to add draftable cards to.")]
        [SerializeField] private ScrollRect draftScrollRect;

        [Tooltip("The area to add the chosen cards to.")]
        [SerializeField] private ScrollRect deckScrollRect;

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

        // The current size of the drafted deck.
        private int deckSize = 0;

        // The container to add the draft cards to.
        private GameObject draftContainer => draftScrollRect.viewport.transform.GetChild(0).gameObject;

        // The container to add the deck cards to.
        private GameObject deckContainer => deckScrollRect.viewport.transform.GetChild(0).gameObject;

        /// <summary>
        /// Adds the drafted cards to the player deck.
        /// </summary>
        public void ConfirmDeck()
        {
            if (!CheckDeckValidity()) { return; }

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
            deckSize = 0;
            initialSelection = null;
            settings = DraftSettings.Get();

            for (int i = 0; i < draftContainer.transform.childCount; i++)
            {
                Destroy(draftContainer.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < deckContainer.transform.childCount; i++)
            {
                Destroy(deckContainer.transform.GetChild(i).gameObject);
            }

            // Get random draft pool from draft loot table.
            IEnumerable<Card> draftpool = settings.draftPoolLootTable.weightedLoot.GetRandomThings(settings.draftPoolSize, new System.Random());
            // Add guaranteed items.
            draftpool = draftpool.Concat(settings.guaranteedOptions);

            // Initializes the draft pool
            foreach (Card card in draftpool)
            {
                CardRenderer renderer = Instantiate(cardRendererPrefab.gameObject).GetComponent<CardRenderer>();
                renderer.transform.SetParent(draftContainer.transform, false);
                renderer.card = card;

                Toggle rendererToggle = renderer.GetComponent<Toggle>();
                rendererToggle.onValueChanged.AddListener(AddToDeck);

                UpdateNavigation();

                // Moves the renderer if it is selected.
                void AddToDeck(bool shouldAdd)
                {
                    if (shouldAdd)
                    {
                        renderer.transform.SetParent(deckContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDeck);
                        rendererToggle.onValueChanged.AddListener(AddToDraft);
                        rendererToggle.isOn = false;
                        deckSize++;
                        CheckDeckValidity();

                        UpdateNavigation();
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
                        deckSize--;
                        CheckDeckValidity();

                        UpdateNavigation();
                    }
                }

                initialSelection ??= renderer.gameObject;
            }


            // Initializes the default deck
            foreach (Card card in settings.initialDeck)
            {
                CardRenderer renderer = Instantiate(cardRendererPrefab.gameObject).GetComponent<CardRenderer>();
                renderer.transform.SetParent(deckContainer.transform, false);
                renderer.card = card;

                Toggle rendererToggle = renderer.GetComponent<Toggle>();
                rendererToggle.onValueChanged.AddListener(AddToDraft);

                UpdateNavigation();

                // Moves the renderer if it is selected.
                void AddToDeck(bool shouldAdd)
                {
                    if (shouldAdd)
                    {
                        renderer.transform.SetParent(deckContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDeck);
                        rendererToggle.onValueChanged.AddListener(AddToDraft);
                        rendererToggle.isOn = false;
                        deckSize++;
                        CheckDeckValidity();

                        UpdateNavigation();
                    }
                }
                void AddToDraft(bool shouldAdd)
                {
                    if (shouldAdd && settings.allowRemovalFromInitialDeck)
                    {
                        renderer.transform.SetParent(draftContainer.transform, false);
                        rendererToggle.onValueChanged.RemoveListener(AddToDraft);
                        rendererToggle.onValueChanged.AddListener(AddToDeck);
                        deckSize--;
                        CheckDeckValidity();

                        UpdateNavigation();
                    }
                    rendererToggle.isOn = false;
                }
            }

            CheckDeckValidity();
        }


        /// <summary>
        /// Reset initial selection.
        /// </summary>
        private void OnDisable()
        {
            initialSelection = null;
        }

        /// <summary>
        /// Calls the appropriate validity event for the current deck.
        /// </summary>
        private bool CheckDeckValidity()
        {
            if (settings.maxPlayerDeckSize < deckSize)
            {
                deckInvalidated_SizeTooLarge?.Invoke();
                return false;
            }
            else if (settings.minPlayerDeckSize > deckSize)
            {
                deckInvalidated_SizeTooSmall?.Invoke();
                return false;
            }
            else
            {
                deckValidated?.Invoke();
                return true;
            }
        }

        /// <summary>
        /// Ensures everything has proper navigation after rearrangement.
        /// </summary>
        private void UpdateNavigation()
        {
            Selectable[] deckCard = deckContainer.GetComponentsInChildren<Selectable>();
            for (int i = 0; i < deckCard.Length; i++)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnUp = draftScrollRect.horizontalScrollbar;
                navigation.selectOnDown = deckScrollRect.horizontalScrollbar;

                if (i > 0)
                {
                    Navigation neighborNavigation = deckCard[i - 1].navigation;
                    neighborNavigation.selectOnRight = deckCard[i];
                    deckCard[i - 1].navigation = neighborNavigation;

                    navigation.selectOnLeft = deckCard[i - 1];
                }
                deckCard[i].navigation = navigation;
            }

            Selectable[] draftCard = draftContainer.GetComponentsInChildren<Selectable>();
            for (int i = 0; i < draftCard.Length; i++)
            {
                Navigation navigation = new Navigation();
                navigation.mode = Navigation.Mode.Explicit;
                navigation.selectOnUp = null;
                navigation.selectOnDown = draftScrollRect.horizontalScrollbar;

                if (i > 0)
                {
                    Navigation neighborNavigation = draftCard[i - 1].navigation;
                    neighborNavigation.selectOnRight = draftCard[i];    
                    draftCard[i - 1].navigation = neighborNavigation;

                    navigation.selectOnLeft = draftCard[i - 1];
                }
                draftCard[i].navigation = navigation;
            }

            Navigation draftScrollNavigation = draftScrollRect.horizontalScrollbar.navigation;
            draftScrollNavigation.selectOnDown = deckContainer.transform.childCount > 0 ?
                deckContainer.transform.GetChild(deckContainer.transform.childCount / 2).GetComponent<Selectable>() :
                deckScrollRect.horizontalScrollbar;
            draftScrollNavigation.selectOnUp = draftContainer.transform.childCount > 0 ?
                draftContainer.transform.GetChild(draftContainer.transform.childCount / 2).GetComponent<Selectable>() :
                null;
            draftScrollRect.horizontalScrollbar.navigation = draftScrollNavigation;

            Navigation deckScrollNavigation = deckScrollRect.horizontalScrollbar.navigation;
            deckScrollNavigation.selectOnUp = deckContainer.transform.childCount > 0 ?
                deckContainer.transform.GetChild(deckContainer.transform.childCount / 2).GetComponent<Selectable>() :
                draftScrollRect.horizontalScrollbar;
            deckScrollRect.horizontalScrollbar.navigation = deckScrollNavigation;
        }
    }
}
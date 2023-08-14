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
                    }
                    rendererToggle.isOn = false;
                }
            }

            CheckDeckValidity();
        }

        // None of the below works because it does not account for updating 
        
        ///// <summary>
        ///// Sets the navigation of a card renderer that has been added to the deck.
        ///// </summary>
        ///// <param name="rendererToggle"> The toggle of the renderer. </param>
        //private void SetToDeckNavigation(Toggle rendererToggle)
        //{
        //    Selectable neighbor = draftContainer.transform.GetChild(draftContainer.transform.childCount - 1).GetComponent<Selectable>();

        //    Navigation navigation = new Navigation();
        //    navigation.mode = Navigation.Mode.Explicit;
        //    navigation.selectOnUp = draftScrollRect.horizontalScrollbar;
        //    navigation.selectOnDown = deckScrollRect.horizontalScrollbar;
        //    rendererToggle.navigation = navigation;

        //    UpdateNavigation(rendererToggle, neighbor);
        //}

        ///// <summary>
        ///// Sets the navigation of a card renderer that has been added to the draft.
        ///// </summary>
        ///// <param name="rendererToggle"> The toggle of the renderer. </param>
        //private void SetToDraftNavigation(Toggle rendererToggle)
        //{
        //    Selectable neighbor = draftContainer.transform.GetChild(draftContainer.transform.childCount - 1).GetComponent<Selectable>();

        //    Navigation navigation = new Navigation();
        //    navigation.mode = Navigation.Mode.Explicit;
        //    navigation.selectOnUp = null;
        //    navigation.selectOnDown = draftScrollRect.horizontalScrollbar;
        //    rendererToggle.navigation = navigation;

        //    UpdateNavigation(rendererToggle, neighbor);
        //}

        ///// <summary>
        ///// Ensures everything has proper navigation after rearrangement.
        ///// </summary>
        ///// <param name="rendererToggle"> The renderer that was moved. </param>
        ///// <param name="neighbor"> The new neighbor of that renderer. </param>
        //private void UpdateNavigation(Toggle rendererToggle, Selectable neighbor)
        //{
        //    Navigation navigation = rendererToggle.navigation;
        //    navigation.selectOnLeft = neighbor;
        //    rendererToggle.navigation = navigation;

        //    Navigation neighborNavigation = neighbor.navigation;
        //    neighborNavigation.selectOnRight = rendererToggle;
        //    neighbor.navigation = neighborNavigation;


        //    Navigation draftScrollNavigation = draftScrollRect.horizontalScrollbar.navigation;
        //    draftScrollNavigation.selectOnDown = deckContainer.transform.childCount > 0 ?
        //        deckContainer.transform.GetChild(deckContainer.transform.childCount / 2).GetComponent<Selectable>() :
        //        null;
        //    draftScrollNavigation.selectOnUp = draftContainer.transform.childCount > 0 ?
        //        draftContainer.transform.GetChild(draftContainer.transform.childCount / 2).GetComponent<Selectable>() :
        //        null;
        //    draftScrollRect.horizontalScrollbar.navigation = draftScrollNavigation;


        //    Navigation deckScrollNavigation = deckScrollRect.horizontalScrollbar.navigation;
        //    deckScrollNavigation.selectOnUp = deckContainer.transform.childCount > 0 ?
        //        deckContainer.transform.GetChild(deckContainer.transform.childCount / 2).GetComponent<Selectable>() :
        //        null;
        //    deckScrollRect.horizontalScrollbar.navigation = deckScrollNavigation;
        //}


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
        private void CheckDeckValidity()
        {
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
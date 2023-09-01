using System;
using System.Collections;
using System.Collections.Generic;
using Cardificer.FiniteStateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cardificer
{
    [RequireComponent(typeof(BaseStateMachine), typeof(GoldenShield), typeof(Health))]
    public class CardificerDeck : MonoBehaviour
    {
        private static CardificerDeck instance;

        [Tooltip("Cardificer's hand size")]
        [SerializeField] private int handSize = 4;

        [Tooltip("Cardificer's starting deck")]
        [SerializeField] private List<CardificerCard> cardificerDeck;

        // Cardificer's current deck
        private static List<CardificerCard> currentDeck;
        public static int cardsInDeck => currentDeck.Count;

        // Cardificer's hand
        private static CardificerCard[] currentHand;
        public static int cardsInHand => currentHand.Length;

        // Cardificer's discard pile
        private static List<CardificerCard> discardPile;
        public static int cardsInDiscardPile => discardPile.Count;

        // Cardificer's state machine
        private BaseStateMachine stateMachine;

        // Currently selected card index in hand
        private int selectedCard = 0;

        public static int selectedCardIndex
        {
            get => instance.selectedCard;
            set => instance.selectedCard = value > cardsInHand - 1 || value <= 0 ? 0 : value;
        }

        [HideInInspector] public static int playableCardsInHand => PlayableCardsInHand();

        /// <summary>
        /// Assigns instance
        /// </summary>
        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Initializes various variables
        /// </summary>
        void Start()
        {
            Shuffle(cardificerDeck);
            currentDeck = cardificerDeck;
            currentHand = new CardificerCard[4];
            discardPile = new List<CardificerCard>();
            stateMachine = GetComponent<BaseStateMachine>();
            stateMachine.GetComponent<Health>().onDamageTaken += OnDamageTaken;
        }

        /// <summary>
        /// Called whenever the Cardificer takes damage, activates golden shield if health is below 50%
        /// </summary>
        void OnDamageTaken()
        {
            // safe to do repeated GetComponent calls because the stateMachine caches GetComponent
            if (!stateMachine.GetComponent<GoldenShield>().goldenShieldActive &&
                stateMachine.GetComponent<Health>().currentHealth <=
                stateMachine.GetComponent<Health>().maxHealth * 0.5f)
            {
                stateMachine.GetComponent<GoldenShield>().ActivateGoldenShield();
            }
        }

        /// <summary>
        /// Gets a card from the Cardificer's hand at the given index
        /// </summary>
        /// <param name="index"> The index in the hand </param>
        /// <returns> The card retrieved, or null if the index is out of bounds </returns>
        public static CardificerCard GetCardFromHand(int index)
        {
            if (currentHand == null || index < 0 || index > currentHand.Length - 1)
            {
                return null;
            }
            else
            {
                CardificerCard chosenCard = currentHand[index];
                return chosenCard; // this can be null if the slot is empty
            }
        }

        /// <summary>
        /// Reshuffles the discard pile back into the deck
        /// </summary>
        public static void ReshuffleDiscardIntoDeck()
        {
            // Add discarded cards back to deck
            for (int i = 0; i < cardsInDiscardPile; i++)
            {
                currentDeck.Add(discardPile[i]);
            }

            // Clear discard pile
            discardPile.Clear();

            // Shuffle deck
            Shuffle(currentDeck);
        }

        /// <summary>
        /// Adds a card at the given index in the hand to the discard pile, then removes that card from the hand
        /// </summary>
        /// <param name="index"> The index in the hand </param>
        public static void DiscardFromHand(int index)
        {
            discardPile.Add(currentHand[index]);

            currentHand[index] = null;
        }

        /// <summary>
        /// Attempts to place the first card from the deck into the hand. Does not draw if the hand is full or deck is empty.
        /// </summary>
        /// <returns> True if a card was drawn, false otherwise </returns>
        public static bool TryDrawCard()
        {
            if (PlayableCardsInHand() < instance.handSize && cardsInDeck > 0)
            {
                // Put a card from the deck into the hand
                if (PlaceInHand(currentDeck[0]))
                {
                    currentDeck.RemoveAt(0);
                    return true;
                }
                // hand placement failed (likely because hand is full)
                return false;
            }
            else
            {
                // Either the hand is full or there are no more cards in the deck to draw, so return false
                return false;
            }
        }

        private static int PlayableCardsInHand()
        {
            if (playableCardsInHand == 0) return 0;

            int validIndicesCount = 0;

            for (int i = 0; i < cardsInHand; i++)
            {
                if (currentHand[i] != null && currentHand[i].playable)
                {
                    validIndicesCount++;
                }
            }
            
            return validIndicesCount;
            
        }
        
        /// <summary>
        /// Places a given CardificerCard into an empty slot in the hand. If there is no empty slot, does nothing.
        /// </summary>
        /// <param name="card"> The card to be placed </param>
        /// <returns> True if a card was placed, false otherwise </returns>
        private static bool PlaceInHand(CardificerCard card)
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                if (currentHand[i] == null)
                {
                    currentHand[i] = card;
                    return true;
                }
            }

            return false;
        }

        public static void SelectRandomCard()
        {
            selectedCardIndex = GetRandomPlayableCardIndex();
        }

        /// <summary>
        /// Utilizes Fisher-Yates shuffle to shuffle the given list of CardificerCards
        /// </summary>
        /// <param name="list"> The list of CardificerCards to be shuffled </param>
        private static void Shuffle(List<CardificerCard> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static int GetRandomPlayableCardIndex()
        {
            if (playableCardsInHand == 0) return 0;

            List<int> validIndices = new List<int>();

            for (int i = 0; i < cardsInHand; i++)
            {
                if (currentHand[i] != null && currentHand[i].playable)
                {
                    validIndices.Add(i);
                }
            }
            
            return validIndices[Random.Range(0, validIndices.Count)];
        }

        public static void DiscardHand()
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                if (currentHand[i] != null)
                {
                    DiscardFromHand(i);
                }
            }
        }
    }
}
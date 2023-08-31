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
        private static List<CardificerCard> currentHand;
        public static int cardsInHand => currentHand.Count;

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
            currentHand = new List<CardificerCard>();
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
        /// <param name="shouldDiscard"> Whether to discard the card </param>
        /// <returns> The card retrieved, or null if the index is out of bounds </returns>
        public static CardificerCard GetCardFromHand(int index, bool shouldDiscard = false)
        {
            if (currentHand == null || index < 0 || index > currentHand.Count - 1)
            {
                return null; 
            }
            else
            {
                CardificerCard chosenCard = currentHand[index];
                if (shouldDiscard)
                {
                    discardPile.Add(chosenCard);
                    currentHand.RemoveAt(index);
                }
                return chosenCard;
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
        /// Moves all cards from the hand into the discard pile
        /// </summary>
        public static void DiscardHand()
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                DiscardFromHand(i);
            }
        }
        
        /// <summary>
        /// Adds a card at the given index in the hand to the discard pile, then removes that card from the hand
        /// </summary>
        /// <param name="index"> The index in the hand </param>
        public static void DiscardFromHand(int index)
        {
           discardPile.Add(currentHand[index]);
           currentHand.RemoveAt(index);
        }

        /// <summary>
        /// Attempts to place the first card from the deck into the hand. Does not draw if the hand is full or deck is empty.
        /// </summary>
        /// <returns> True if a card was drawn, false otherwise </returns>
        public static bool TryDrawCard()
        {
            if (cardsInHand < instance.handSize && cardsInDeck > 0)
            {
                // Put a card from the deck into the hand
                currentHand.Add(currentDeck[0]);
                currentDeck.RemoveAt(0);
                return true;
            }
            else
            {
                // Either the hand is full or there are no more cards in the deck to draw, so return false
                return false;
            }
        }

        /// <summary>
        /// Utilizes Fisher-Yates shuffle to shuffle the given list of CardificerCards
        /// </summary>
        /// <param name="list"> The list of CardificerCards to be shuffled </param>
        private static void Shuffle(List<CardificerCard> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = Random.Range(0, n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    }
}
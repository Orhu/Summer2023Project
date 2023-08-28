using System.Collections;
using System.Collections.Generic;
using Cardificer.FiniteStateMachine;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Cardificer
{
    public class CardificerDeck : MonoBehaviour
    {
        [CreateAssetMenu(menuName="Cards/CardificerCard")]
        public class CardificerCard : ScriptableObject
        {
            [Tooltip("Sprite representing the rune of this card")]
            [SerializeField] public Sprite runeSprite;

            [Tooltip("Action time (duration) of this card")]
            [SerializeField] public float actionTime;

            [Tooltip("What state to enter when this card is played")]
            [SerializeField] public BaseState stateToEnter;
            
            [Tooltip("What state to enter when the action time is complete")]
            [SerializeField] public BaseState stateToExit;

        }

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
            currentDeck = cardificerDeck;
            currentHand = new List<CardificerCard>();
            discardPile = new List<CardificerCard>();
            
        }
        
        /// <summary>
        /// Gets a card from the Cardificer's hand at the given index
        /// </summary>
        /// <param name="index"> The index in the hand </param>
        /// <returns> The card retrieved, or null if the index is out of bounds </returns>
        public static CardificerCard GetCardFromHand(int index)
        {
            if (index > currentHand.Count - 1 || index < 0)
            {
                return null;
            }
            else
            {
                return currentHand[index];
            }
        }

        /// <summary>
        /// Retrieves a card from the top of the hand, and optionally deletes the card from the hand
        /// </summary>
        /// <param name="card"> The card </param>
        /// <param name="deleteCardAfterGrab"> Whether or not to delete the card after it is grabbed </param>
        /// <returns> True if a card was grabbed, false if the hand is empty </returns>
        public static bool GetTopCard(out CardificerCard card, bool deleteCardAfterGrab = false)
        {
            if (cardsInHand > 0)
            {
                card = currentHand[0];
                if (deleteCardAfterGrab)
                {
                    currentHand.RemoveAt(0);
                }

                return true;
            }
            else
            {
                card = null;
                return false;
            }
        }

        /// <summary>
        /// Reshuffles the discard pile and hand back into the deck, then refills the hand
        /// </summary>
        private static void ReshuffleDeckAndHand()
        {
            // Place hand into discard pile
            DiscardHand();
            
            // Add discarded cards back to deck
            for (int i = 0; i < cardsInDiscardPile; i++)
            {
                currentDeck.Add(discardPile[i]);
            }

            // Shuffle deck
            Shuffle(currentDeck);
            
            // Clear hand and discard pile
            discardPile.Clear();
            currentHand.Clear();

            // Fill up the hand with cards
            instance.StartCoroutine(DrawCards());
            IEnumerator DrawCards()
            {
                while (TryDrawCard())
                {
                    yield return null;
                }
                yield break;
            }
        }

        /// <summary>
        /// Moves all cards from the hand into the discard pile
        /// </summary>
        private static void DiscardHand()
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                discardPile.Add(currentHand[i]);
            }
            
            currentHand.Clear();
        }

        /// <summary>
        /// Attempts to place the first card from the deck into the hand. Does not draw if the hand is full or deck is empty.
        /// </summary>
        /// <returns> True if a card was drawn, false otherwise </returns>
        private static bool TryDrawCard()
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
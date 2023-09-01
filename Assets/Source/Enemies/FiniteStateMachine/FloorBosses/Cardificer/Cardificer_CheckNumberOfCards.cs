using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that can perform various checks on the Cardificer's deck
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Check Number of Cards")]
    public class Cardificer_CheckNumberOfCards : Decision
    {
        [Tooltip("Number of cards to check for (3 and GreaterThan would mean 3 > # of cards in hand)")] 
        [SerializeField] private int numberToCheck;

        [SerializeField] private CompareStateVariableToInt.ComparisonType comparisonType;
        
        [SerializeField] private CardificerDeckType deckType;
        
        /// <summary>
        /// Enum used for representing a Cardificer deck type
        /// </summary>
        private enum CardificerDeckType
        {
            Deck,
            Hand,
            DiscardPile
        }

        /// <summary>
        /// Checks the number of cards in the given deck, and returns comparison result based on the inputted values into the scriptable object
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> True if the numberOfCards meets the comparison requirement, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            // Find the number we want to compare to
            int threshold = 0;
            switch (deckType)
            {
                case CardificerDeckType.Deck:
                    threshold = CardificerDeck.cardsInDeck;
                    break;
                case CardificerDeckType.Hand:
                    threshold = CardificerDeck.playableCardsInHand;
                    break;
                case CardificerDeckType.DiscardPile:
                    threshold = CardificerDeck.cardsInDiscardPile;
                    break;
                default:
                    Debug.LogError("Provided with an invalid deck type when checking number of cards in a deck! Invalid comparison may occur.");
                    break;
            }
            
            // Perform comparison
            switch (comparisonType)
            {
                case CompareStateVariableToInt.ComparisonType.Equal:
                    return numberToCheck == threshold;
                case CompareStateVariableToInt.ComparisonType.GreaterThan:
                    return numberToCheck > threshold;
                case CompareStateVariableToInt.ComparisonType.GreaterThanOrEqual:
                    return numberToCheck >= threshold;
                case CompareStateVariableToInt.ComparisonType.LessThan:
                    return numberToCheck < threshold;
                case CompareStateVariableToInt.ComparisonType.LessThanOrEqual:
                    return numberToCheck <= threshold;
                default:
                    Debug.LogError("Provided with an invalid comparison type when checking number of cards in a deck! Returning false.");
                    return false;
            }
        }
    }
}
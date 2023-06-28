using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true if the given key exists and its value is equal to the given int value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cauldron of Desire/All Cards Drawn")]
    public class COD_AllCardsDrawn : Decision
    {
        /// <summary>
        /// Returns true if the given key exists and its value is equal to the given int value
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> true if the given key exists and its value is equal to the given int value, false otherwise </returns>
        protected override bool Evaluate(BaseStateMachine state)
        {
            var cardsDrawnExists = state.trackedVariables.TryGetValue("CardsDrawn", out var cardsDrawn);
            var cardsToDrawExists = state.trackedVariables.TryGetValue("CardsToDraw", out var cardsToDraw);
            return cardsDrawnExists && cardsToDrawExists && (int)cardsDrawn == (int)cardsToDraw;
        }
    }
}
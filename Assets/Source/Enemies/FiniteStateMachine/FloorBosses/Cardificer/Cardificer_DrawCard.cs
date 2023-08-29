using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that draws a Cardificer card from the top of the deck
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Draw Card")]
    public class Cardificer_DrawCard : SingleAction
    {
        /// <summary>
        /// Attempts to draw a card from the deck into the hand
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            CardificerDeck.TryDrawCard();
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
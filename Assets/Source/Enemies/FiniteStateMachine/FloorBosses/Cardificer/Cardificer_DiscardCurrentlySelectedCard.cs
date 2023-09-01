using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that discards the currently selected card in hand
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Discard Selected Card")]
    public class Cardificer_DiscardCurrentlySelectedCard : SingleAction
    {
        /// <summary>
        /// Discards the currently selected card in hand
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            CardificerDeck.DiscardFromHand(CardificerDeck.selectedCardIndex);
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
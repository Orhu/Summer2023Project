using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that discards whatever CardificerCard is currently selected
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Discard Currently Selected Card")]
    public class Cardificer_DiscardSelectedCard : SingleAction
    {
        /// <summary>
        /// Discards whatever CardificerCard is currently selected
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
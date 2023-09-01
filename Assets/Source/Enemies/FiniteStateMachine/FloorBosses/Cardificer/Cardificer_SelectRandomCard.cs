using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that selects a random card from hand and plays an animation for it
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Select Random Card")]
    public class Cardificer_SelectRandomCard : SingleAction
    {
        /// <summary>
        /// Selects a random card from hand and plays an animation for it
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            CardificerDeck.SelectRandomCard(); // sets selectedCardIndex to a random playable card
            stateMachine.GetComponentInChildren<CardificerHandRenderer>().AnimateSelectCard(); // plays animation (duration dependent on HandRenderer component configuration)
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
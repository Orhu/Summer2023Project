using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that discards the Cardificer's hand
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Discard Hand")]
    public class Cardificer_DiscardHand : SingleAction
    {
        /// <summary>
        /// Discards the Cardificer's hand
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            CardificerDeck.DiscardHand();
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
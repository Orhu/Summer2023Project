using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the HandRenderer's highlight to the currently selected card
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Set Highlight to Selected Card")]
    public class Cardificer_SetHighlightToSelectedCard : SingleAction
    {
        /// <summary>
        /// Sets the highlight to the currently selected card
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.GetComponentInChildren<CardificerHandRenderer>().SetHighlightToSelectedCard();
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
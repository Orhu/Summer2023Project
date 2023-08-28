using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that plays the Cardificer card at the first slot in the Cardificer's hand
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Play Card")]
    public class Cardificer_PlayCard : SingleAction
    {
        /// <summary>
        /// Plays the card attached to this scriptable object
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits until the action time on the card is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            bool cardAvailable = CardificerDeck.GetTopCard(out CardificerCard cardToPlay, true);

            if (cardAvailable)
            {
                stateMachine.currentState = cardToPlay.stateToEnter.GetState();
                yield return new WaitForSeconds(cardToPlay.actionTime);
                stateMachine.currentState = cardToPlay.stateToExit.GetState();

                stateMachine.cooldownData.cooldownReady[this] = true;
            } else 
            {
                Debug.LogWarning("Cardificer attempted to play a card, but no card was available in the hand. Cardificer will not play a card.");
                stateMachine.cooldownData.cooldownReady[this] = true;
                yield break;
            } 
        }
    }
}
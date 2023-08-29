using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that draws a Cardificer card from the top of the deck
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cardificer/Shuffle Discard Pile Into Deck")]
    public class Cardificer_ShuffleDiscardIntoDeck : SingleAction
    {
        /// <summary>
        /// Shuffles the discard pile into the deck
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            CardificerDeck.ReshuffleDiscardIntoDeck();
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
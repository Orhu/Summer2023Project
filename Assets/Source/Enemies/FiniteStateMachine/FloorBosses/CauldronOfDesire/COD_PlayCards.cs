using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackSequence = Cardificer.FiniteStateMachine.COD_DrawCards.AttackSequence;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that plays all cards currently drawn
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cauldron of Desire/Play Cards")]
    public class COD_PlayCards : SingleAction
    {
        /// <summary>
        /// Plays the attack sequence index as indicated by the CardsPlayed variable, then adds one to the CardsPlayed variable
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits amount of time specified by the given attack sequence </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("CardsPlayed", 0);

            if (!stateMachine.trackedVariables.ContainsKey("Card0"))
            {
                Debug.LogError("Cauldron of Desire has no cards to play even though it is in its attack state!");
            }

            var cardPlayIndex = (int)stateMachine.trackedVariables["CardsPlayed"];
            
            AttackSequence currentAttackSequence = stateMachine.trackedVariables["Card" + cardPlayIndex] as AttackSequence;

            List<Action> actionSequence = currentAttackSequence.actionSequence;
            List<float> actionDelaySequence = currentAttackSequence.actionDelaySequence;
            for (int i = 0; i < currentAttackSequence.actionSequence.Count; i++)
            {
                actionSequence[i].Play(stateMachine);
                yield return new WaitForSeconds(actionDelaySequence[i]);
            }
            
            stateMachine.trackedVariables["CardsPlayed"] = (int)stateMachine.trackedVariables["CardsPlayed"] + 1;
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
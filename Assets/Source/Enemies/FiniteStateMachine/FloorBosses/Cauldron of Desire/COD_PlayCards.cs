using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;
using AttackSequence = Cardificer.FiniteStateMachine.COD_DrawCards.AttackSequence;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that plays all cards currently drawn
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cauldron of Desire/Play Cards")]
    public class COD_PlayCards : SingleAction
    {
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("CardsPlayed", 0);
            stateMachine.trackedVariables.TryAdd("CardsDone", 0);

            if (!stateMachine.trackedVariables.ContainsKey("Card0"))
            {
                Debug.LogError("Cauldron of Desire has no cards to play even though it is in its attack state!");
            }

            var cardPlayIndex = (int)stateMachine.trackedVariables["CardsPlayed"];
            
            AttackSequence currentAttackSequence = stateMachine.trackedVariables["Card" + cardPlayIndex] as AttackSequence;
            BaseStateMachine.print("Attack " + cardPlayIndex + " being fired!");

            var actionSequence = currentAttackSequence.actionSequence;
            var actionDelaySequence = currentAttackSequence.actionDelaySequence;
            for (int i = 0; i < currentAttackSequence.actionSequence.Count; i++)
            {
                stateMachine.currentAttackTarget = Player.Get().transform.position;
                actionSequence[i].Play(stateMachine);
                BaseStateMachine.print("Just fired index " + i);
                yield return new WaitForSeconds(actionDelaySequence[i]);
            }
            
            stateMachine.trackedVariables["CardsPlayed"] = (int)stateMachine.trackedVariables["CardsPlayed"] + 1;
            stateMachine.trackedVariables["CardsDone"] = (int)stateMachine.trackedVariables["CardsDone"] + 1;
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
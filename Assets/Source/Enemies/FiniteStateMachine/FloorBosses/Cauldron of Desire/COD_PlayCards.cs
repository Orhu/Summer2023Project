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
        [Tooltip("How long to wait after an attack sequence is played before playing the next one")]
        [SerializeField] private float delayBetweenAttacks;
        
        [Tooltip("After playing one attack in the sequence, how long should we wait before playing the next?")]
        [SerializeField] private float delayBetweenProjectiles;
        
        // tracks number of cards to play based on how many were drawn
        private int numberCardsToPlay;

        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            numberCardsToPlay = (int)stateMachine.trackedVariables["CardsDrawn"];

            stateMachine.trackedVariables.TryAdd("CardsPlayed", 0);
            stateMachine.trackedVariables["CardsPlayed"] =
                0; // if it was already added, the TryAdd wont update the value

            if (!stateMachine.trackedVariables.ContainsKey("Card0"))
            {
                Debug.LogError("Cauldron of Desire has no cards to play even though it is in its attack state!");
            }
            
            for (int i = 0; i < numberCardsToPlay; i++)
            {
                AttackSequence currentAttack = stateMachine.trackedVariables["Card" + i] as AttackSequence;
                BaseStateMachine.print("Attack " + i + " firing!");
                foreach (var attack in currentAttack.actionSequence)
                {
                    yield return new WaitForSeconds(delayBetweenProjectiles);
                    stateMachine.currentAttackTarget = Player.Get().transform.position;
                    attack.Play(stateMachine);
                }
                BaseStateMachine.print("Attack " + i + " finished firing");
                yield return new WaitForSeconds(delayBetweenAttacks);
                stateMachine.trackedVariables["CardsPlayed"] = (int)stateMachine.trackedVariables["CardsPlayed"] + 1;
            }
            
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

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
            stateMachine.trackedVariables["CardsPlayed"] = 0; // if it was already added, the TryAdd wont update the value
            
            for (int i = 0; i < numberCardsToPlay; i++)
            {
                stateMachine.currentAttackTarget = Player.Get().transform.position;
                List<Action> currentAttack = (List<Action>)stateMachine.trackedVariables["Card" + i];
                foreach (var attack in currentAttack)
                {
                    yield return new WaitForSeconds(delayBetweenProjectiles);
                    attack.Play(stateMachine);
                }
                yield return new WaitForSeconds(delayBetweenAttacks);
                stateMachine.trackedVariables["CardsPlayed"] = (int)stateMachine.trackedVariables["CardsPlayed"] + 1;
            }
            
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
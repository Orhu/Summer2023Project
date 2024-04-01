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
            //get animator used to trigger when an attack starts/ stops
            Animator anim = stateMachine.gameObject.transform.Find("Sprite").GetComponent<Animator>();

            stateMachine.trackedVariables.TryAdd("CardsPlayed", 0);
            stateMachine.trackedVariables.TryAdd("ActionTimeComplete", false);

            if (!stateMachine.trackedVariables.ContainsKey("Card0"))
            {
                Debug.LogError("Cauldron of Desire has no cards to play even though it is in its attack state!");
            }

            var cardPlayIndex = (int)stateMachine.trackedVariables["CardsPlayed"];
            
            AttackSequence currentAttackSequence = stateMachine.trackedVariables["Card" + cardPlayIndex] as AttackSequence;

            List<Attack> actionSequence = currentAttackSequence.actionSequence;
            
            for (int i = 0; i < currentAttackSequence.actionSequence.Count; i++)
            {
                stateMachine.trackedVariables["ActionTimeComplete"] = false;

                //open mouth - works but decided not to include
                //anim.SetBool("isAttacking", true);

                actionSequence[i].Play(stateMachine, FloorGenerator.currentRoom.livingEnemies, () =>
                {
                    stateMachine.trackedVariables["ActionTimeComplete"] = true;
                });
                
                yield return new WaitUntil(() => (bool)stateMachine.trackedVariables["ActionTimeComplete"]);
            }

            //close mouth - works but decided not to include
            //anim.SetBool("isAttacking", false);

            stateMachine.cooldownData.cooldownReady[this] = true;
            stateMachine.trackedVariables["CardsPlayed"] = (int)stateMachine.trackedVariables["CardsPlayed"] + 1;
            yield break;
        }
    }
}
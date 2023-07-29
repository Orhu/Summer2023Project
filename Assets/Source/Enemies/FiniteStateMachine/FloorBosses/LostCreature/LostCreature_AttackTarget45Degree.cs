using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the attack target to face in whichever 45 degree angle is the farthest
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Attack Target Random 45 Degree")]
    public class LostCreature_AttackTarget45Degree : SingleAction
    {
        /// <summary>
        /// Sets attack target to a random diagonal
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 statePos = stateMachine.transform.position;
            Vector2 upLeft = statePos + Vector2.up + Vector2.left;
            Vector2 upRight = statePos + Vector2.up + Vector2.right;
            Vector2 downLeft = statePos + Vector2.down + Vector2.left;
            Vector2 downRight = statePos + Vector2.down + Vector2.right;
            Vector2[] possiblePicks = { upLeft, upRight, downLeft, downRight };

            stateMachine.currentAttackTarget = possiblePicks[UnityEngine.Random.Range(0, possiblePicks.Length)];
            
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
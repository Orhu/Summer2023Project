using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackSequence = Cardificer.FiniteStateMachine.COD_DrawCards.AttackSequence;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that teleports the state machine to a given location after a given delay
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Teleport After Delay")]
    public class LostCreature_TeleportAfterDelay : SingleAction
    {
        [Tooltip("The position to teleport to, relative to room center")]
        public Vector2 posToTeleportTo;
        
        [Tooltip("Teleport will happen after how many seconds?")]
        public float delay;
        
        /// <summary>
        /// After the given delay, teleports the state machine to the given position
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits amount of time specified before teleporting. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            yield return new WaitForSeconds(delay);
            stateMachine.transform.position = FloorGenerator.currentRoom.roomLocation + posToTeleportTo;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
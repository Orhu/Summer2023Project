using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackSequence = Cardificer.FiniteStateMachine.COD_DrawCards.AttackSequence;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that plays an attack, then marks ActionPlayed as true when the action is complete.
    /// Additionally, since this attack is for Ancient Power, we check what side the boss is on and determine the attack to fire from that.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Play Ancient Power")]
    public class LostCreature_PlayAncientPower : SingleAction
    {
        [Tooltip("Attack to launch if boss is on north or south wall.")]
        public Attack NorthSouthAttack;

        [Tooltip("Attack to launch if boss is on east or west wall.")]
        public Attack EastWestAttack;

        /// <summary>
        /// Plays the given attack, and marks ActionTimeComplete true when the attack action time is done
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits amount of time specified by the given attack sequence </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("ActionTimeComplete", false);
            stateMachine.trackedVariables["ActionTimeComplete"] = false;

            int roomSide = (int)stateMachine.trackedVariables["RoomSide"];

            Attack attack;
            if (roomSide == 0 || roomSide == 3)
            {
                // North or South
                attack = NorthSouthAttack;
            }
            else
            {
                // East or West
                attack = EastWestAttack;
            }
            
            attack.Play(stateMachine, FloorGenerator.currentRoom.livingEnemies, () =>
            {
                stateMachine.trackedVariables["ActionTimeComplete"] = true;
                stateMachine.cooldownData.cooldownReady[this] = true;
            });
            yield break;
        }
    }
}
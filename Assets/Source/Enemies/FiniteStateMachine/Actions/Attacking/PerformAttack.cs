using System.Collections;
using System.Runtime.InteropServices;
using Skaillz.EditInline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that fires an attack
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Attacking/Perform Attack")]
    public class PerformAttack : SingleAction
    {
        [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
        public float actionChargeUpTime;

        [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
        public float actionCooldownTime;

        [Tooltip("The actions that will be taken when the enemy attempts to issue an action.")] [EditInline]
        public Cardificer.Action[] actions;

        /// <summary>
        /// Fire an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StartCoroutine(LaunchAttack(stateMachine));
            yield break;
        }

        /// <summary>
        /// Performs an attack if canAct is enabled, otherwise does nothing
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        IEnumerator LaunchAttack(BaseStateMachine stateMachine)
        {
            yield return new WaitForSeconds(actionChargeUpTime);
            if (stateMachine.canAct)
            {
                foreach (var action in actions)
                {
                    action.Play(stateMachine, FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies);
                }
                yield return new WaitForSeconds(actionCooldownTime);
            }
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
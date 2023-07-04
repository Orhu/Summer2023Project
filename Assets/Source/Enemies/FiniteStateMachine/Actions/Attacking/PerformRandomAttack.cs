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
    [CreateAssetMenu(menuName = "FSM/Actions/Attacking/Perform Random Attack")]
    public class PerformRandomAttack : SingleAction
    {
        [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
        public float actionChargeUpTime;

        [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
        public float actionCooldownTime;

        [Tooltip("The actions that will be taken when the enemy attempts to issue an action and the probability of each happening.")]
        public GenericWeightedThings<Action> actions;

        /// <summary>
        /// Fire an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        /// <returns> The time to wait until this is finished. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StartCoroutine(LaunchAttack(stateMachine));
            yield break;
        }

        /// <summary>
        /// Performs an attack if canAct is enabled, otherwise does nothing
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        private IEnumerator LaunchAttack(BaseStateMachine stateMachine)
        {
            yield return new WaitForSeconds(actionChargeUpTime);
            if (stateMachine.canAct)
            {
                actions.GetRandomThing().Play(stateMachine, FloorGenerator.currentRoom.livingEnemies);
                yield return new WaitForSeconds(actionCooldownTime);
            }
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
using System.Collections;
using Skaillz.EditInline;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Tooltip("The attacks that will be launched when the enemy attempts to attack.")] [EditInline]
        public Attack[] attacks;

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
                foreach (var attack in attacks)
                {
                    attack.Play(stateMachine, FloorGenerator.currentRoom.livingEnemies, () => stateMachine.cooldownData.cooldownReady[this] = true);
                }
            }
        }
    }
}
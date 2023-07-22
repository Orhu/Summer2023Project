using System.Collections;
using System.Collections.Generic;
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

        [Tooltip("Whether or not this will hit everything.")]
        public bool friendlyFire = false;

        /// <summary>
        /// Fire an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        /// <returns></returns>
        protected sealed override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("NumOfActiveProjectiles", 0);

            if (!stateMachine.trackedVariables.ContainsKey("OnAttack"))
            {
                stateMachine.trackedVariables.Add("OnAttack", null);
            }

            stateMachine.StartCoroutine(LaunchAttack(stateMachine));
            yield break;
        }

        /// <summary>
        /// The objects the attacks of this will ignore.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        protected virtual List<GameObject> getIgnoredObjects(BaseStateMachine stateMachine)
        {
            return friendlyFire ? new List<GameObject>() : FloorGenerator.currentRoom.livingEnemies;
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
                for (int i = 0; i < attacks.Length; i++)
                {
                    stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                        (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] + 1;
                    attacks[i].Play(stateMachine, getIgnoredObjects(stateMachine), 
                        () =>
                        {
                            stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                                (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] - 1;
                            stateMachine.cooldownData.cooldownReady[this] = true;
                        });

                    (stateMachine.trackedVariables["OnAttack"] as System.Action)?.Invoke();
                }
            }
        }
    }
}
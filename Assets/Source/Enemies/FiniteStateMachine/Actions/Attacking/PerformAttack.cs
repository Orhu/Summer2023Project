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
    public class PerformAttack : PerformAction
    {
        [Tooltip("The attacks that will be launched when the enemy attempts to attack.")]
        [SerializeField] private Action[] attacks;

        /// <summary>
        /// Gets the attacks that will be used by this
        /// </summary>
        /// <returns> The attacks that will be launched when the enemy attempts to attack. </returns>
        public override Action[] GetAttacks()
        { 
            return attacks; 
        }
    }

    /// <summary>
    /// Represents an action that fires an attack
    /// </summary>
    public abstract class PerformAction : SingleAction
    {
        [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
        public float actionChargeUpTime;

        [Tooltip("Whether or not this will hit everything.")]
        public bool friendlyFire = false;

        /// <summary>
        /// Gets the attacks that will be used by this
        /// </summary>
        /// <returns> The attacks that will be launched when the enemy attempts to attack. </returns>
        public abstract Action[] GetAttacks();

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
            yield return new UnityEngine.WaitForSeconds(actionChargeUpTime);
            if (stateMachine.canAct)
            {
                bool attackPlayed = false;
                foreach (Action action in GetAttacks())
                {
                    if (action is Attack attack)
                    {
                        stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                            (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] + 1;
                        attack.Play(stateMachine, getIgnoredObjects(stateMachine),
                            () =>
                            {
                                stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                                    (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] - 1;
                                stateMachine.cooldownData.cooldownReady[this] = true;
                            });
                        attackPlayed = true;
                    }
                    else
                    {
                        action.Play(stateMachine, getIgnoredObjects(stateMachine));
                    }

                }

                if (!attackPlayed)
                {
                    stateMachine.cooldownData.cooldownReady[this] = true;
                }
                (stateMachine.trackedVariables["OnAttack"] as System.Action)?.Invoke();
            }
            else
            {
                stateMachine.cooldownData.cooldownReady[this] = true;
            }
        }
    }
}
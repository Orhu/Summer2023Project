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

        [Tooltip("Projectile Lifetime? Only needed if tracking projectile counts (such as the Trapper)")]
        public float projectileLifetime;

        /// <summary>
        /// Fire an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("NumOfActiveProjectiles", 0);
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
                for (int i = 0; i < attacks.Length; i++)
                {
                    stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                        (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] + 1;
                    // if this is the last attack in the sequence we want to use its action time to enable cooldown
                    if (i == attacks.Length - 1)
                    {
                        attacks[i].Play(stateMachine, FloorGenerator.currentRoom.livingEnemies, () =>
                        {
                            stateMachine.cooldownData.cooldownReady[this] = true;
                        });
                        
                        yield return new WaitForSeconds(attacks[i].lifetime); // track lifetime and decrement accordingly
                        stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                            (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] - 1;
                    } // otherwise, just play the attack and do not enable cooldown when it finishes
                    else
                    {
                        attacks[i].Play(stateMachine, FloorGenerator.currentRoom.livingEnemies);
                        
                        yield return new WaitForSeconds(attacks[i].lifetime);  // track lifetime and decrement accordingly
                        stateMachine.trackedVariables["NumOfActiveProjectiles"] =
                            (int)stateMachine.trackedVariables["NumOfActiveProjectiles"] - 1;
                    }
                }
            }
        }
    }
}
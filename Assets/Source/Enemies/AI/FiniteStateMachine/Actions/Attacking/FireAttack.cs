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
    public class FireAttack : Action
    {
        [Tooltip("After requesting an action, how long does it take for the action to be performed?")]
        public float actionChargeUpTime;

        [Tooltip("After the action is performed, what is the delay before the action can be performed again?")]
        public float actionCooldownTime;

        // [Tooltip("How many seconds to apply the Exhaust status effect. Note: Total cooldown of an attack is equal to exhaust duration + cooldown duration")]
        //  public float exhaustDuration;

        [Tooltip("The actions that will be taken when the enemy attempts to issue an action.")] [EditInline]
        public Cardificer.Action[] actions;

        // [Tooltip("Before the action charge up is started, do you want to do anything else?")]
        //  public UnityEvent<BaseStateMachine> beforeChargeUp;

        //  [Tooltip("Before the action is performed, do you want to do anything else?")]
        //  public UnityEvent<BaseStateMachine> beforeAction;

        //  [Tooltip("Immediately after the action is performed, do you want to do anything else?")]
        // public UnityEvent<BaseStateMachine> afterAction;

        // [Tooltip("After the cooldown ends, do you want to do anything else?")]
        // public UnityEvent<BaseStateMachine> afterCooldown;

        /// <summary>
        /// Fire an attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StartCoroutine(PerformAttack(stateMachine));
            yield break;
        }

        /// <summary>
        /// Performs an attack if canAct is enabled, otherwise does nothing
        /// </summary>
        /// <param name="stateMachine"> The stateMachine performing the attack </param>
        IEnumerator PerformAttack(BaseStateMachine stateMachine)
        {
            // beforeChargeUp?.Invoke(stateMachine);
            yield return new WaitForSeconds(actionChargeUpTime);
            // beforeAction?.Invoke(stateMachine);
            if (stateMachine.canAct)
            {
                foreach (var action in actions)
                {
                    action.Play(stateMachine, FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies);
                }
                yield return new WaitForSeconds(actionCooldownTime);
            }

            // afterCooldown?.Invoke(stateMachine);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
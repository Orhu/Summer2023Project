using System.Collections;
using UnityEngine;

namespace Cardificer
{

    /// <summary>
    /// Represents an action in a finite state machine
    /// </summary>
    public abstract class FSMAction : BaseAction
    {
        /// <summary>
        /// If this action's cooldown is ready, then execute the action.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            if (stateMachine.cooldownData.cooldownReady[this])
            {
                stateMachine.cooldownData.cooldownReady[this] = false;
                stateMachine.StartCoroutine(PlayAction(stateMachine));
            }
        }

        /// <summary>
        /// Execute this action. All inheriting methods need to make sure to use stateMachine.cooldownData.cooldownReady[this] = true to re-enable the action.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public abstract IEnumerator PlayAction(BaseStateMachine stateMachine);
    }
}
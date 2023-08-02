using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that executes multiple actions
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Multi Action")]
    public class MultiAction : BaseAction
    {
        [Tooltip("Actions to perform.")]
        [SerializeField] private List<BaseAction> actions;

        /// <summary>
        /// Execute all provided actions.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            foreach (var action in actions)
            {
                action.Execute(stateMachine);
            }
        }
    }
}
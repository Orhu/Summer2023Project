
using System.Collections;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that disables following the current path and enables ignoring any new pathfinding requests
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Set Movement Mode")]
    public class SetMovementMode : SingleAction
    {
        [Tooltip("The movement mode to change this to.")]
        [SerializeField] private MovementType newMovemenetType;

        /// <summary>
        /// Stops the follow path coroutine 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.currentMovementType = newMovemenetType;
            yield break;
        }
    }
}

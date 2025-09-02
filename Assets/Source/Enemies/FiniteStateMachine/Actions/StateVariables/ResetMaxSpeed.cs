
using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that resets the max speed back to its default value.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Reset Max Speed")]
    public class ResetMaxSpeed : SingleAction
    {
        /// <summary>
        /// Resets the max speed back to its default value. 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.GetComponent<SimpleMovement>().maxSpeed = stateMachine.GetComponent<SimpleMovement>().originalMaxSpeed;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}

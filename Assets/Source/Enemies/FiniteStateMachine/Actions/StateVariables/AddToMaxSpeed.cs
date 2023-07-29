using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that adds to the max speed.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/State Variables/Add Max Speed")]
    public class AddToMaxSpeed : SingleAction
    {
        [Tooltip("The percent of the original max speed by which the move speed is changed.")]
        [SerializeField] private float amount = 0.1f;

        [Tooltip("The minimum allowed speed the state machine can reach.")]
        [SerializeField] private float minSpeed = 0;
        
        [Tooltip("Whether or not to account for delta time in the calculation.")]
        [SerializeField] private bool useDeltaTime = false;

        /// <summary>
        /// Adds to the max speed. 
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            SimpleMovement movement = stateMachine.GetComponent<SimpleMovement>();
            movement.maxSpeed = Mathf.Max(movement.maxSpeed + amount * movement.originalMaxSpeed * (useDeltaTime ? Time.deltaTime : 1), minSpeed);
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}

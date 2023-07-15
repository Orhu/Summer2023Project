using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets a given variable name to a given integer value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Set Tracked Variable to Int")]
    public class SetTrackedVariableToInt : SingleAction
    {
        [Tooltip("Tracked variable to set (creates it at set number if it doesn't exist)")]
        [SerializeField] private string trackedVariableName;
        
        [Tooltip("Number to set it to")]
        [SerializeField] private int numberToSetVarTo;

        /// <summary>
        /// Sets the requested name variable to the requested int.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd(trackedVariableName, numberToSetVarTo); 
            stateMachine.trackedVariables[trackedVariableName] = numberToSetVarTo;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
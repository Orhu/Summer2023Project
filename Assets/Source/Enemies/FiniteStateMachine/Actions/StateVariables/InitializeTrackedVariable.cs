using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that tries to initialize a state variable to the requested value. Does not overwrite if the value already exists.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Initialize Tracked Variable")]
    public class InitializeTrackedVariable : SingleAction
    {
        [Tooltip("State variable to initialize (does not override if variable already exists)")]
        [SerializeField] private string trackedVariableName;
        
        [Tooltip("Value to set variable to (does not override if variable already exists)")]
        [SerializeField] private int numberToInitializeVarTo;
        
        /// <summary>
        /// Sets a variable in the state machine with the given name and number default (does not overwrite if val already exists)
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd(trackedVariableName, numberToInitializeVarTo); 
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets a given variable name to a given integer value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Set Tracked Variable to Int")]
    public class SetTrackedVariableToInt : SingleAction
    {
        [Tooltip("Tracked variable to set")]
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
            stateMachine.trackedVariables[trackedVariableName] = numberToSetVarTo;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
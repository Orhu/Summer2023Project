using System;
using System.Collections;
using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that draws some number of cards and adds them to the state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Actions/Set Tracked Variable to Int")]
    public class COD_SetTrackedVariable : SingleAction
    {
        [Tooltip("Tracked variable to set")]
        [SerializeField] private string trackedVariableName;
        
        [Tooltip("Number to set it to")]
        [SerializeField] private int numberToSetVarTo;

        /// <summary>
        /// Sets the requested name variable to the requested int.
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables[trackedVariableName] = numberToSetVarTo;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
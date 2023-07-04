using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true if the given key exists and its value is equal to the given int value
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Decisions/State Variable Equals Num")]
    public class CompareStateVariableToInt : Decision
    {
        [Tooltip("State variable to compare")]
        [SerializeField] private string stateVariableName;
        
        [Tooltip("Transition returns true when the state variable is equal to this number")]
        [SerializeField] private int numberToCheck;
        
        /// <summary>
        /// Returns true if the given key exists and its value is equal to the given int value
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> true if the given key exists and its value is equal to the given int value, false otherwise </returns>
        protected override bool Evaluate(BaseStateMachine state)
        {
            bool stateVariableExists = state.trackedVariables.TryGetValue(stateVariableName, out var variableValue);
            return stateVariableExists && (int)variableValue == numberToCheck;
        }
    }
}
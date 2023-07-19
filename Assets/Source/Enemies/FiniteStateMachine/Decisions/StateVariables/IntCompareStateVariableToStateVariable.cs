using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true if both given keys exist and are equal as ints
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/Integer/Decisions/State Variable Comparison")]
    public class IntCompareStateVariableToStateVariable : Decision
    {
        [Tooltip("State variable to compare")]
        [SerializeField] private string stateVariable1Name;
        
        [Tooltip("State variable to compare")]
        [SerializeField] private string stateVariable2Name;

        [Tooltip("Minimum value both must be for comparison to return true")]
        [SerializeField] private int minValue = 0;
        
        /// <summary>
        /// Returns true if the given key exists and its value is equal to the given int value
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> true if the given key exists and its value is equal to the given int value, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            bool stateVariable1Exists = state.trackedVariables.TryGetValue(stateVariable1Name, out var variable1Value);
            bool stateVariable2Exists = state.trackedVariables.TryGetValue(stateVariable2Name, out var variable2Value);
            return stateVariable1Exists &&
                   stateVariable2Exists &&
                   (int)variable1Value >= minValue &&
                   (int)variable2Value >= minValue && 
                   (int)variable1Value == (int)variable2Value;
        }
    }
}
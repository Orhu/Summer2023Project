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
        
        [Tooltip("How should we compare the state variable")]
        [SerializeField] private ComparisonType comparison = ComparisonType.Equal;
        
        [Tooltip("Transition returns true when the state variable is compared to this number (3 and GreaterThan would check if stateVar > 3)")]
        [SerializeField] private int numberToCheck;

        /// <summary>
        /// Enum used for representing a comparison type
        /// </summary>
        public enum ComparisonType
        {
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            Equal
        }
        
        /// <summary>
        /// Returns true if the given key exists and its value is equal to the given int value
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> true if the given key exists and its value is equal to the given int value, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            bool stateVariableExists = state.trackedVariables.TryGetValue(stateVariableName, out var variableValue);

            switch (comparison)
            {
                case ComparisonType.GreaterThan:
                    return stateVariableExists && (int)variableValue > numberToCheck;
                case ComparisonType.GreaterThanOrEqual: 
                    return stateVariableExists && (int)variableValue >= numberToCheck;
                case ComparisonType.LessThan:
                    return stateVariableExists && (int)variableValue < numberToCheck;
                case ComparisonType.LessThanOrEqual:
                    return stateVariableExists && (int)variableValue <= numberToCheck;
                case ComparisonType.Equal:
                    return stateVariableExists && (int)variableValue == numberToCheck;
            }
            
            Debug.LogError("Provided with an invalid comparison type! Returning false.");
            return false;
            
        }
    }
}
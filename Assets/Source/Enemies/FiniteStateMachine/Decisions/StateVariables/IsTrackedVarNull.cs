using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true the tracked var is null.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Tracked Variables/GameObjects/Decisions/Is Null")]
    public class IsTrackedVarNull : Decision
    {
        [Tooltip("State variable to compare")]
        [SerializeField] private string variableName;

        /// <summary>
        /// Gets whether the variable doesn't exist or is null.
        /// </summary>
        /// <param name="state"> The state machine to use </param>
        /// <returns> True if variable doesn't exist or is null false otherwise. </returns>
        public override bool Decide(BaseStateMachine state)
        {
            return !state.trackedVariables.TryGetValue(variableName, out object value) || value == null;
        }
    }
}
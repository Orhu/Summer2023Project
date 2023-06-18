using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a decision that returns true if our current destination has been reached (following path completed)
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Destination Reached")]
    public class DestinationReached : FSMDecision
    {
        /// <summary>
        /// Returns true if destination is reached, false otherwise
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> true if destination is reached, false otherwise </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return invert ? !stateMachine.destinationReached : stateMachine.destinationReached;
        }
    }
}
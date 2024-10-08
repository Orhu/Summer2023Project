using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision that returns true if our current destination has been reached (following path completed)
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Pathfinding Target/Destination Reached")]
    public class DestinationReached : Decision
    {
        /// <summary>
        /// Returns true if the given state machine has reached its destination
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> true if the given state machine has reached its destination </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return stateMachine.destinationReached;
        }
    }
}
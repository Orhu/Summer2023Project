using UnityEngine;

namespace Cardificer.FiniteStateMachine.Dodge
{
    /// <summary>
    /// Represents a decision that returns whether or not dodge is currently requested
    /// </summary>
    [CreateAssetMenu (menuName = "FSM/Decisions/Dodge/Is Dodge Needed")]
    public class NeedToDodge : Decision
    {
        /// <summary>
        /// Returns whether the state wants to dodge
        /// </summary>
        /// <param name="state"> State to check </param>
        /// <returns> True if dodge is wanted, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            return state.needToDodge;
        }
    }
}
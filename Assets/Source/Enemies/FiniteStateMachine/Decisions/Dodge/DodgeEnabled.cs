using UnityEngine;

namespace Cardificer.FiniteStateMachine.Dodge
{
    /// <summary>
    /// Represents a decision that returns whether or not dodge is currently enabled (off cooldown)
    /// </summary>
    [CreateAssetMenu (menuName = "FSM/Decisions/Dodge/Is Dodge Enabled")]
    public class DodgeEnabled : Decision
    {
        /// <summary>
        /// Returns whether the state can dodge
        /// </summary>
        /// <param name="state"> State to check </param>
        /// <returns> True if dodge is enabled (off cooldown), false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            return state.canDodge;
        }
    }
}
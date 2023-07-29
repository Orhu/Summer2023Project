using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking whether our current velocity is below a certain amount
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Velocity Below Amount")]
    public class VelocityBelowAmount : Decision
    {
        [Tooltip("Velocity threshold to check against")]
        [SerializeField] private float velocityThreshold;

        /// <summary>
        /// Returns true if the velocity is below the provided threshold
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> True if the velocity is below the provided threshold, false otherwise </returns>
        public override bool Decide(BaseStateMachine stateMachine)
        {
            return stateMachine.GetComponent<Rigidbody2D>().velocity.magnitude <= velocityThreshold;
        }
    }
}
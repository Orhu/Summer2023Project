using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking if the player is in the forward direction
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Decisions/Player/Player in Forward Direction")]
    public class PlayerInForwardDirection : Decision
    {
        [Tooltip("Size of forward check (in degrees)")] [Range(0, 360)]
        [SerializeField] private float forwardCheckAngle;
        
        /// <summary>
        /// Evaluates whether the player's pos is within the given degrees of this stateMachine's forward movement direction
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is in the forward direction relative to the state machine, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            Vector2 enemyPos = state.transform.position;
            Vector2 enemyForward = enemyPos + state.GetComponent<Movement>().movementInput;
            Vector2 playerPos = Player.Get().transform.position;

            // Calculate the angle between the enemy's forward direction and the vector from enemy to player
            var angle = Vector2.SignedAngle(enemyForward - enemyPos, playerPos - enemyPos);

            // Check if the angle is within the forwardCheckAngle degrees cone of the forward direction
            return Mathf.Abs(angle) <= forwardCheckAngle / 2f;
        }
    }
}
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a decision checking if the player is in the forward direction
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Decisions/Player/Player in Forward Direction")]
    public class PlayerInForwardDirection : Decision
    {
        [Tooltip("Size of forward check (in degrees)")] [Range(0, 360)]
        [SerializeField] private float forwardCheckAngle;

        [Tooltip("Allow this check to view in all four cardinal directions?")]
        [SerializeField] private bool multiLOS;

        /// <summary>
        /// Evaluates whether the player's pos is within the given degrees of this stateMachine's forward movement direction
        /// </summary>
        /// <param name="state"> The stateMachine to use </param>
        /// <returns> True if the target is in the forward direction relative to the state machine, false otherwise </returns>
        public override bool Decide(BaseStateMachine state)
        {
            if (!multiLOS)
            {
                Vector2 enemyPos = state.transform.position;
                Vector2 enemyForward = enemyPos + state.GetComponent<Movement>().movementInput;
                Vector2 playerPos = Player.Get().transform.position;

                // Calculate the angle between the enemy's forward direction and the vector from enemy to player
                var angle = Vector2.SignedAngle(enemyForward - enemyPos, playerPos - enemyPos);

                // Check if the angle is within the forwardCheckAngle degrees cone of the forward direction
                return Mathf.Abs(angle) <= forwardCheckAngle / 2f;
            }
            else
            {
                Vector2 enemyPos = state.transform.position;
                Vector2 enemyUp = enemyPos + Vector2.up;
                Vector2 enemyRight = enemyPos + Vector2.right;
                Vector2 enemyLeft = enemyPos + Vector2.left;
                Vector2 enemyDown = enemyPos + Vector2.down;
                Vector2 playerPos = Player.Get().transform.position;

                // Calculate the angle between the enemy's direction and the vector from enemy to player
                var upAngle = Vector2.SignedAngle(enemyUp - enemyPos, playerPos - enemyPos);
                var rightAngle = Vector2.SignedAngle(enemyRight - enemyPos, playerPos - enemyPos);
                var leftAngle = Vector2.SignedAngle(enemyLeft - enemyPos, playerPos - enemyPos);
                var downAngle = Vector2.SignedAngle(enemyDown - enemyPos, playerPos - enemyPos);

                // Check if the angle is within the forwardCheckAngle degrees cone of the forward direction
                var upSeesPlayer = Mathf.Abs(upAngle) <= forwardCheckAngle / 2f;
                var rightSeesPlayer = Mathf.Abs(rightAngle) <= forwardCheckAngle / 2f;
                var leftSeesPlayer = Mathf.Abs(leftAngle) <= forwardCheckAngle / 2f;
                var downSeesPlayer = Mathf.Abs(downAngle) <= forwardCheckAngle / 2f;

                return upSeesPlayer || rightSeesPlayer || leftSeesPlayer || downSeesPlayer;
            }
        }
    }
}
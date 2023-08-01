using System.Collections;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the pathfinding target within a certain number of tiles form the player.
    /// </summary>
    [CreateAssetMenu(menuName= "FSM/Actions/Targeting/Set Target To Point Around Player")]
    public class SetTargetToPointAroundPlayer : SingleAction
    {
        [Tooltip("The maximum distance to the given tile")] [Min(0f)]
        [SerializeField] private float maxDistance = 100f;

        [Tooltip("The minimum distance to the given tile")] [Min(0f)]
        [SerializeField] private float minDistance = 0f;

        [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
        [SerializeField] private TargetType targetType;

        [Tooltip("Cooldown between target acquisitions")] [Min(0f)]
        [SerializeField] private float cooldown = 0f;

        /// <summary>
        /// Enum representing targeting modes for setting a target
        /// </summary>
        [System.Flags]
        enum TargetType
        {
            None = 0,
            Pathfinding = 1,
            Attack = 2,
        }

        /// <summary>
        /// Sets pathfinding and/or attack target depending on the requested targeting type
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Ends when the action is complete. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            // Get Random Tile
            Vector2 targetLocation = Player.GetFeetPosition() + Random.insideUnitCircle * Random.Range(minDistance, maxDistance);

            // Set Target
            if (targetType.HasFlag(TargetType.Pathfinding))
            {
                stateMachine.currentPathfindingTarget = targetLocation;
            }

            if (targetType.HasFlag(TargetType.Attack))
            {
                stateMachine.currentAttackTarget = targetLocation;
            }

            yield return new UnityEngine.WaitForSeconds(cooldown);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }
    }
}
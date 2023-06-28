using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that path finds away from the current target. Will override the pathfinding target.
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Flee from Attack Target")]
    public class FleeFromAttackTarget : ChaseTarget
    {
        [Tooltip("The maximum number of tiles to flee at a time. Larger number means its better at getting out of dead ends.")] [Range(1,5)]
        [SerializeField] private int fleeTiles = 1;

        /// <summary>
        /// Moves directly towards the pathfinding target.
        /// </summary>
        /// <param name="stateMachine"> StateMachine to be used </param>
        /// <returns> Updates move input every frame until duration has elapsed. </returns>
        protected sealed override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            PathfindingTile currentTile = RoomInterface.instance.WorldPosToTile(stateMachine.transform.position).Item1;
            List<PathfindingTile> possibleTargetTiles = new List<PathfindingTile>() { currentTile };
            IEnumerable<PathfindingTile> newNeighbors = new List<PathfindingTile>() { currentTile };
            for (int i = 0; i < fleeTiles; i++)
            {
                foreach (PathfindingTile newNeighbor in newNeighbors)
                {
                    newNeighbors = newNeighbors.Union(RoomInterface.instance.GetNeighbors(newNeighbor, stateMachine.currentMovementType));
                }
                newNeighbors.Except(possibleTargetTiles);
                possibleTargetTiles.AddRange(newNeighbors);
            }

            stateMachine.currentPathfindingTarget = RoomInterface.instance.TileToWorldPos(possibleTargetTiles[0]);
            foreach (PathfindingTile possibleTargetTile in possibleTargetTiles)
            {
                Vector2 tilePosition = RoomInterface.instance.TileToWorldPos(possibleTargetTile);
                if (FleeDistance(tilePosition, stateMachine) > FleeDistance(stateMachine.currentPathfindingTarget, stateMachine))
                {
                    stateMachine.currentPathfindingTarget = tilePosition;
                }
            }

            return base.PlayAction(stateMachine);
        }

        /// <summary>
        /// Gets the distance away from the attack target.
        /// </summary>
        /// <param name="position"> The position to get the distance of. </param>
        /// <param name="stateMachine"> The state machine who's target should be flead from. </param>
        /// <returns> The squared distance from the attack target. </returns>
        private float FleeDistance(Vector2 position, BaseStateMachine stateMachine)
        {
            return (position - stateMachine.currentAttackTarget).sqrMagnitude;
        }
    }
}
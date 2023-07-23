using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that sets the pathfinding target within a certain number of tiles form the player.
    /// </summary>
    [CreateAssetMenu(menuName= "FSM/Actions/Targeting/Set Target To Point Around Player")]
    public class SetTargetToPointAroundPlayer : SingleAction
    {
        [Tooltip("The maximum number of tiles to search at a time.")] [Range(1,5)]
        [SerializeField] private int tiles = 1;

        /// <summary>
        /// Sets the pathfinding target to the furthest tile within tiles of player.
        /// </summary>
        /// <param name="stateMachine"> StateMachine to be used </param>
        /// <returns> Updates move input every frame until duration has elapsed. </returns>
        protected sealed override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            PathfindingTile currentTile = RoomInterface.instance.WorldPosToTile(Player.GetFeetPosition()).Item1;
            List<PathfindingTile> possibleTargetTiles = new List<PathfindingTile>() { currentTile };
            IEnumerable<PathfindingTile> newNeighbors = new List<PathfindingTile>() { currentTile };
            for (int i = 0; i < tiles; i++)
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
                if (FleeDistance(tilePosition, stateMachine) > FleeDistance(stateMachine.currentPathfindingTarget, stateMachine) && possibleTargetTile.allowedMovementTypes.HasFlag(stateMachine.currentMovementType))
                {
                    stateMachine.currentPathfindingTarget = tilePosition;
                }
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;

            // Gets the squared distance away from the attack target.
            float FleeDistance(Vector2 position, BaseStateMachine stateMachine)
            {
                return (position - stateMachine.currentAttackTarget).sqrMagnitude;
            }
        }
    }
}
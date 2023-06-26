using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that allows the Cauldron of Desire to select its next spot to move and sets it as the pathfinding target
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cauldron of Desire/Target Next Move Spot")]
    public class COD_TargetNewMovementSpot : SingleAction
    {
        // Tracks seeded random from save manager
        private Random random;

        // Marked true when the locations have been cached. Should only be marked true once per boss fight
        private bool locationsCached;

        /// <summary>
        /// Grabs all
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("Random", new Random(SaveManager.savedFloorSeed));
            random = stateMachine.trackedVariables["Random"] as Random;

            if (!locationsCached)
            {
                CacheMovementPoints(stateMachine);
                locationsCached = true;
            }

            List<Vector2> movementOptions = new List<Vector2>(5)
            {
                (Vector2)stateMachine.trackedVariables["Center"],
                (Vector2)stateMachine.trackedVariables["TopLeftQuadrant"],
                (Vector2)stateMachine.trackedVariables["TopRightQuadrant"],
                (Vector2)stateMachine.trackedVariables["BottomLeftQuadrant"],
                (Vector2)stateMachine.trackedVariables["BottomRightQuadrant"]
            };

            FindClosestAndRemove(movementOptions, stateMachine);

            Vector2 randomMove = movementOptions[random.Next(movementOptions.Count)];
            stateMachine.currentPathfindingTarget = randomMove;

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }

        private void FindClosestAndRemove(List<Vector2> points, BaseStateMachine stateMachine)
        {
            if (points == null || points.Count == 0)
            {
                Debug.LogError("Cauldron of Desire tried to find next movement point but list provided empty or null!");
                return;
            }

            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            // Find the index of the closest Vector2 to the current position
            for (int i = 0; i < points.Count; i++)
            {
                float distance = Vector2.Distance(points[i], stateMachine.GetFeetPos());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            // Remove the found closest point
            if (closestIndex != -1)
            {
                points.RemoveAt(closestIndex);
            }
            else
            {
                Debug.LogWarning("Cauldron of Desire couldn't find closest movement point! Picking fully randomly.");
            }
        }

        private void CacheMovementPoints(BaseStateMachine stateMachine)
        {
            Vector2Int roomDimensions = RoomInterface.instance.myRoomSize;

            // Calculate center point
            Vector2 centerPoint = new Vector2(roomDimensions.x / 2f, roomDimensions.y / 2f);
            stateMachine.trackedVariables.Add("CenterPoint", centerPoint);

            // Calculate top left quadrant center
            Vector2 topLeftQuadrant = new Vector2(roomDimensions.x / 4f, roomDimensions.y / 4f);
            stateMachine.trackedVariables.Add("TopLeftQuadrant", topLeftQuadrant);
            
            // Calculate top right quadrant center
            Vector2 topRightQuadrant = new Vector2(3f * roomDimensions.x / 4f, roomDimensions.y / 4f);
            stateMachine.trackedVariables.Add("TopRightQuadrant", topRightQuadrant);
            
            // Calculate bottom left quadrant center
            Vector2 bottomLeftQuadrant = new Vector2(roomDimensions.x / 4f, 3f * roomDimensions.y / 4f);
            stateMachine.trackedVariables.Add("BottomLeftQuadrant", bottomLeftQuadrant);
            
            // Calculate bottom right quadrant center
            Vector2 bottomRightQuadrant = new Vector2(3f * roomDimensions.x / 4f, 3f * roomDimensions.y / 4f);
            stateMachine.trackedVariables.Add("BottomRightQuadrant", bottomRightQuadrant);
        }
    }
}
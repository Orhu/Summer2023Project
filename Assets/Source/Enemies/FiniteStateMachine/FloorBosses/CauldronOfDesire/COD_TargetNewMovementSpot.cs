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

        /// <summary>
        /// Grabs all possible locations to move to, removes the nearest point, and chooses a random one as the new pathfinding target.
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> Does not wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("Random", new Random(SaveManager.savedFloorSeed));
            random = stateMachine.trackedVariables["Random"] as Random;

            if (!stateMachine.trackedVariables.ContainsKey("CenterPoint"))
            {
                CacheMovementPoints(stateMachine);
            }

            List<Vector2> movementOptions = new List<Vector2>(5)
            {
                (Vector2)stateMachine.trackedVariables["CenterPoint"],
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

        /// <summary>
        /// Given a list of Vector2 points, determines the closest one to the given state machine's feet position and removes it from the list
        /// </summary>
        /// <param name="points"> List of Vector2 points </param>
        /// <param name="stateMachine"> The state machine to use </param>
        private void FindClosestAndRemove(List<Vector2> points, BaseStateMachine stateMachine)
        {
            if (points == null || points.Count == 0)
            {
                Debug.LogError("Cauldron of Desire tried to find next movement point but list provided is empty or null!");
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

        /// <summary>
        /// Calculates movement point Vector2s in the Cauldron of Desire's room, then stores them as state machine tracked variables
        /// </summary>
        /// <param name="stateMachine"> The state machine to use </param>
        private void CacheMovementPoints(BaseStateMachine stateMachine)
        {
            Vector2Int roomDimensions = RoomInterface.instance.myRoomSize;
            float quarterX = roomDimensions.x / 4f;
            float quarterY = roomDimensions.y / 4f;
            Vector2 roomCenterWorldPos = RoomInterface.instance.myWorldPosition;

            // Calculate center point
            Vector2 centerPoint = roomCenterWorldPos;
            stateMachine.trackedVariables.Add("CenterPoint", centerPoint);

            // Calculate top left quadrant center
            Vector2 topLeftQuadrant = new Vector2(roomCenterWorldPos.x - quarterX, roomCenterWorldPos.y + quarterY);
            stateMachine.trackedVariables.Add("TopLeftQuadrant", topLeftQuadrant);
            
            // Calculate top right quadrant center
            Vector2 topRightQuadrant = new Vector2(roomCenterWorldPos.x + quarterX, roomCenterWorldPos.y + quarterY);
            stateMachine.trackedVariables.Add("TopRightQuadrant", topRightQuadrant);
            
            // Calculate bottom left quadrant center
            Vector2 bottomLeftQuadrant = new Vector2(roomCenterWorldPos.x - quarterX , roomCenterWorldPos.y - quarterY);
            stateMachine.trackedVariables.Add("BottomLeftQuadrant", bottomLeftQuadrant);
            
            // Calculate bottom right quadrant center
            Vector2 bottomRightQuadrant = new Vector2(roomCenterWorldPos.x + quarterX, roomCenterWorldPos.y - quarterY);
            stateMachine.trackedVariables.Add("BottomRightQuadrant", bottomRightQuadrant);
        }
    }
}
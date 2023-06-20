using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that moves towards the current pathfinding target as specified by the stateMachine using this action.
    /// When leaving this state, enemies should stop the stateMachine.pathData.prevFollowCoroutine coroutine,
    /// as well as enabling stateMachine.pathData.ignorePathRequests
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Request and Follow Path")]
    public class ChaseTarget : SingleAction
    {
        [Tooltip("After a Path request is submitted, how long before another one is allowed?")]
        [SerializeField] private float pathLockout = 0.25f;
        
        // Default value found by testing units traveled every frame with movement speed of 4.
        // Essentially, this number allows us to avoid the "zigzagging" effect that happens when an enemy overshoots its
        // target (due to moving too fast in one frame) by providing a buffer for Vector2 position float comparison.
        // But, if the number is too high, the leniency will cause the enemy to not get close enough to their target pos before
        // turning, leading to unpredictable behavior.
        [Tooltip("When reaching a waypoint, how close do we have to be before we stop? (Vector2 position float comparison buffer)")]
        [SerializeField] private float distanceBuffer = 0.061f;
        
        // need to track our current data
        private ChaseData chaseData;

        /// <summary>
        /// Starts chase action
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Waits pathLockout seconds before allowing another request. </returns>
        protected sealed override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            RequestPath(stateMachine);
            yield return new WaitForSeconds(pathLockout);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }

        /// <summary>
        /// Updates our position and target position, then submits a path request
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        private void RequestPath(BaseStateMachine stateMachine)
        {
            PathRequestManager.RequestPath(stateMachine, OnPathFound);
        }

        /// <summary>
        /// Called when a path is successfully found
        /// </summary>
        /// <param name="newPath"> The new path </param>
        /// <param name="success"> Whether the path was successfully found or not </param>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        private void OnPathFound(Vector2[] newPath, bool success, BaseStateMachine stateMachine)
        {
            if (!success || stateMachine == null || stateMachine.pathData.ignorePathRequests) return;

            stateMachine.pathData.path = new Path(newPath, stateMachine.feetColliderPosition);

            if (stateMachine.pathData.prevFollowCoroutine != null)
            {
                stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
            }

            var newCoroutine = FollowPath(stateMachine);
            stateMachine.pathData.prevFollowCoroutine = newCoroutine;
            stateMachine.StartCoroutine(newCoroutine);
        }

        /// <summary>
        /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        /// <returns> Allows other code to execute in between iterations of the while (true) loop </returns>
        private IEnumerator FollowPath(BaseStateMachine stateMachine)
        {
            if (stateMachine.pathData.path.lookPoints.Length == 0)
            {
                yield break;
            }

            Vector2 currentWaypoint = stateMachine.pathData.path.lookPoints[0];
            stateMachine.currentWaypoint = currentWaypoint;

            while (true)
            {
                if (ArrivedAtPoint(currentWaypoint, stateMachine))
                {
                    stateMachine.pathData.targetIndex++;
                    if (stateMachine.pathData.targetIndex >= stateMachine.pathData.path.lookPoints.Length)
                    {
                        // reached the end of the waypoints, stop moving here
                        stateMachine.GetComponent<Movement>().movementInput = Vector2.zero;
                        yield break;
                    }

                    currentWaypoint = stateMachine.pathData.path.lookPoints[stateMachine.pathData.targetIndex];
                }

                stateMachine.GetComponent<Movement>().movementInput =
                    (currentWaypoint - (Vector2)stateMachine.transform.position).normalized;
                yield return null;
            }
        }

        /// <summary>
        /// Determines if we are "arrived" at a given point based on the serialized buffer variable
        /// </summary>
        /// <param name="point"> Point to check against </param>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        private bool ArrivedAtPoint(Vector2 point, BaseStateMachine stateMachine)
        {
            return Vector2.Distance(point, stateMachine.feetColliderPosition) <= distanceBuffer;
        }
    }
}
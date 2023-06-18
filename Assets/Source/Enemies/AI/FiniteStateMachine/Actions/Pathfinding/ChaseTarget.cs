using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that moves towards the current pathfinding target as specified by the stateMachine using this action.
    /// This action will continuously update its path so it should only be run once. 
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Pathfinding/Initialize Chase Target Loop")]
    public class ChaseTarget : FSMAction
    {
        [Tooltip("How often does this enemy update its path, in seconds?")]
        [SerializeField] private float delayBetweenPathUpdates = 0.25f;

        [Tooltip("How close do we need to be to our point before we are happy?")]
        [SerializeField] private float distanceBuffer = 0.061f;

        // need to track our current data
        private ChaseData chaseData;

        public override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.StartCoroutine(UpdatePath(stateMachine));
            yield break;
        }

        /// <summary>
        /// Sends a request for a path, and continuously submits requests every delayBetweenPathUpdates seconds
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        /// <returns></returns>
        IEnumerator UpdatePath(BaseStateMachine stateMachine)
        {
            RequestPath(stateMachine);

            while (true)
            {
                yield return new WaitForSeconds(delayBetweenPathUpdates);
                RequestPath(stateMachine);
            }
        }

        /// <summary>
        /// Updates our position and target position, then submits a path request
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        void RequestPath(BaseStateMachine stateMachine)
        {
            PathRequestManager.RequestPath(stateMachine, OnPathFound);
        }

        /// <summary>
        /// Called when a path is successfully found
        /// </summary>
        /// <param name="newPath"> The new path </param>
        /// <param name="success"> Whether the path was successfully found or not </param>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        public void OnPathFound(Vector2[] newPath, bool success, BaseStateMachine stateMachine)
        {
            if (!success || stateMachine == null || stateMachine.pathData.ignorePathRequests) return;

            stateMachine.pathData.path = new Path(newPath, stateMachine.feetCollider.transform.position);

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
        /// <returns></returns>
        IEnumerator FollowPath(BaseStateMachine stateMachine)
        {
            if (stateMachine.pathData.path.lookPoints.Length == 0)
            {
                yield break;
            }

            Vector2 currentWaypoint = stateMachine.pathData.path.lookPoints[0];

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
            return Vector2.Distance(point, stateMachine.feetCollider.transform.position) <= distanceBuffer;
        }
    }
}
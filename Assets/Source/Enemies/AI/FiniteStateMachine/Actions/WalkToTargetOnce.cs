using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents an action that moves towards the current target as specified by the stateMachine using this action
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Walk To Target Once")]
    public class WalkToTargetOnce : FSMAction
    {
        [Tooltip("How often does this enemy update its path, in seconds?")]
        [SerializeField] private float delayBetweenPathUpdates = 0.25f;

        [Tooltip("How close do we need to be to our point before we are happy?")]
        [SerializeField] private float distanceBuffer = 0.061f;

        // need to track our current data
        public struct ChaseData
        {
            private BaseStateMachine stateMachine;
            private Coroutine prevUpdateCoroutine;
            private Coroutine prevFollowCoroutine;
        }
        private ChaseData chaseData;

        /// <summary>
        /// Not needed for this action, but demanded due to FSMAction inheritance
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        public override void OnStateUpdate(BaseStateMachine stateMachine)
        {
            if (stateMachine.cooldownData.cooldownReady[this])
            {
                stateMachine.cooldownData.cooldownReady[this] = false;
                var coroutine = RequestPath(stateMachine);
                stateMachine.StartCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Enable incoming path callbacks, assign our state machine, and start pathfinding coroutine
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        public override void OnStateEnter(BaseStateMachine stateMachine)
        {
            stateMachine.pathData.ignorePathRequests = false;
            // sometimes, because transitions can occur every frame, rapid transitions cause the key not to be deleted properly and error. this check prevents that error
            if (!stateMachine.cooldownData.cooldownReady.ContainsKey(this))
            {
                stateMachine.cooldownData.cooldownReady.Add(this, true);
            }
            else
            {
                stateMachine.cooldownData.cooldownReady[this] = true;
            }
        }

        /// <summary>
        /// Disable any incoming path callbacks, and stop coroutines related to chasing
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        public override void OnStateExit(BaseStateMachine stateMachine)
        {
            stateMachine.pathData.ignorePathRequests = true;
        }

        /// <summary>
        /// Updates our position and target position, then submits a path request
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        IEnumerator RequestPath(BaseStateMachine stateMachine)
        {
            PathRequestManager.RequestPath(stateMachine, OnPathFound);
            yield return new WaitForSeconds(delayBetweenPathUpdates);
            stateMachine.cooldownData.cooldownReady[this] = true;
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
            stateMachine.destinationReached = false;
            if (stateMachine.pathData.path.lookPoints.Length == 0)
            {
                stateMachine.destinationReached = true;
                yield break;
            }

            Vector2 currentWaypoint = stateMachine.pathData.path.lookPoints[0];
            stateMachine.debugWaypoint = currentWaypoint;

            while (true)
            {
                if (ArrivedAtPoint(currentWaypoint, stateMachine))
                {
                    stateMachine.pathData.targetIndex++;
                    if (stateMachine.pathData.targetIndex >= stateMachine.pathData.path.lookPoints.Length)
                    {
                        // reached the end of the waypoints, stop moving here
                        stateMachine.GetComponent<Movement>().movementInput = Vector2.zero;
                        stateMachine.destinationReached = true;
                        yield break;
                    }

                    currentWaypoint = stateMachine.pathData.path.lookPoints[stateMachine.pathData.targetIndex];
                    stateMachine.debugWaypoint = currentWaypoint;
                }

                stateMachine.GetComponent<Movement>().movementInput = (currentWaypoint - (Vector2)stateMachine.transform.position).normalized;
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
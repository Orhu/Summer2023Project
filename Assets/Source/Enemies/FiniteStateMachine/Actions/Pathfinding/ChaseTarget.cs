using System.Collections;
using System.Runtime.InteropServices;
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
        [SerializeField] private float pathLockout = 0.03f;

        [Tooltip("Starting at stopping dist from the target destination, move speed rapidly drops until target destination is reached.")]
        [SerializeField] private float stoppingDist = 0.5f;
        
        // need to track our current data
        private ChaseData chaseData;

        /// <summary>
        /// Starts chase action
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Waits pathLockout seconds before allowing another request. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
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

            stateMachine.pathData.path = new Path(newPath, stateMachine.GetFeetPos(), stoppingDist);

            if (stateMachine.pathData.prevFollowCoroutine != null)
            {
                stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
            }

            var newCoroutine = FollowPath(stateMachine);
            stateMachine.pathData.prevFollowCoroutine = newCoroutine;
            stateMachine.pathData.targetIndex = 0;
            stateMachine.StartCoroutine(newCoroutine);
        }

        /// <summary>
        /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        /// <returns> Allows other code to execute in between iterations of the while (true) loop </returns>
        private IEnumerator FollowPath(BaseStateMachine stateMachine)
        {
            stateMachine.speedPercent = 1f; // reset speed percent to normal
            
            if (stateMachine.pathData.path.waypoints.Length == 0)
            {
                yield break;
            }

            while (stateMachine.pathData.keepFollowingPath)
            {
                while (stateMachine.pathData.path.turnBoundaries[stateMachine.pathData.targetIndex]
                       .HasCrossedLine(stateMachine.GetFeetPos()))
                {
                    if (stateMachine.pathData.targetIndex == stateMachine.pathData.path.finishLineIndex)
                    {
                        stateMachine.pathData.keepFollowingPath = false;
                        break;
                    }
                    else
                    {
                        stateMachine.pathData.targetIndex++;
                    }
                }

                if (stateMachine.pathData.keepFollowingPath)
                {
                    if (stateMachine.pathData.targetIndex >= stateMachine.pathData.path.slowDownIndex && stoppingDist > 0)
                    {
                        stateMachine.speedPercent = Mathf.Clamp01(stateMachine.pathData.path
                                                         .turnBoundaries[stateMachine.pathData.path.finishLineIndex]
                                                         .DistanceFromPoint(stateMachine.GetFeetPos()) /
                                                     stoppingDist);
                        if (stateMachine.speedPercent < 0.01f)
                        {
                            stateMachine.pathData.keepFollowingPath = false;
                        }
                    }
                    stateMachine.GetComponent<Movement>().movementInput =
                        (stateMachine.pathData.path.waypoints[stateMachine.pathData.targetIndex] - stateMachine.GetFeetPos()).normalized;
                }
                
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
            return Vector2.Distance(point, stateMachine.GetFeetPos()) <= stateMachine.distanceBuffer;
        }
    }
}
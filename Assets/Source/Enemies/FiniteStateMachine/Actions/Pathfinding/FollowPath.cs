using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that follows whatever path is currently stored by the state machine.
    /// When leaving this state, enemies should stop the stateMachine.pathData.prevFollowCoroutine coroutine,
    /// as well as enabling stateMachine.pathData.ignorePathRequests
    /// </summary>
    [CreateAssetMenu(menuName="FSM/Actions/Pathfinding/Follow Path (No Request)")]
    public class FollowPath : SingleAction
    {
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
            if (stateMachine.pathData.prevFollowCoroutine != null)
            {
                stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
            }

            var newCoroutine = TracePath(stateMachine);
            stateMachine.pathData.prevFollowCoroutine = newCoroutine;
            stateMachine.pathData.targetIndex = 0;
            stateMachine.pathData.keepFollowingPath = true;
            stateMachine.StartCoroutine(newCoroutine);
            yield break;
        }

        /// <summary>
        /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        /// <returns> Allows other code to execute in between iterations of the while (true) loop </returns>
        private IEnumerator TracePath(BaseStateMachine stateMachine)
        {
            stateMachine.speedPercent = 1f; // reset speed percent to normal
            stateMachine.currentPathfindingTarget = stateMachine.pathData.path.waypoints[^1];
            
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
                        stateMachine.GetComponent<Movement>().movementInput = Vector2.zero;
                        stateMachine.cooldownData.cooldownReady[this] = true;
                        yield break;
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
                            stateMachine.GetComponent<Movement>().movementInput = Vector2.zero;
                            stateMachine.cooldownData.cooldownReady[this] = true;
                            yield break;
                        }
                    }
                    stateMachine.GetComponent<Movement>().movementInput =
                        (stateMachine.pathData.path.waypoints[stateMachine.pathData.targetIndex] - stateMachine.GetFeetPos()).normalized;
                }
                
                yield return null;
            }
            stateMachine.cooldownData.cooldownReady[this] = true;
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
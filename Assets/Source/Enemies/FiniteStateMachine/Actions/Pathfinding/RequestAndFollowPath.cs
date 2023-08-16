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
    public class RequestAndFollowPath : SingleAction
    {
        [Tooltip("After a Path request is submitted, how long before another one is allowed?")] [SerializeField]
        private float pathLockout = 0.03f;

        [Tooltip("Starting at stopping dist from the target destination, move speed rapidly drops until target destination is reached.")]
        [SerializeField] private float stoppingDist = 0.1f;

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
            yield return new UnityEngine.WaitForSeconds(pathLockout);
            stateMachine.cooldownData.cooldownReady[this] = true;
        }

        /// <summary>
        /// Updates our position and target position, then submits a path request
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to be used. </param>
        private void RequestPath(BaseStateMachine stateMachine)
        {
            PathRequestManager.AsyncRequestPath(stateMachine, (Vector2[] path, bool successful) =>
            {
                if (!successful || stateMachine == null || stateMachine.pathData.ignorePathRequests) return;

                stateMachine.pathData.path = new Path(path, stateMachine.GetFeetPos(), 0);

                if (stateMachine.pathData.prevFollowCoroutine != null)
                {
                    stateMachine.StopCoroutine(stateMachine.pathData.prevFollowCoroutine);
                }

                var newCoroutine = TracePath(stateMachine);
                stateMachine.pathData.prevFollowCoroutine = newCoroutine;
                stateMachine.pathData.targetIndex = 0;
                stateMachine.pathData.keepFollowingPath = true;
                stateMachine.StartCoroutine(newCoroutine);
            });
        }

        /// <summary>
        /// Follows the path to the target, if we have one.
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
                        stateMachine.currentPathfindingTarget = stateMachine.GetFeetPos();
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
                            stateMachine.currentPathfindingTarget = stateMachine.GetFeetPos();
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
    }
}
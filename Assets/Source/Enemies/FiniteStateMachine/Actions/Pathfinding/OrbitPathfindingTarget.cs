using System.Collections;
using UnityEngine;
using ChaseData = Cardificer.FiniteStateMachine.BaseStateMachine.ChaseData;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// An action that causes this to walk in cicles around the player.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/Pathfinding/Orbit Pathfinding Target")]
    public class OrbitPathfindingTarget : SingleAction
    {
        [Tooltip("The direction this will orb in.")]
        [SerializeField] private OrbitDirection orbitDirection;
        private enum OrbitDirection { Clockwise, CounterClockwise, Random, }
        
        [Tooltip("The distance this will try to orbit at.")] [Min(0)]
        [SerializeField] private float orbitDistance = 3;
        
        [Tooltip("The distance this will try to orbit at.")] [Min(0)]
        [SerializeField] private float orbitWidth = 1;

        [Tooltip("The time this will orbit for.")] [Min(0)]
        [SerializeField] private float orbitTime = 1;

        /// <summary>
        /// Starts orbit action
        /// </summary>
        /// <param name="stateMachine"> stateMachine to be used </param>
        /// <returns> Doesn't wait </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            bool orbitClockwise = false;
            switch (orbitDirection)
            {
                case OrbitDirection.Clockwise:
                    orbitClockwise = true;
                    break;

                case OrbitDirection.CounterClockwise:
                    orbitClockwise = false;
                    break;

                case OrbitDirection.Random:
                    orbitClockwise = Random.value > 0.5;
                    break;

            }

            float timeElapsed = 0f;
            while (timeElapsed < orbitTime && stateMachine.pathData.keepFollowingPath)
            {
                Vector2 directionToTarget = (stateMachine.currentPathfindingTarget - stateMachine.GetFeetPos());
                float distanceToTarget = directionToTarget.magnitude;
                directionToTarget.Normalize();
                stateMachine.GetComponent<Movement>().movementInput = orbitClockwise ? new Vector2(-directionToTarget.y, directionToTarget.x) : new Vector2(directionToTarget.y, -directionToTarget.x);


                if (distanceToTarget < orbitDistance - orbitWidth / 2)
                {
                    stateMachine.GetComponent<Movement>().movementInput -= directionToTarget;
                    stateMachine.GetComponent<Movement>().movementInput.Normalize();
                }
                else if (distanceToTarget > orbitDistance + orbitWidth / 2)
                {
                    stateMachine.GetComponent<Movement>().movementInput += directionToTarget;
                    stateMachine.GetComponent<Movement>().movementInput.Normalize();
                }


                yield return null;
                timeElapsed += Time.deltaTime;
            }

            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
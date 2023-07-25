using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that draws some number of cards and adds them to the state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Set Pathfinding Target to Wall Side")]
    public class LostCreature_PathfindingTargetOffsetFromCenter : SingleAction
    {
        [Tooltip("Point to target, offset from room's center")] 
        [SerializeField] private Vector2 centerOffset;

        /// <summary>
        /// Sets pathfinding target to the wall side requested
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            Vector2 roomCenter = RoomInterface.instance.myWorldPosition;
            stateMachine.currentPathfindingTarget = roomCenter + centerOffset;
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }
    }
}
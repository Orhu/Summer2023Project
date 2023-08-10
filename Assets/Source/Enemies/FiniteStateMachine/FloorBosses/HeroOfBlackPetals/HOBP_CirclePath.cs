using System;
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
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Set Path To Figure8 Or Infinity")]
    public class HOBP_CirclePath : SingleAction
    {
        [Tooltip("Center point of the path shape, as an offset from the room's center")]
        [SerializeField] private Vector2 centerOffset;

        [Tooltip("Number of points to use in the shape")]
        [SerializeField] private int shapePoints = 8;

        [Tooltip("Radius size of the circle shape")]
        [SerializeField] private float radius = 5f;


        /// <summary>
        /// Formulates a path as requested and sets it to be the state machine's path.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Does not wait. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.pathData.path = new Path(FormulatePath(), stateMachine.GetFeetPos(), 0);
            stateMachine.cooldownData.cooldownReady[this] = true;
            yield break;
        }

        /// <summary>
        /// Generates a list of Vector2's needed to draw the path for the requested path type.
        /// </summary>
        /// <returns> List of Vector2 that make a circle path around the center position. </returns>
        private Vector2[] FormulatePath()
        {
            Vector2 roomCenter = RoomInterface.instance.myWorldPosition;
            List<Vector2> points = new List<Vector2>();

            for (int i = 0; i <= shapePoints; i++)
            {
                var x = ((float)i / shapePoints) * (2 * Mathf.PI);
                var vecX = radius * Mathf.Cos(x);
                var vecY = radius * Mathf.Sin(x);
                points.Add(roomCenter + centerOffset + new Vector2(vecX, vecY));
            }

            return points.ToArray();
        }
    }
}
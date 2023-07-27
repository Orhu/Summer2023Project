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
    public class LostCreature_SetPathTo : SingleAction
    {
        [Tooltip("Center point of the path shape, as an offset from the room's center")]
        [SerializeField] private Vector2 centerOffset;

        [Tooltip("Number of points to use in the shape")]
        [SerializeField] private int shapePoints = 8;

        [Tooltip("Bounding box size of the shape")]
        [SerializeField] private Vector2 shapeBounds;


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
        /// Assumes the figure 8 or infinity path will be within a 29x4 or 4x29 bounding box. Generates a list of Vector2's needed to draw the path for the requested path type.
        /// </summary>
        /// <returns> List of Vector2 that make either an infinity or figure 8 path around the center position. </returns>
        private Vector2[] FormulatePath()
        {
            float quarterHoriz = (float)shapeBounds.x / 4;
            float quarterVert = (float)shapeBounds.y / 4;
            Vector2 roomCenter = RoomInterface.instance.myWorldPosition;
            List<Vector2> points = new List<Vector2>();

            if (shapeBounds.x > shapeBounds.y)
            { // horizontal
                var constantX = shapeBounds.x * (2 / Mathf.PI);
                var constantY = shapeBounds.y * (4 / Mathf.PI);

                for (int i = 0; i <= shapePoints; i++)
                {
                    var x = ((float)i / shapePoints) * (2 * Mathf.PI);
                    BaseStateMachine.print("x: " + x);
                    var vecX = constantX * Mathf.Sin(x);
                    BaseStateMachine.print("Sin(x): " + vecX);
                    BaseStateMachine.print("Cos(x): " + Mathf.Cos(x));
                    var vecY = constantY * Mathf.Sin(x) * Mathf.Cos(x);
                    points.Add(roomCenter + centerOffset + new Vector2(vecX, vecY));
                }
            }
            else
            { // vertical
                var constantX = shapeBounds.x * (4 / Mathf.PI);
                var constantY = shapeBounds.y * (2 / Mathf.PI);

                for (int i = 0; i <= shapePoints; i++)
                {
                    var x = ((float)i / shapePoints) * (2 * Mathf.PI);
                    BaseStateMachine.print("x: " + x);
                    var vecX = constantX * Mathf.Sin(x) * Mathf.Cos(x);
                    BaseStateMachine.print("Sin(x): " + vecX);
                    BaseStateMachine.print("Cos(x): " + Mathf.Cos(x));
                    var vecY = constantY * Mathf.Sin(x);
                    points.Add(roomCenter + centerOffset + new Vector2(vecX, vecY));
                }
            }

            /*
                // Left Circle
                points.Add(roomCenter + new Vector2(centerOffset.x - quarterHoriz, centerOffset.y + quarterVert));
                points.Add(roomCenter + new Vector2(centerOffset.x - 2 * quarterHoriz, centerOffset.y));
                points.Add(roomCenter + new Vector2(centerOffset.x - quarterHoriz, centerOffset.y - quarterVert));

                // Right Circle
                points.Add(roomCenter + new Vector2(centerOffset.x + quarterHoriz, centerOffset.y + quarterVert));
                points.Add(roomCenter + new Vector2(centerOffset.x + 2 * quarterHoriz, centerOffset.y));
                points.Add(roomCenter + new Vector2(centerOffset.x + quarterHoriz, centerOffset.y - quarterVert));

                // Center
                points.Add(roomCenter + centerOffset);*/
            return points.ToArray();
        }
    }
}
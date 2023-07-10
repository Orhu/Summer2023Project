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
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Lost Creature/Set Path To...")]
    public class LostCreature_SetPathTo : SingleAction
    {
        private enum PathType
        {
            FigureEight,
            Infinity
        }

        [Tooltip("The path type to use")] [SerializeField]
        private PathType pathType;

        [Tooltip("Center point of the path shape, as an offset from the room's center")] [SerializeField]
        private Vector2 centerOffset;


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
            float quarter = 29f / 4f;
            Vector2 roomCenter = RoomInterface.instance.myWorldPosition;
            List<Vector2> points = new List<Vector2>();

            if (pathType == PathType.FigureEight)
            {
                points.Add(roomCenter + new Vector2(centerOffset.x - quarter, centerOffset.y + 2));
                points.Add(roomCenter + new Vector2(centerOffset.x - 2 * quarter, centerOffset.y));
                points.Add(roomCenter + new Vector2(centerOffset.x - quarter, centerOffset.y - 2));
                
                points.Add(roomCenter + centerOffset);
                
                points.Add(roomCenter + new Vector2(centerOffset.x + quarter, centerOffset.y + 2));
                points.Add(roomCenter + new Vector2(centerOffset.x + 2 * quarter, centerOffset.y));
                points.Add(roomCenter + new Vector2(centerOffset.x + quarter, centerOffset.y - 2));
                return points.ToArray();
            }
            else
            {
                points.Add(roomCenter + new Vector2(centerOffset.x + 2, centerOffset.y - quarter));
                points.Add(roomCenter + new Vector2(centerOffset.x, centerOffset.y - 2 * quarter));
                points.Add(roomCenter + new Vector2(centerOffset.x - 2, centerOffset.y - quarter));
                
                points.Add(roomCenter + centerOffset);
                
                points.Add(roomCenter + new Vector2(centerOffset.x + 2, centerOffset.y + quarter));
                points.Add(roomCenter + new Vector2(centerOffset.x, centerOffset.y + 2 * quarter));
                points.Add(roomCenter + new Vector2(centerOffset.x - 2, centerOffset.y + quarter));
                return points.ToArray();
            }
        }
    }
}
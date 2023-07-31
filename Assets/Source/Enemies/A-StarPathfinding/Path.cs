using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// This class provides a simple way to synthesize a path into one data structure
    /// </summary>
    public class Path
    {
        // Array of waypoints to travel down
        public readonly Vector2[] waypoints;

        // Array of turn boundaries
        public readonly Line[] turnBoundaries;

        // Final index of the turn boundaries array
        public readonly int finishLineIndex;

        // What index waypoint should slowdown start
        public readonly int slowDownIndex;

        /// <summary>
        /// Initialize a new instance of a Path, creating a Line object between each waypoint
        /// </summary>
        /// <param name="waypoints"> Array of waypoints to travel down </param>
        /// <param name="startPos"> Starting position </param>
        /// <param name="stoppingDist"> Distance from endpoint to begin slowing down and stopping </param>
        public Path(Vector2[] waypoints, Vector3 startPos, float stoppingDist)
        {
            this.waypoints = waypoints;
            turnBoundaries = new Line[this.waypoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector2 previousPoint = startPos;
            for (int i = 0; i < this.waypoints.Length; i++)
            {
                Vector2 currentPoint = this.waypoints[i];
                Vector2 turnBoundaryPoint = currentPoint;
                // Create a new line for the turn boundary
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint);
                previousPoint = turnBoundaryPoint;
            }

            float dstFromEndPoint = 0;
            for (int i = this.waypoints.Length - 1; i > 0; i--) {
                dstFromEndPoint += Vector2.Distance (this.waypoints [i], this.waypoints [i - 1]);
                if (dstFromEndPoint > stoppingDist) {
                    slowDownIndex = i;
                    break;
                }
            }
        }
    }
}
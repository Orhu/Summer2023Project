using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides a simple way to synthesize a path into one data structure
/// </summary>
public class Path
{
    // array of waypoints to travel down
    public readonly Vector2[] lookPoints;
    
    // array of turn boundaries
    public readonly Line[] turnBoundaries;
    
    // final index of the turn boundaries array
    public readonly int finishLineIndex;

    /// <summary>
    /// Initialize a new instance of a Path, creating a Line object between each waypoint
    /// </summary>
    /// <param name="waypoints"> Array of waypoints to travel down </param>
    /// <param name="startPos"> Starting position </param>
    public Path(Vector2[] waypoints, Vector3 startPos) 
    {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = startPos;
        for (int i = 0; i < lookPoints.Length; i++) 
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            // calculate the turn boundary point based on whether it's the finish line or not
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint;
            // create a new line for the turn boundary
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint);
            previousPoint = turnBoundaryPoint;
        }
    }
    
    /// <summary>
    /// Draw debug gizmos
    /// </summary>
    public void DrawWithGizmos() 
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints) {
            Gizmos.DrawCube (p + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line l in turnBoundaries) {
            l.DrawWithGizmos (10);
        }
    }
}

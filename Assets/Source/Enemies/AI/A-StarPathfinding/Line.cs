using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a line in 2D space
/// </summary>
public struct Line
{
    // constant gradient value for vertical lines to avoid division by zero
    const float verticalLineGradient = 1e5f;

    // gradient of the line
    float gradient;
    
    // y-intercept of the line
    float y_intercept;
    
    // first point on the line
    Vector2 pointOnLine_1;
    
    // second point on the line
    Vector2 pointOnLine_2;

    // perpendicular gradient
    float gradientPerpendicular;

    // indicates whether we are approaching the side
    bool approachSide;

    /// <summary>
    /// Initializes a new instance of the Line struct given a point on the line and a point perpendicular to the line.
    /// </summary>
    /// <param name="pointOnLine"> A point on the line. </param>
    /// <param name="pointPerpendicularToLine"> A point perpendicular to the line. </param>
    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        // calculate gradient of the perpendicular line
        if (dx == 0) {
            gradientPerpendicular = verticalLineGradient;
        } else {
            gradientPerpendicular = dy / dx;
        }

        // calculate gradient of the line
        if (gradientPerpendicular == 0) {
            gradient = verticalLineGradient;
        } else {
            gradient = -1 / gradientPerpendicular;
        }

        // calculate y-intercept of the line
        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        // define two points on the line
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2 (1, gradient);

        // determine if we are approaching the side
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    /// <summary>
    /// Determines whether the given point is on the same side as the approach side of the line.
    /// </summary>
    /// <param name="p"> The point to check. </param>
    bool GetSide(Vector2 p) {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    /// <summary>
    /// Checks if a given point has crossed the line.
    /// </summary>
    /// <param name="p"> The point to check. </param>
    /// <returns> True if the point has crossed the line, false otherwise. </returns>
    public bool HasCrossedLine(Vector2 p) {
        return GetSide (p) != approachSide;
    }

    /// <summary>
    /// Calculates the perpendicular distance from a point to the line.
    /// </summary>
    /// <param name="p"> The point to calculate the distance from. </param>
    /// <returns> The perpendicular distance from the point to the line. </returns>
    public float DistanceFromPoint(Vector2 p) {
        float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
        float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + y_intercept;
        return Vector2.Distance (p, new Vector2 (intersectX, intersectY));
    }
    
    /// <summary>
    /// Draw debug gizmos
    /// </summary>
    /// <param name="length">The length of the line to draw.</param>
    public void DrawWithGizmos(float length) {
        Vector3 lineDir = new Vector3 (1, 0, gradient).normalized;
        Vector3 lineCentre = new Vector3 (pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
    }
}

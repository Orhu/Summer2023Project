using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ContextSolver can be given steering behaviors, obstacles, and targets and determine which direction the AI should move
/// </summary>
public class ContextSolver : MonoBehaviour
{
    // show gizmos?
    [SerializeField]
    private bool showGizmos = true;

    // gizmo parameters
    Vector2 resultDirection = Vector2.zero;
    private float rayLength = 2;

    /// <summary>
    /// Calculates direction to move based on the current steering behaviors, obstacles, and targets
    /// </summary>
    /// <param name="behaviors">List of possible steering behaviors</param>
    /// <param name="aiData">AI data container holding information such as targets and obstacles</param>
    /// <returns>Direction to move</returns>
    public Vector2 GetDirectionToMove(List<SteeringBehavior> behaviors, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        // loop through each behaviour
        foreach (SteeringBehavior behavior in behaviors)
        {
            (danger, interest) = behavior.GetSteering(danger, interest, aiData);
        }

        // subtract danger values from interest array
        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        // get the average direction
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }

        outputDirection.Normalize();

        resultDirection = outputDirection;

        // return the selected movement direction
        return resultDirection;
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}
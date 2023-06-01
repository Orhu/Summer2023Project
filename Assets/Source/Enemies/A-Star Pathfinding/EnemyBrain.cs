using System.Collections;
using System.Collections.Generic;
using CardSystem;
using Skaillz.EditInline;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is designed to puppeteer a controller with enemy behaviors.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    [Header("Pathfinding")]
    
    [Tooltip("How far from a tile can an enemy be before they are considered \"arrived\"?")] 
    [SerializeField] private float buffer;
    
    [Tooltip("How close to the player does this enemy get before switching to attack state?")]
    [SerializeField] private float attackRange; 
    // TODO unimplemented

    [Tooltip("What do we do when we switch into the attack state?")]
    public UnityEvent launchAttack;

    [Header("Target Scanning")] 
    
    [Tooltip("Do we need line of sight to detect the target?")] 
    [SerializeField] private bool needsLineOfSight;
    // TODO unimplemented
    
    [Tooltip("The radius in which this enemy can detect the target")]
    [SerializeField] private float scanRadius;
    
    [Tooltip("How long of a delay before this enemy starts looking for targets?")]
    [SerializeField] private float scanDelay;
    
    [Tooltip("How often does this enemy scan for targets?")]
    [SerializeField] private float scanRate;
    
    [Tooltip("Layer containing target (probably the player layer)")]
    [SerializeField] private LayerMask targetLayer;

    [Header("Damage")] 
    
    [Tooltip("Does this enemy deal damage to the player when it is touched?")]
    [SerializeField] private bool dealsDamageOnTouch;
    
    [Tooltip("How much damage does this enemy deal when touched?")]
    [SerializeField] private float damageOnTouch;

    [Header("Debug")]

    [Tooltip("Draw debug gizmos?")]
    [SerializeField] private bool drawGizmos;
    
    // current target
    private Transform target;
    
    // path to target 
    private Vector2[] path;
    
    // index of where we are in the path
    private int targetIndex;

    // controller component for issuing commands
    private Controller controller;

    /// <summary>
    /// Requests path to target and initializes variables
    /// </summary>
    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        controller = GetComponent<Controller>();
       // InvokeRepeating(nameof(GetTargetPosition), scanDelay, scanRate);
    }

    /// <summary>
    /// Called when a path is successfully found
    /// </summary>
    /// <param name="newPath"> The new path </param>
    /// <param name="success"> Whether the path was successfully found or not </param>
    public void OnPathFound(Vector2[] newPath, bool success)
    {
        if (success)
        {
            path = newPath;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    /// <summary>
    /// Follows the path to the target, if we have one
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = path[0];

        while (true)
        {
            if ((Vector2)transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            controller.MoveTowards(currentWaypoint, buffer);
            yield return null;
        }
    }

    /// <summary>
    /// Gets a target position for this unit
    /// </summary>
    /// <returns> Vector2 representing the position of the target </returns>
    public Vector2 GetTargetPosition()
    {
        var targetFound = Physics2D.OverlapCircle(transform.position, scanRadius, targetLayer);
        return targetFound.transform.position;
    }

    /// <summary>
    /// Performs an attack
    /// </summary>
    void PerformAttack()
    {
        controller.PerformAttack();
    }

    /// <summary>
    /// Draw gizmos
    /// </summary>
    public void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f));
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawCube(path[i-1], path[i]);
                }
            }
        }
    }
}

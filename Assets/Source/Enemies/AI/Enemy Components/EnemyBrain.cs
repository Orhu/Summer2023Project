using System;
using System.Collections;
using System.Collections.Generic;
using CardSystem;
using Skaillz.EditInline;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// This class is designed to puppeteer a controller with enemy behaviors.
/// </summary>
public class EnemyBrain : MonoBehaviour
{
    [Header("Pathfinding")]
    [Tooltip("How far from a tile can an enemy be before they are considered \"arrived\"?")]
    [SerializeField]
    private float buffer;

    [Tooltip("How close to the player does this enemy get before switching to attack state?")] [SerializeField]
    private float attackRange;
    // TODO unimplemented

    [Header("Target Scanning")] [Tooltip("Do we need line of sight to detect the target?")] [SerializeField]
    private bool needsLineOfSight;
    // TODO unimplemented

    [Tooltip("The radius in which this enemy can detect the target")] [SerializeField]
    private float scanRadius;

    [Tooltip("How long of a delay before this enemy starts looking for targets?")] [SerializeField]
    private float scanInitialDelay;

    [Tooltip("How often does this enemy scan for targets, in seconds?")] [SerializeField]
    private float delayBetweenScans;

    [Tooltip("Layer containing target (probably the player layer)")] [SerializeField]
    private LayerMask targetLayer;

    [Header("Damage")] [Tooltip("Does this enemy deal damage to the player when it is touched?")] [SerializeField]
    private bool dealsDamageOnTouch;
    // TODO unimplemented

    [Tooltip("How much damage does this enemy deal when touched?")] [SerializeField]
    private float damageOnTouch;
    // TODO unimplemented

    [Header("Debug")] [Tooltip("Draw debug gizmos?")] [SerializeField]
    private bool drawGizmos;

    // current target
    private Collider2D target;

    // path to target 
    private Vector2[] path;

    // index of where we are in the path
    private int targetIndex;

    // controller component for issuing commands
    private Controller controller;
    
    // what room this enemy is in
    private Room room;

    /// <summary>
    /// Requests path to target and initializes variables
    /// </summary>
    void Start()
    {
        controller = GetComponent<Controller>();
        room = GetComponentInParent<Room>();
        InvokeRepeating(nameof(UpdatePath), scanInitialDelay, delayBetweenScans);
    }

    /// <summary>
    /// Re-scans for target and updates path accordingly
    /// </summary>
    void UpdatePath()
    {
        GetTargetPosition();
        if (target != null)
        {
            PathRequestManager.RequestPath(transform.position, target.transform.position, OnPathFound, room);
        }
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
            targetIndex = 0;
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
        if (path.Length == 0)
        {
             yield break;
        }
        
        Vector2 currentWaypoint = path[0];

        while (true)
        {
            if (ArrivedAtPoint(currentWaypoint))
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    // reached the end of the waypoints, stop moving here
                    controller.MovementInput = Vector2.zero;
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            controller.MoveTowards(currentWaypoint, buffer);
            yield return null;
        }
    }

    /// <summary>
    /// Uses buffer to determine if we are arrived at the given vector2
    /// </summary>
    /// <param name="waypoint"> Waypoint we are checking </param>
    /// <returns> True if we are within buffer (serialized) units of the given waypoint </returns>
    bool ArrivedAtPoint(Vector2 waypoint)
    {
        var myPos = transform.position;
        var withinX = Math.Abs(waypoint.x - myPos.x) < buffer;
        var withinY = Math.Abs(waypoint.y - myPos.y) < buffer;
        return withinX && withinY;
    }

    /// <summary>
    /// Gets a target position for this unit and updates its target accordingly
    /// </summary>
    /// <returns> This enemy's current target </returns>
    public Collider2D GetTargetPosition()
    {
        target = Physics2D.OverlapCircle(transform.position, scanRadius, targetLayer);
        return target;
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
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawCube(path[i - 1], path[i]);
                }
            }
        }
    }
}
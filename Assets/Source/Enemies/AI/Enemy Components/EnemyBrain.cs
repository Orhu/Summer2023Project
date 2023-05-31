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

    [Header("Debug")] [Tooltip("Draw debug gizmos?")] [SerializeField]
    private bool drawGizmos;

    // current target
    [HideInInspector] public Collider2D target;

    // path to target 
    private Vector2[] path;

    // index of where we are in the path
    private int targetIndex;

    // controller component for issuing commands
    private Controller controller;

    // what room this enemy is in
    private Room room;
    
    // what state is this enemy in
    enum State
    {
        Idle,
        Chase,
        Attack,
    }
    private State currentState;

    /// <summary>
    /// Initializes variables
    /// </summary>
    void Awake()
    {
        controller = GetComponent<Controller>();
        room = GetComponentInParent<Room>();
    }

    /// <summary>
    /// Begins the UpdatePath repeating invoke call
    /// </summary>
    void Start()
    {
        InvokeRepeating(nameof(GetTargetPosition), scanInitialDelay, delayBetweenScans);
    }

    /// <summary>
    /// Maintains the finite state machine and changes states/performs actions accordingly
    /// </summary>
    void Update()
    {
        var haveATarget = target != null;
        if (haveATarget)
        {
            // check if we are within attack range
            var withinAttackRange = Vector2.Distance(transform.position, target.transform.position) <= attackRange;
            currentState = withinAttackRange ? State.Attack : State.Chase;
        }
        else
        {
            // no target scanned, go back to idle
            currentState = State.Idle;
        }

        // state behaviors
        switch (currentState)
        {
            case State.Attack:
                controller.movementInput = Vector2.zero;
                controller.PerformAttack();
                break;
            case State.Chase:
                if (haveATarget)
                {
                    UpdatePath();
                }
                break;
            case State.Idle:
                break;
        }
    }

    /// <summary>
    /// Re-scans for target and updates path accordingly
    /// </summary>
    void UpdatePath()
    {
        if (currentState == State.Chase)
        {
            PathRequestManager.RequestPath(controller.feet.transform.position, target.transform.position, OnPathFound,
                room);
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
    /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        if (path.Length == 0 || currentState != State.Chase)
        {
            // if we have no path, or we are not in the chase state, do not follow path
            controller.movementInput = Vector2.zero;
            yield break;
        }

        Vector2 currentWaypoint = path[0];

        while (true)
        {
            // check if we need to advance to the next checkpoint
            if (ArrivedAtPoint(currentWaypoint))
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    // reached the end of the waypoints, stop moving here
                    controller.movementInput = Vector2.zero;
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            // if we didn't hit a break, then just keep moving to the current waypoint
            controller.MoveTowards(currentWaypoint, buffer);
            yield return null;
        }
    }

    /// <summary>
    /// Uses buffer to determine if we are arrived at the given vector2
    /// </summary>
    /// <param name="waypoint"> Waypoint we are checking </param>
    /// <returns> True if we are within buffer (serialized) units of the given waypoint </returns>
    public bool ArrivedAtPoint(Vector2 waypoint)
    {
        var myPos = controller.feet.transform.position;
        var withinX = Math.Abs(waypoint.x - myPos.x) < buffer;
        var withinY = Math.Abs(waypoint.y - myPos.y) < buffer;
        return withinX && withinY;
    }

    /// <summary>
    /// Gets a target position for this unit and updates its target accordingly
    /// </summary>
    /// <returns> True if we found a target, false if none found </returns>
    public void GetTargetPosition()
    {
        target = Physics2D.OverlapCircle(transform.position, scanRadius, targetLayer);
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
        
        Gizmos.color = Color.red;
        if (target != null)
        {
            // draw current target tile
            var targetTile = room.WorldPosToTile(target.transform.position);
            Gizmos.DrawCube(room.TileToWorldPos(targetTile), Vector3.one);
        }
        
        Gizmos.color = Color.cyan;
        var myTile = room.WorldPosToTile(controller.feet.transform.position);
        Gizmos.DrawCube(room.TileToWorldPos(myTile), Vector3.one);

        Gizmos.color = Color.green;
        if (path != null && path.Length > 0)
        {
            if (targetIndex < path.Length)
            {
                // reached the end of the waypoints
                Gizmos.DrawLine(controller.feet.transform.position, path[targetIndex]);
                Gizmos.DrawCube(path[targetIndex], Vector3.one * 0.5f);
            }
        }
    }
}
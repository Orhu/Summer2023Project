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
    [Tooltip("How close to the player does this enemy get before switching to attack state?")] [SerializeField]
    private float attackRange;
    // TODO unimplemented
    
    [Tooltip("How much does the player need to move before we update path?")] [SerializeField]
    private float pathUpdateMoveThreshold = .5f;

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
    
    [Header("Debug")]
    
    [Tooltip("Draw debug gizmos?")]
    [SerializeField] private bool drawGizmos;

    // current target
    [HideInInspector] public Collider2D target;

    // path to target 
    private Path path;

    // index of where we are in the path
    private int targetIndex;

    // controller component for issuing commands
    private Controller controller;

    // what room this enemy is in
    private Room room;

    public float turnDst = 5;
    public float stoppingDst = 10;

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
        room = transform.parent.GetComponentInParent<Room>();
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
                    StartCoroutine(nameof(UpdatePath));
                }
                break;
            case State.Idle:
                break;
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
            path = new Path(newPath, controller.feet.transform.position, turnDst, stoppingDst);
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    /// <summary>
    /// Re-scans for target and updates path accordingly
    /// </summary>
    IEnumerator UpdatePath()
    {
        var feetPos = controller.feet.transform.position;
        var targetPos = target.transform.position;
        
        PathRequestManager.RequestPath(feetPos, targetPos, OnPathFound,
                room);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector2 targetPosOld = targetPos;

        while (true)
        {
            yield return new WaitForSeconds(delayBetweenScans);
            if (currentState != State.Chase)
            {
                yield break;
            }

            if (((Vector2)targetPos - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(feetPos, targetPos, OnPathFound, room);
                targetPosOld = targetPos;
            }
        }
    }

    /// <summary>
    /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        var feetPos = controller.feet.transform.position;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(feetPos.x, feetPos.y);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                controller.MoveTowards(path.lookPoints[pathIndex]);
            }

            yield return null;
        }
    }


    /// <summary>
    /// Gets a target position for this unit and updates its target accordingly
    /// </summary>
    /// <returns> True if we found a target, false if none found </returns>
    public void GetTargetPosition()
    {
        target = Physics2D.OverlapCircle(transform.position, scanRadius, targetLayer);
    }

    public void OnDrawGizmos()
    {
        if (drawGizmos && path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
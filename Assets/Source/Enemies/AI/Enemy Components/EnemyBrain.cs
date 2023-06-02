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

    [Header("Target Scanning")] [Tooltip("Do we need line of sight to detect the target?")] [SerializeField]
    private bool needsLineOfSight;
    // TODO unimplemented

    [Tooltip("The radius in which this enemy can detect the target")] [SerializeField]
    private float scanRadius;

    [Tooltip("How long of a delay before this enemy starts looking for targets?")] [SerializeField]
    private float scanInitialDelay;

    [Tooltip("How often does this enemy scan for targets and update its path, in seconds?")] [SerializeField]
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

    // what state is this enemy in
    enum State
    {
        Idle,
        Chase,
        Attack,
    }
    private State currentState;
    
    // tracks the position of the feet by caching it every frame, rather than grabbing it in multiple places every frame
    private Vector2 feetPos;

    // tracks the position of the target by caching it every scan, rather than grabbing it from the target collider manually in multiple places
    private Vector2 targetPos;

    // tracks the results of the most recent target scan
    private bool prevTargetScanFoundTarget = false;

    /// <summary>
    /// Initializes variables
    /// </summary>
    void Awake()
    {
        controller = GetComponent<Controller>();
    }

    /// <summary>
    /// Begins the UpdatePath repeating invoke call
    /// </summary>
    void Start()
    {
        feetPos = controller.feet.transform.position;
        InvokeRepeating(nameof(GetTargetPosition), scanInitialDelay, delayBetweenScans);
    }

    /// <summary>
    /// Maintains the finite state machine and changes states/performs actions accordingly
    /// </summary>
    void Update()
    {
        feetPos = controller.feet.transform.position;
        
        // conditions for current state
        if (prevTargetScanFoundTarget)
        {
            // check if we are within attack range
            var withinAttackRange = Vector2.Distance(transform.position, targetPos) <= attackRange;
            SwitchState(withinAttackRange ? State.Attack : State.Chase);
        }
        else
        {
            // no target scanned, go back to idle
            SwitchState(State.Idle);
        }

        // state behaviors
        switch (currentState)
        {
            case State.Attack:
                AttackState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Idle:
                IdleState();
                break;
        }
    }

    void AttackState()
    {
        controller.PerformAttack();
    }
    
    void ChaseState()
    {
        
    }

    void IdleState()
    {
        
    }

    /// <summary>
    /// Performs any one-off actions needed for a state switch
    /// </summary>
    /// <param name="newState"> State to switch to </param>
    void SwitchState(State newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        switch (newState)
        {
            case State.Attack:
                controller.movementInput = Vector2.zero;
                StopCoroutine(nameof(UpdatePath));
                StopCoroutine(nameof(FollowPath));
                print(name + ": Attack State Switch");
                break;
            case State.Chase:
                StartCoroutine(nameof(UpdatePath));
                print(name + ": Chase State Switch");
                break;
            case State.Idle:
                print(name + ": Idle State Switch");
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
        if (success && currentState == State.Chase)
        {
            path = new Path(newPath, controller.feet.transform.position);
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    /// <summary>
    /// Re-scans for target and updates path accordingly
    /// </summary>
    IEnumerator UpdatePath()
    {
        PathRequestManager.RequestPath(feetPos, targetPos, OnPathFound);

        while (true)
        {
            yield return new WaitForSeconds(delayBetweenScans);

                PathRequestManager.RequestPath(feetPos, targetPos, OnPathFound);
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
        targetPos = target.transform.position;
        prevTargetScanFoundTarget = target != null;
    }

    public void OnDrawGizmos()
    {
        if (drawGizmos && path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
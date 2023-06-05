using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Represents an action that moves towards the current target as specified by the stateMachine using this action
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Chase Target")]
public class ChaseTarget : FSMAction
{
    [Tooltip("How often does this enemy update its path, in seconds?")]
    [SerializeField] private float delayBetweenPathUpdates;

    [Tooltip("How close do we need to be to our point before we are happy?")]
    [SerializeField] private float distanceBuffer;

    // path to target 
    private Path path;

    // index of where we are in the path
    private int targetIndex;

    // save our state machine as a variable
    private BaseStateMachine myStateMachine;

    // save our feet position
    private Vector2 feetPos;

    // save our target position
    private Vector2 targetPos;

    // boolean to switch when we don't want to receive any more path requests (eg the state has switched)
    private bool ignorePathRequests;

    // track the previous coroutine so we can stop it later
    private Coroutine prevCoroutine;

    /// <summary>
    /// Not needed for this action, but demanded due to FSMAction inheritance
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to be used. </param>
    public override void OnStateUpdate(BaseStateMachine stateMachine)
    {
    }

    /// <summary>
    /// Enable incoming path callbacks, assign our state machine, and start pathfinding coroutine
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to be used. </param>
    public override void OnStateEnter(BaseStateMachine stateMachine)
    {
        ignorePathRequests = false;
        myStateMachine = stateMachine;
        var coroutine = UpdatePath();
        myStateMachine.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Disable any incoming path callbacks, and stop coroutines related to chasing
    /// </summary>
    /// <param name="stateMachine"> The stateMachine to be used. </param>
    public override void OnStateExit(BaseStateMachine stateMachine)
    {
        ignorePathRequests = true;
        myStateMachine.StopCoroutine(nameof(UpdatePath));
        myStateMachine.StopCoroutine(nameof(FollowPath));
    }

    /// <summary>
    /// Sends a request for a path, and continuously submits requests every delayBetweenPathUpdates seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdatePath()
    {
        UpdatePositionsAndRequestPath();

        while (true)
        {
            yield return new WaitForSeconds(delayBetweenPathUpdates);
            UpdatePositionsAndRequestPath();
        }
    }

    /// <summary>
    /// Updates our position and target position, then submits a path request
    /// </summary>
    void UpdatePositionsAndRequestPath()
    {
        targetPos = myStateMachine.currentTarget.transform.position;
        feetPos = myStateMachine.feetCollider.transform.position;
        PathRequestManager.RequestPath(feetPos, targetPos, OnPathFound);
    }

    /// <summary>
    /// Called when a path is successfully found
    /// </summary>
    /// <param name="newPath"> The new path </param>
    /// <param name="success"> Whether the path was successfully found or not </param>
    public void OnPathFound(Vector2[] newPath, bool success)
    {
        if (ignorePathRequests || !success) return;

        path = new Path(newPath, feetPos);
        
        if (myStateMachine != null && prevCoroutine != null)
        {
            myStateMachine.StopCoroutine(prevCoroutine);
        }

        var newCoroutine = FollowPath();
        prevCoroutine = myStateMachine.StartCoroutine(newCoroutine);
    }

    /// <summary>
    /// Follows the path to the target, if we have one. If we reach attackRange of our target, then stop and attack
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        if (path.lookPoints.Length == 0)
        {
            yield break;
        }

        Vector2 currentWaypoint = path.lookPoints[0];

        while (true)
        {
            if (ArrivedAtPoint(currentWaypoint))
            {
                targetIndex++;
                if (targetIndex >= path.lookPoints.Length)
                {
                    // reached the end of the waypoints, stop moving here
                    myStateMachine.GetComponent<Controller>().movementInput = Vector2.zero;
                    yield break;
                }

                currentWaypoint = path.lookPoints[targetIndex];
            }

            myStateMachine.GetComponent<Controller>().MoveTowards(currentWaypoint);
            yield return null;
        }
    }

    /// <summary>
    /// Determines if we are "arrived" at a given point based on the serialized buffer variable
    /// </summary>
    /// <param name="point"> Point to check against </param>
    private bool ArrivedAtPoint(Vector2 point)
    {
        return Vector2.Distance(point, myStateMachine.feetCollider.transform.position) <= distanceBuffer;
    }
}
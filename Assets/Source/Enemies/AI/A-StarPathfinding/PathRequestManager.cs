using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// This class is a singleton that manages the pathfinding requests such that every unit is not requesting a path on the same frame at the same time, causing lag
/// </summary>
public class PathRequestManager : MonoBehaviour
{
    /// <summary>
    /// Represents a pathfinding request
    /// </summary>
    struct PathRequest
    {
        // callback action
        public Action<Vector2[], bool, BaseStateMachine> callback;
        
        // store the state machine
        public BaseStateMachine stateMachine;

        /// <summary>
        /// Constructor for a pathfinding request
        /// </summary>
        /// <param name="myStateMachine"> The state machine that is requesting a path </param>
        /// <param name="myCallback"> What function to call when path calculation is complete </param>
        public PathRequest(BaseStateMachine myStateMachine, Action<Vector2[], bool, BaseStateMachine> myCallback)
        {
            callback = myCallback;
            stateMachine = myStateMachine;
        }
    }

    // The queue of requests
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

    // The current working request
    private PathRequest currentPathRequest;

    // This instance of the path request manager
    private static PathRequestManager instance;

    // The pathfinding component
    private Pathfinding pathfinding;

    // Indicates if we are currently processing a path
    private bool isProcessingPath;

    /// <summary>
    /// Intiializes variables
    /// </summary>
    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    /// <summary>
    /// Request a path from start to end
    /// </summary>
    /// <param name="stateMachine"> The state machine that is requesting a path </param>
    /// <param name="callback"> Action that will receive the found path and a boolean saying if the path was found </param>
    public static void RequestPath(BaseStateMachine stateMachine, Action<Vector2[], bool, BaseStateMachine> callback)
    {
        PathRequest newRequest = new PathRequest(stateMachine, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    /// <summary>
    /// Attempt to process the next request in the queue, if there is one
    /// </summary>
    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.stateMachine);
        }
    }

    /// <summary>
    /// Called when a path is finished processing
    /// </summary>
    /// <param name="path"> The new path </param>
    /// <param name="success"> Whether a path was successfully found to the target </param>
    /// <param name="stateMachine"> The stateMachine to be used. </param>
    public void FinishedProcessingPath(Vector2[] path, bool success, BaseStateMachine stateMachine)
    {
        if (currentPathRequest.callback.Target != null)
        {
            currentPathRequest.callback(path, success, stateMachine);
        }

        isProcessingPath = false;
        TryProcessNext();
    }
}
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
        // start position
        public Vector2 startPos;
        
        // end position
        public Vector2 endPos;
        
        // callback action
        public Action<Vector2[], bool> callback;

        // the room the enemy is a part of
        public Room room;

        /// <summary>
        /// Constructor for a pathfinding request
        /// </summary>
        /// <param name="myStart"> Starting position </param>
        /// <param name="myEnd"> Target position </param>
        /// <param name="myCallback"> What function to call when path calculation is complete </param>
        public PathRequest(Vector2 myStart, Vector2 myEnd, Action<Vector2[], bool> myCallback, Room myRoom)
        {
            startPos = myStart;
            endPos = myEnd;
            callback = myCallback;
            room = myRoom;
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
    /// <param name="pathStart"> Start location </param>
    /// <param name="pathEnd"> End location </param>
    /// <param name="callback"> Action that will receive the found path and a boolean saying if the path was found </param>
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback, Room myRoom)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, myRoom);
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
            pathfinding.StartFindPath(currentPathRequest.startPos, currentPathRequest.endPos, currentPathRequest.room);
        }
    }

    /// <summary>
    /// Called when a path is finished processing
    /// </summary>
    /// <param name="path"> The new path </param>
    /// <param name="success"> Whether a path was successfully found to the target </param>
    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}

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
        public Vector2 startPos;
        public Vector2 endPos;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 myStart, Vector2 myEnd, Action<Vector2[], bool> myCallback)
        {
            startPos = myStart;
            endPos = myEnd;
            callback = myCallback;
        }
    }

    // The queue of requests
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    
    // The current working request
    private PathRequest currentPathRequest;

    // This instance of the path request manager
    private static PathRequestManager instance;

    private Pathfinding pathfinding;

    private bool isProcessingPath;

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
    public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.startPos, currentPathRequest.endPos);
        }
    }

    public void FinishedProcessingPath(Vector2[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}

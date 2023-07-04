using System.Collections.Generic;
using System;
using Cardificer.FiniteStateMachine;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer
{
    /// <summary>
    /// This class is a singleton that manages the pathfinding requests such that every unit is not requesting a path on the same frame at the same time, causing lag
    /// </summary>
    public class PathRequestManager : MonoBehaviour
    {
        /// <summary>
        /// Represents a pathfinding request
        /// </summary>
        public struct PathRequest
        {
            // callback action
            public Action<Vector2[], bool> callback;
            
            // store start position
            public Vector2 startPos;
            
            // store end position
            public Vector2 endPos;
            
            // store movement type of this request
            public MovementType movementType;

            /// <summary>
            /// Constructor for a pathfinding request
            /// </summary>
            /// <param name="stateMachine"> The state machine that is requesting a path </param>
            /// <param name="callback"> What function to call when path calculation is complete </param>
            public PathRequest(BaseStateMachine stateMachine, Action<Vector2[], bool> callback = null)
            {
                this.callback = callback;
                startPos = stateMachine.GetFeetPos();
                endPos = stateMachine.currentPathfindingTarget;
                movementType = stateMachine.currentMovementType;
            }

            /// <summary>
            /// Constructor for pathfinding request
            /// </summary>
            /// <param name="startPos"> Starting position to path from </param>
            /// <param name="endPos"> End position to path to </param>
            /// <param name="movementType"> Movement type of the path request </param>
            /// <param name="callback"> What function to call when path calculation is complete </param>
            public PathRequest(Vector2 startPos, Vector2 endPos, MovementType movementType, Action<Vector2[], bool> callback = null)
            {
                this.callback = callback;
                this.startPos = startPos;
                this.endPos = endPos;
                this.movementType = movementType;
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
        /// Request a path from start to end asynchronously (more performant)
        /// </summary>
        /// <param name="stateMachine"> The state machine that is requesting a path </param>
        /// <param name="callback"> Action that will receive the found path and a boolean saying if the path was found </param>
        public static void AsyncRequestPath(BaseStateMachine stateMachine, Action<Vector2[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(stateMachine, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        /// <summary>
        /// Request a path from start to end asynchronously (more performant)
        /// </summary>
        /// <param name="startPos"> Starting position </param>
        /// <param name="endPos"> Ending position </param>
        /// <param name="movementType"> Movement type of the path requested </param>
        /// <param name="callback"> Action that will receive the found path and a boolean saying if the path was found </param>
        public static void AsyncRequestPath(Vector2 startPos, Vector2 endPos, MovementType movementType,
            Action<Vector2[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(startPos, endPos, movementType, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        /// <summary>
        /// Request a path from start to end synchronously (less performant, but instant)
        /// </summary>
        /// <param name="stateMachine"> The state machine that is requesting a path </param>
        /// <param name="path"> The found path </param>
        /// <returns> True if pathfinding found a path, false otherwise </returns>
        public static bool SyncRequestPath(BaseStateMachine stateMachine, out Vector2[] path)
        {
            PathRequest newRequest = new PathRequest(stateMachine);
            var pathResult = Pathfinding.instance.FindPathSync(newRequest);
            path = pathResult.Item1;
            return pathResult.Item2;
        }
        
        /// <summary>
        /// Request a path from start to end synchronously (less performant, but instant)
        /// </summary>
        /// <param name="startPos"> Starting position </param>
        /// <param name="endPos"> Ending position </param>
        /// <param name="movementType"> Movement type of the path requested </param>
        /// <param name="path"> The found path </param>
        /// <returns> True if pathfinding found a path, false otherwise </returns>
        public static bool SyncRequestPath(Vector2 startPos, Vector2 endPos, MovementType movementType, out Vector2[] path)
        {
            PathRequest newRequest = new PathRequest(startPos, endPos, movementType);
            var pathResult = Pathfinding.instance.FindPathSync(newRequest);
            path = pathResult.Item1;
            return pathResult.Item2;
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
                pathfinding.StartFindPath(currentPathRequest);
            }
        }

        /// <summary>
        /// Called when a path is finished processing
        /// </summary>
        /// <param name="path"> The new path </param>
        /// <param name="success"> Whether a path was successfully found to the target </param>
        public void FinishedProcessingPath(Vector2[] path, bool success)
        {
            if (currentPathRequest.callback.Target != null)
            {
                currentPathRequest.callback(path, success);
            }

            isProcessingPath = false;
            TryProcessNext();
        }
    }
}
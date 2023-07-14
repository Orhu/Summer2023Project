using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathRequest = Cardificer.PathRequestManager.PathRequest;

namespace Cardificer
{
    /// <summary>
    /// Handles A* pathfinding calculations
    /// </summary>
    public class Pathfinding : MonoBehaviour
    {
        // The request manager component that sends us requests
        private PathRequestManager requestManager;

        // The room interface that allows us to mess with room properties without having to mess with the room itself
        private RoomInterface roomInterface;
        
        // The Pathfinding singleton
        public static Pathfinding instance;

        /// <summary>
        /// Initialize variables
        /// </summary>
        void Awake()
        {
            requestManager = GetComponent<PathRequestManager>();
            roomInterface = GetComponent<RoomInterface>();
            instance = this;
        }

        /// <summary>
        /// Starts the AsyncFindPath coroutine from start to target pos
        /// </summary>
        /// <param name="request"> The path request data </param>
        public void StartFindPath(PathRequest request)
        {
            StartCoroutine(AsyncFindPath(request));
        }

        /// <summary>
        /// Find a path from a given pos to a target pos
        /// </summary>
        /// <param name="request"> The path request data </param>
        /// <returns> Sends a signal to the request manager that a path has been found </returns>
        private IEnumerator AsyncFindPath(PathRequest request)
        {
            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;

            (PathfindingTile, bool) startNodeResult = roomInterface.WorldPosToTile(request.startPos);
            (PathfindingTile, bool) targetNodeResult = roomInterface.WorldPosToTile(request.endPos);

            PathfindingTile startNode;
            PathfindingTile targetNode;

            if (startNodeResult.Item2 && targetNodeResult.Item2)
            {
                startNode = startNodeResult.Item1;
                targetNode = targetNodeResult.Item1;
                startNode.retraceStep = startNode;
            }
            else
            {
                requestManager.FinishedProcessingPath(waypoints, pathSuccess);
                yield break;
            }


            if (startNode.allowedMovementTypes.HasFlag(request.movementType) && 
                targetNode.allowedMovementTypes.HasFlag(request.movementType))
            {
                Heap<PathfindingTile> openSet = new Heap<PathfindingTile>(roomInterface.GetMaxRoomSize());
                HashSet<PathfindingTile> closedSet = new HashSet<PathfindingTile>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    // grab lowest fCost tile. Due to the heap data structure, this will always be the first element
                    PathfindingTile currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        // found target
                        pathSuccess = true;
                        break;
                    }

                    foreach (PathfindingTile neighbor in roomInterface.GetNeighbors(currentNode,
                                 request.movementType))
                    {
                        if (!neighbor.allowedMovementTypes.HasFlag(request.movementType) || closedSet.Contains(neighbor))
                        {
                            // this node has already been explored, or is not walkable, so skip
                            continue;
                        }

                        int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) +
                                                        neighbor.movementPenalty;
                        if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                        {
                            neighbor.gCost = newMovementCostToNeighbor;
                            neighbor.hCost = GetDistance(neighbor, targetNode);
                            neighbor.retraceStep = currentNode;

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbor);
                            }
                        }
                    }
                }
            }

            yield return null;
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode, request);
            }

            requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }

        /// <summary>
        /// Instantaneously seeks a path. May cause lag if used too much.
        /// </summary>
        /// <param name="request"> The path request data </param>
        public (Vector2[], bool) FindPathSync(PathRequest request)
        {
            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;

            var startNodeResult = roomInterface.WorldPosToTile(request.startPos);
            var targetNodeResult = roomInterface.WorldPosToTile(request.endPos);

            PathfindingTile startNode;
            PathfindingTile targetNode;

            if (startNodeResult.Item2 && targetNodeResult.Item2)
            {
                startNode = startNodeResult.Item1;
                targetNode = targetNodeResult.Item1;
                startNode.retraceStep = startNode;
            }
            else
            {
                return (waypoints, pathSuccess);
            }


            if (startNode.allowedMovementTypes.HasFlag(request.movementType) && 
                targetNode.allowedMovementTypes.HasFlag(request.movementType))
            {
                Heap<PathfindingTile> openSet = new Heap<PathfindingTile>(roomInterface.GetMaxRoomSize());
                HashSet<PathfindingTile> closedSet = new HashSet<PathfindingTile>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    // grab lowest fCost tile. Due to the heap data structure, this will always be the first element
                    PathfindingTile currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        // found target
                        pathSuccess = true;
                        break;
                    }

                    foreach (PathfindingTile neighbor in roomInterface.GetNeighbors(currentNode,
                                 request.movementType))
                    {
                        if (!neighbor.allowedMovementTypes.HasFlag(request.movementType) || closedSet.Contains(neighbor))
                        {
                            // this node has already been explored, or is not walkable, so skip
                            continue;
                        }

                        int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) +
                                                        neighbor.movementPenalty;
                        if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                        {
                            neighbor.gCost = newMovementCostToNeighbor;
                            neighbor.hCost = GetDistance(neighbor, targetNode);
                            neighbor.retraceStep = currentNode;

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbor);
                            }
                        }
                    }
                }
            }

            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode, request);
            }

            return (waypoints, pathSuccess);
        }

        /// <summary>
        /// Retrace path backwards via parents to determine final shortest path
        /// </summary>
        /// <param name="startTile"> Start tile </param>
        /// <param name="endTile"> End tile </param>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> Array containing waypoints to travel from start to end </returns>
        Vector2[] RetracePath(PathfindingTile startTile, PathfindingTile endTile, PathRequest request)
        {
            List<PathfindingTile> path = new List<PathfindingTile>();
            PathfindingTile currentNode = endTile;

            while (currentNode != startTile)
            {
                path.Add(currentNode);
                currentNode = currentNode.retraceStep;
            }

            Vector2[] waypoints = PathToVectors(path, request);
            Array.Reverse(waypoints);
            return waypoints;
        }

        /// <summary>
        /// Converts given PathfindingTile path into a path of Vector2 waypoints
        /// </summary>
        /// <param name="path"> Input path </param>
        /// <param name="stateMachine"> The stateMachine to use </param>
        /// <returns> List of Vector2 waypoints on every PathfindingTile point </returns>
        Vector2[] PathToVectors(List<PathfindingTile> path, PathRequest request)
        {
            List<Vector2> waypoints = new List<Vector2>();
            waypoints.Add(request.endPos);
            foreach (var tile in path)
            {
                waypoints.Add(roomInterface.TileToWorldPos(tile));
            }

            return waypoints.ToArray();
        }

        /// <summary>
        /// Determine how many tiles are in between tile a and tile b
        /// </summary>
        /// <param name="a"> First tile </param>
        /// <param name="b"> Second tile </param>
        /// <returns> Distance, in tiles (moves), between the two tiles </returns>
        int GetDistance(PathfindingTile a, PathfindingTile b)
        {
            int distX = Mathf.Abs(a.gridLocation.x - b.gridLocation.x);
            int distY = Mathf.Abs(a.gridLocation.y - b.gridLocation.y);

            if (distX > distY)
            {
                // 24 is the cost of a diagonal move
                // 10 is the cost of a straight move
                return 24 * distY + 10 * (distX - distY);
            }
            else
            {
                return 24 * distX + 10 * (distY - distX);
            }
        }
    }
}
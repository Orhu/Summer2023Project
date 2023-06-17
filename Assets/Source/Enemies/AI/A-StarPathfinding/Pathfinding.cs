using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathfindingTile = RoomInterface.PathfindingTile;

namespace Cardificer
{
    /// <summary>
    /// Handles A* pathfinding for enemies
    /// </summary>
    public class Pathfinding : MonoBehaviour
    {
        // the request manager component that sends us requests
        private PathRequestManager requestManager;

        // the room interface that allows us to mess with room properties without having to mess with the room itself
        private RoomInterface roomInterface;

        /// <summary>
        /// Initialize variables
        /// </summary>
        void Awake()
        {
            requestManager = GetComponent<PathRequestManager>();
            roomInterface = GetComponent<RoomInterface>();
        }

        /// <summary>
        /// Starts the FindPath coroutine from start to target pos
        /// </summary>
        /// <param name="stateMachine"> The state machine that is requesting a path </param>
        public void StartFindPath(BaseStateMachine stateMachine)
        {
            StartCoroutine(FindPath(stateMachine));
        }

        /// <summary>
        /// Find a path from a given pos to a target pos
        /// </summary>
        /// <param name="stateMachine"> The state machine that is requesting a path </param>
        /// <returns> Sends a signal to the request manager that a path has been found </returns>
        IEnumerator FindPath(BaseStateMachine stateMachine)
        {
            roomInterface.GrabCurrentRoom();
            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;

            if (stateMachine == null)
            {
                // if we are in here, the enemy died sometime between submitting a path request and this point.
                // do not send a callback as that will just error, instead just do nothing.
                yield break;
            }

            var startNodeResult = roomInterface.WorldPosToTile(stateMachine.feetCollider.transform.position);
            var targetNodeResult = roomInterface.WorldPosToTile(stateMachine.currentTarget);

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
                requestManager.FinishedProcessingPath(waypoints, pathSuccess, stateMachine);
                yield break;
            }


            if (startNode.walkable && targetNode.walkable)
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

                    foreach (PathfindingTile neighbor in roomInterface.GetNeighbors(currentNode))
                    {
                        if (!neighbor.walkable || closedSet.Contains(neighbor))
                        {
                            // this node has already been explored, or is not walkable, so skip
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor) +
                                                         neighbor.movementPenalty;
                        if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor))
                        {
                            neighbor.gCost = newMovementCostToNeighbour;
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
                waypoints = RetracePath(startNode, targetNode);
            }

            requestManager.FinishedProcessingPath(waypoints, pathSuccess, stateMachine);
        }

        /// <summary>
        /// Retrace path backwards via parents to determine final shortest path
        /// </summary>
        /// <param name="startTile"> Start tile </param>
        /// <param name="endTile"> End tile </param>
        /// <returns> Array containing waypoints to travel from start to end </returns>
        Vector2[] RetracePath(PathfindingTile startTile, PathfindingTile endTile)
        {
            List<PathfindingTile> path = new List<PathfindingTile>();
            PathfindingTile currentNode = endTile;

            while (currentNode != startTile)
            {
                path.Add(currentNode);
                currentNode = currentNode.retraceStep;
            }

            Vector2[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        /// <summary>
        /// Simplifies path by removing unnecessary waypoints by determining directions
        /// </summary>
        /// <param name="path"> Input path </param>
        /// <returns></returns>
        Vector2[] SimplifyPath(List<PathfindingTile> path)
        {
            /*
                    List<Vector2> waypoints = new List<Vector2>();
                    waypoints.Add(targetPosition);
                    Vector2 directionOld = Vector2.zero;

                    for (int i = 1; i < path.Count; i ++) {
                        Vector2 directionNew = new Vector2(path[i-1].gridLocation.x - path[i].gridLocation.x,path[i-1].gridLocation.y - path[i].gridLocation.y);
                        if (directionNew != directionOld) {
                            waypoints.Add(roomInterface.TileToWorldPos(path[i]));
                        }
                        directionOld = directionNew;
                    }*/
            List<Vector2> waypoints = new List<Vector2>();
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
                // 14 is the cost of a diagonal move
                // 10 is the cost of a straight move
                return 14 * distY + 10 * (distX - distY);
            }
            else
            {
                return 14 * distX + 10 * (distY - distX);
            }
        }
    }
}
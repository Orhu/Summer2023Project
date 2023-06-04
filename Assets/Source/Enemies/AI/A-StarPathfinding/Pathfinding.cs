using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathfindingTile = RoomInterface.PathfindingTile;

/// <summary>
/// Handles A* pathfinding for enemies
/// </summary>
public class Pathfinding : MonoBehaviour
{
    // the request manager component that sends us requests
    private PathRequestManager requestManager;
    
    // the room interface that allows us to mess with room properties without having to mess with the room itself
    private RoomInterface roomInterface;

    // current target position
    private Vector2 targetPosition;

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
    /// <param name="startPos"> Starting position </param>
    /// <param name="targetPos"> Target position </param>
    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    /// <summary>
    /// Find a path from a given pos to a target pos
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    /// <param name="targetPos"> Target position </param>
    /// <returns> Sends a signal to the request manager that a path has been found </returns>
    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    { 
        targetPosition = targetPos;
        roomInterface.GrabCurrentRoom();
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;
		
        PathfindingTile startNode = roomInterface.WorldPosToTile(startPos);
        PathfindingTile targetNode = roomInterface.WorldPosToTile(targetPos);
        startNode.retraceStep = startNode;
		
		
        if (startNode.walkable && targetNode.walkable) {
            Heap<PathfindingTile> openSet = new Heap<PathfindingTile>(roomInterface.GetMaxRoomSize());
            HashSet<PathfindingTile> closedSet = new HashSet<PathfindingTile>();
            openSet.Add(startNode);
			
            while (openSet.Count > 0) {
                // grab lowest fCost tile. Due to the heap data structure, this will always be the first element
                PathfindingTile currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
				
                if (currentNode == targetNode) {
                    // found target
                    pathSuccess = true;
                    break;
                }
				
                foreach (PathfindingTile neighbor in roomInterface.GetNeighbors(currentNode)) {
                    if (!neighbor.walkable || closedSet.Contains(neighbor)) {
                        // this node has already been explored, or is not walkable, so skip
                        continue;
                    }
					
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;
                    if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor)) {
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
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
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
		
        while (currentNode != startTile) {
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
        List<Vector2> waypoints = new List<Vector2>();
        waypoints.Add(targetPosition);
        Vector2 directionOld = Vector2.zero;
		
        for (int i = 1; i < path.Count; i ++) {
            Vector2 directionNew = new Vector2(path[i-1].gridLocation.x - path[i].gridLocation.x,path[i-1].gridLocation.y - path[i].gridLocation.y);
            if (directionNew != directionOld) {
                waypoints.Add(roomInterface.TileToWorldPos(path[i]));
            }
            directionOld = directionNew;
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
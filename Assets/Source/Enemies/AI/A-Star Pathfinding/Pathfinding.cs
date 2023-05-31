using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles A* pathfinding for enemies
/// </summary>
public class Pathfinding : MonoBehaviour
{
    // the request manager component that sends us requests
    private PathRequestManager requestManager;

    // current target position
    private Vector2 targetPosition;

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
    }

    /// <summary>
    /// Find a path from a given pos to a target pos
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    /// <param name="targetPos"> Target position </param>
    /// <param name="room"> The room the enemy is a part of </param>
    /// <returns> Sends a signal to the request manager that a path has been found </returns>
    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos, Room room)
    {

        targetPosition = targetPos;
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;
		
        Tile startNode = room.WorldPosToTile(startPos);
        Tile targetNode = room.WorldPosToTile(targetPos);
		
		
        if (startNode.walkable && targetNode.walkable) {
            Heap<Tile> openSet = new Heap<Tile>(room.maxSize);
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startNode);
			
            while (openSet.Count > 0) {
                // grab lowest fCost tile. Due to the heap data structure, this will always be the first element
                Tile currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
				
                if (currentNode == targetNode) {
                    // found target
                    pathSuccess = true;
                    break;
                }
				
                foreach (Tile neighbor in room.GetNeighbors(currentNode)) {
                    if (!neighbor.walkable || closedSet.Contains(neighbor)) {
                        // this node has already been explored, or is not walkable, so skip
                        continue;
                    }
					
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = newMovementCostToNeighbour;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;
						
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode, room);
        }
        requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
    }

    /// <summary>
    /// Retrace path backwards via parents to determine final shortest path
    /// </summary>
    /// <param name="startTile"> Start tile </param>
    /// <param name="endTile"> End tile </param>
    /// <param name="room"> The room the enemy is a part of </param>
    /// <returns> Array containing waypoints to travel from start to end </returns>
    Vector2[] RetracePath(Tile startTile, Tile endTile, Room room)
    {
        List<Tile> path = new List<Tile>();
        Tile currentNode = endTile;
		
        while (currentNode != startTile) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2[] waypoints = SimplifyPath(path, room);
        Array.Reverse(waypoints);
        return waypoints;
		
    }

    /// <summary>
    /// Simplifies path by removing unnecessary waypoints by determining directions
    /// </summary>
    /// <param name="path"> Input path </param>
    /// <param name="room"> The room the enemy is a part of </param>
    /// <returns></returns>
    Vector2[] SimplifyPath(List<Tile> path, Room room)
    {
        List<Vector2> waypoints = new List<Vector2>();
        // add target destination as our final waypoint, no matter what
        waypoints.Add(targetPosition);
        Vector2 directionOld = Vector2.zero;
		
        for (int i = 1; i < path.Count; i ++) {
            Vector2 directionNew = new Vector2(path[i-1].gridLocation.x - path[i].gridLocation.x,path[i-1].gridLocation.y - path[i].gridLocation.y);
            if (directionNew != directionOld) {
                // change in direction, add waypoint here
                waypoints.Add(room.TileToWorldPos(path[i]));
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
    int GetDistance(Tile a, Tile b)
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

    /// <summary>
    /// Starts the FindPath coroutine from start to target pos
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    /// <param name="targetPos"> Target position </param>
    /// <param name="room"> The room this enemy is part of </param>
    public void StartFindPath(Vector2 startPos, Vector2 targetPos, Room room)
    {
        StartCoroutine(FindPath(startPos, targetPos, room));
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    // the request manager component that sends us requests
    private PathRequestManager requestManager;

    // The room this enemy is part of
    private Room room;

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        room = GetComponentInParent<Room>();
        requestManager = GetComponent<PathRequestManager>();
    }

    /// <summary>
    /// Find a path from a given pos to a target pos
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    /// <param name="targetPos"> Target position </param>
    /// <returns> Sends a signal to the request manager that a path has been found </returns>
    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] waypoints = new Vector2[0];
        bool pathfindSuccess = false;

        Tile startTile = room.WorldPosToTile(startPos);
        Tile targetTile = room.WorldPosToTile(targetPos);

        if (startTile.walkable && targetTile.walkable)
        {
            Heap<Tile> openList = new Heap<Tile>(room.maxSize);
            HashSet<Tile> closedList = new HashSet<Tile>();
            openList.Add(startTile);

            while (openList.Count > 0)
            {
                // grab lowest fCost tile
                Tile currentTile = openList.RemoveFirst();
                closedList.Add(currentTile);

                if (currentTile == targetTile)
                {
                    // found the target
                    pathfindSuccess = true;
                    break;
                }

                foreach (Tile neighbor in room.GetNeighbors(currentTile))
                {
                    if (!neighbor.walkable || closedList.Contains(neighbor))
                    {
                        // this node has already been explored, or it is not walkable, so skip
                        continue;
                    }

                    int newCostToNeighbor = currentTile.gCost + GetDistance(currentTile, neighbor);
                    if (newCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetTile);
                        neighbor.parent = currentTile;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
        }

        yield return null;
        if (pathfindSuccess)
        {
            waypoints = RetracePath(startTile, targetTile);
        }

        requestManager.FinishedProcessingPath(waypoints, pathfindSuccess);
    }

    /// <summary>
    /// Retrace path backwards via parents to determine final shortest path
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <returns> Array containing waypoints to travel from start to end </returns>
    Vector2[] RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
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
    Vector2[] SimplifyPath(List<Tile> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridLocation.x - path[i].gridLocation.x, path[i - 1].gridLocation.y - path[i].gridLocation.y);
            if (directionNew != directionOld)
            {
                // we have a change in direction, add a waypoint here
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
        int distX = Mathf.Abs(a.gridLocation.x - b.gridLocation.y);
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
    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Tile = Room.Tile;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    
    // represents the grid of this room
    private Room.RoomGrid grid;

    void Start()
    {
        grid = GetComponentInParent<Room>().ThisRoom;
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }
    
    void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Tile startTile = grid.TileFromWorldPoint(startPos);
        Tile targetTile = grid.TileFromWorldPoint(targetPos);
        
        List<Tile> openList = new List<Tile>();
        HashSet<Tile> closedList = new HashSet<Tile>();
        openList.Add(startTile);

        while (openList.Count > 0)
        {
            Tile currentTile = openList[0];
            // finding the lowest fCost tile
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentTile.fCost || (openList[i].fCost == currentTile.fCost && openList[i].hCost < currentTile.hCost))
                {
                    currentTile = openList[i];
                }
            }
            
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == targetTile)
            {
                // found the target
                RetracePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbor in grid.GetNeighbors(currentTile))
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

    /// <summary>
    /// Retrace path backwards via parents to determine final shortest path
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    void RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
    }

    /// <summary>
    /// Determine how many tiles are in between tile a and tile b
    /// </summary>
    /// <param name="a"> First tile </param>
    /// <param name="b"> Second tile </param>
    /// <returns> Distance, in tiles (moves), between the two tiles </returns>
    int GetDistance(Tile a, Tile b)
    {
        int distX = Mathf.Abs(a.gridX - b.gridX);
        int distY = Mathf.Abs(a.gridY - b.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}

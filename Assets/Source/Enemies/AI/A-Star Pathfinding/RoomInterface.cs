using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInterface : MonoBehaviour
{
    private Room myRoom;
    private Vector2Int myRoomSize;
    private Tile[,] myRoomGrid;
    private Vector2 myWorldPosition;
    
    public void UpdateRoom(Room newRoom)
    {
        myRoom = newRoom;
        myRoomSize = newRoom.roomSize;
        myWorldPosition = newRoom.transform.position;
        DeepCopyGrid(newRoom.roomGrid);
    }

    void DeepCopyGrid(Tile[,] inputArray)
    {
        myRoomGrid = new Tile[myRoomSize.x, myRoomSize.y];
        
        foreach (var tile in inputArray)
        {
            myRoomGrid[tile.gridLocation.x, tile.gridLocation.y] = tile;
        }
    }
    
      /// <summary>
    /// Gets the tile at the given world position
    /// </summary>
    /// <param name="worldPos"> The world position </param>
    /// <returns> The tile </returns>
    public Tile WorldPosToTile(Vector2 worldPos)
      {
          Vector2Int tilePos = new Vector2Int(
              Mathf.RoundToInt(worldPos.x + myRoomSize.x / 2 - myWorldPosition.x),
              Mathf.RoundToInt(worldPos.y + myRoomSize.y / 2 - myWorldPosition.y)
          );
          return myRoomGrid[tilePos.x, tilePos.y];
    }

    /// <summary>
    /// Gets the world position of the given tile
    /// </summary>
    /// <param name="tile"> The tile </param>
    /// <returns> The world position </returns>
    public Vector2 TileToWorldPos(Tile tile)
    {
        Vector2 worldPos = new Vector2(
            tile.gridLocation.x + myWorldPosition.x - myRoomSize.x / 2,
            tile.gridLocation.y + myWorldPosition.y - myRoomSize.y / 2
        );
        return worldPos;
    }

    /// <summary>
    /// Gets the neighbors of a given tile
    /// </summary>
    /// <param name="tile"> The tile </param>
    /// <returns> The neighbors </returns>
    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        // Loop through each adjacent tile
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the current tile itself
                if (x == 0 && y == 0)
                    continue;

                int checkX = tile.gridLocation.x + x;
                int checkY = tile.gridLocation.y + y;

                // Check if the adjacent tile is within the grid bounds
                if (checkX >= 0 && checkX < myRoomSize.x && checkY >= 0 && checkY < myRoomSize.y)
                {
                    neighbors.Add(myRoomGrid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
}

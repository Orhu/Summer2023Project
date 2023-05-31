using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a grid of tiles, and handles any room behaviors
/// </summary>
public class Room : MonoBehaviour
{
    // The grid of tiles
    [HideInInspector] public Tile[,] roomGrid;

    // The size of the room
    [HideInInspector] public Vector2Int roomSize;

    // Returns the size of the room, x * y
    [HideInInspector]
    public int maxSize
    {
        // TODO not sure if this is right
        get => roomSize.x * roomSize.y;
    }

    // The type of the room
    [HideInInspector] public RoomType roomType;

    // The location of the room in the map
    [HideInInspector] public Vector2Int roomLocation;

    // Whether this room has been generated or not
    [HideInInspector] private bool generated = false;

    /// <summary>
    /// Gets the tile at the given world position
    /// </summary>
    /// <param name="worldPos"> The world position </param>
    /// <returns> The tile </returns>
    public Tile WorldPosToTile(Vector2 worldPos)
    {
        Vector2Int gridLocation = new Vector2Int();
        gridLocation.x = Mathf.RoundToInt(worldPos.x - (transform.position.x - roomSize.x / 2));
        gridLocation.y = Mathf.RoundToInt(worldPos.y - (transform.position.y - roomSize.y / 2));
        return roomGrid[gridLocation.x, gridLocation.y];
    }

    /// <summary>
    /// Gets the world position of the given tile
    /// </summary>
    /// <param name="tile"> The tile </param>
    /// <returns> The world position </returns>
    public Vector2 TileToWorldPos(Tile tile)
    {
        Vector2 worldPos = new Vector2();
        worldPos.x = tile.gridLocation.x + transform.position.x - roomSize.x / 2;
        worldPos.y = tile.gridLocation.y + transform.position.y - roomSize.y / 2;
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
                if (checkX >= 0 && checkX < roomSize.x && checkY >= 0 && checkY < roomSize.y)
                {
                    try
                    {
                        // Check specific cases for corner tiles
                        if (y == -1 && x == -1)
                        {
                            // Bottom left corner tile
                            // Make sure the corner is reachable by either the tile above or the tile to the right
                            if (roomGrid[checkX + x, checkY].walkable && roomGrid[checkX, checkY - y].walkable)
                            {
                                neighbors.Add(roomGrid[checkX, checkY]);
                            }
                        }
                        else if (y == -1 && x == 1)
                        {
                            // Bottom right corner tile
                            // Make sure the corner is reachable by either the tile above or the tile to the left
                            if (roomGrid[checkX - x, checkY].walkable && roomGrid[checkX, checkY + y].walkable)
                            {
                                neighbors.Add(roomGrid[checkX, checkY]);
                            }
                        }
                        else if (y == 1 && x == -1)
                        {
                            // Top left corner tile
                            // Make sure the corner is reachable by either the tile below or the tile to the right
                            if (roomGrid[checkX + x, checkY].walkable && roomGrid[checkX, checkY - y].walkable)
                            {
                                neighbors.Add(roomGrid[checkX, checkY]);
                            }
                        }
                        else if (y == 1 && x == 1)
                        {
                            // Top right corner tile
                            // Make sure the corner is reachable by either the tile below or the tile to the left
                            if (roomGrid[checkX - x, checkY].walkable && roomGrid[checkX, checkY - y].walkable)
                            {
                                neighbors.Add(roomGrid[checkX, checkY]);
                            }
                        }
                        else
                        {
                            // This tile is in a cardinal direction, no need to check anything. Just add it!
                            neighbors.Add(roomGrid[checkX, checkY]);
                        }
                    }
                    catch
                    {
                        // Catch here in case one of the tiles being checked is out of bounds in the grid.
                        // We don't want to cause an error, so we simply skip adding that tile to the neighbours list.
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// TODO: Implement this
    /// </summary>
    public void OpenDoors()
    {
    }

    /// <summary>
    /// TODO: Implement this
    /// </summary>
    public void CloseDoors()
    {
    }

    /// <summary>
    /// Handles when the player enters the room
    /// </summary>
    /// <param name="collision"> The collider that entered the trigger </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FloorGenerator.floorGeneratorInstance.currentRoom = this;
            if (!generated)
            {
                Template template = transform.parent.gameObject.GetComponent<FloorGenerator>().floorGenerationParameters
                    .templateGenerationParameters.GetRandomTemplate(roomType);
                GetComponent<TemplateGenerator>().Generate(this, template);
                generated = true;
            }
        }
    }

    /// <summary>
    /// Handles when the player exits the room
    /// </summary>
    /// <param name="collision"> The collider that exited the trigger </param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<TilemapRenderer>().enabled = false;
        }*/
    }
}
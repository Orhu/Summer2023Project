using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a grid of tiles, and handles any room behaviors
/// </summary>
public class Room : MonoBehaviour
{
    [HideInInspector] public Tile[,] roomGrid;

    [HideInInspector] public Vector2Int roomSize;

    [HideInInspector] public RoomType roomType;

    [HideInInspector] public Vector2Int roomLocation;

    [HideInInspector] private bool generated = false;
    
    public Tile WorldPosToTile(Vector2 worldPos)
    {
        Vector2Int gridLocation = new Vector2Int();
        gridLocation.x = (int) (worldPos.x - transform.position.x) / roomSize.x;
        gridLocation.y = (int) (worldPos.y - transform.position.y) / roomSize.y;
        return roomGrid[gridLocation.x, gridLocation.y];
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int i = 1; i <= (int) Direction.Down; i *= 2)
        {
            Vector2Int locationOffset = new Vector2Int();
            locationOffset.x = System.Convert.ToInt32(((Direction) i & Direction.Right) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Left) != Direction.None);
            locationOffset.y = System.Convert.ToInt32(((Direction) i & Direction.Up) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Down) != Direction.None);

            bool outOfXRange = tile.gridLocation.x + locationOffset.x < 0 || tile.gridLocation.x + locationOffset.x >= roomSize.x;
            bool outOfYRange = tile.gridLocation.y + locationOffset.y < 0 || tile.gridLocation.y + locationOffset.y >= roomSize.y;
            if (outOfXRange || outOfYRange) { continue; }

            neighbors.Add(roomGrid[tile.gridLocation.x + locationOffset.x, tile.gridLocation.y + locationOffset.y]);
        }

        return neighbors;
    }

    public void OpenDoors()
    {

    }

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
            if (!generated)
            {
                Template template = transform.parent.gameObject.GetComponent<FloorGenerator>().floorGenerationParameters.templateGenerationParameters.GetRandomTemplate(roomType);
                GetComponent<TemplateGenerator>().Generate(this, template);
                generated = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<TilemapRenderer>().enabled = false;
        }*/
    }
}

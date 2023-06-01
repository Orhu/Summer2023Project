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
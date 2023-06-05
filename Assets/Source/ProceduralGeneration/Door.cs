using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles door behavior
/// </summary>
public class Door : MonoBehaviour
{
    [Tooltip("The open and closed sprites for this door")]
    public DoorSprites doorSprites;

    // The direction the door goes in
    [HideInInspector] public Direction direction;

    // The room that this door connects with
    [HideInInspector] public MapCell connectedCell;

    // Whether or not this door can be entered
    [HideInInspector] public bool enterable = false;

    /// <summary>
    /// Sets the door to the open state
    /// </summary>
    public void Open()
    {
        GetComponent<SpriteRenderer>().sprite = doorSprites.doorOpened;
        GetComponent<BoxCollider2D>().isTrigger = true;
        // Make the box collider a bit smaller so the player can't get stuck on walls when trying to move through doors
        GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
    }

    /// <summary>
    /// Sets the door to the closed state
    /// </summary>
    public void Close()
    {
        GetComponent<SpriteRenderer>().sprite = doorSprites.doorClosed;
        GetComponent<BoxCollider2D>().isTrigger = false;
        // Make the box collider normal sized so the door acts like a wall
        GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
    }

    /// <summary>
    /// Enters the other room
    /// </summary>
    public void Enter()
    {
        if (enterable)
        {
            FloorGenerator.floorGeneratorInstance.currentRoom.Exit();

            // Get the opposite direction (since the bottom door of this room goes to the top door of the next room)
            int oppositeDirection = (int)direction;
            oppositeDirection *= 2;
            oppositeDirection *= 2;
            oppositeDirection = oppositeDirection % (int)Direction.All;
            connectedCell.room.GetComponent<Room>().Enter((Direction) oppositeDirection);
        }
    }

    /// <summary>
    /// Enters the other room
    /// </summary>
    /// <param name="collision"> The collision that entered this door </param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && GetComponent<BoxCollider2D>().isTrigger)
        {
            Enter();
        }
    }
}

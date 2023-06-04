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
    [HideInInspector] public Room connectedRoom;

    // Whether or not this door can be entered
    [HideInInspector] public bool enterable = false;

    /// <summary>
    /// Sets the door to the open state
    /// </summary>
    public void Open()
    {
        GetComponent<SpriteRenderer>().sprite = doorSprites.doorOpened;
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    /// <summary>
    /// Sets the door to the closed state
    /// </summary>
    public void Close()
    {
        GetComponent<SpriteRenderer>().sprite = doorSprites.doorClosed;
        GetComponent<BoxCollider2D>().isTrigger = false;
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
            connectedRoom.Enter((Direction) oppositeDirection);
        }
    }

    /// <summary>
    /// Enters the other room
    /// </summary>
    /// <param name="collision"> The collision that entered this door </param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Enter();
        }
    }
}

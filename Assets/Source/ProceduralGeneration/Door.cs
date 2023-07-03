using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Handles door behavior
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Tooltip("The open and closed sprites for this door")]
        public DoorSprites doorSprites;

        [Tooltip("The width of the door collider when it's made smaller")]
        public float colliderWidth;

        // The direction the door goes in
        [HideInInspector] public Direction direction;

        // The room that this door connects with
        [HideInInspector] public MapCell connectedCell;

        // Whether or not this door can be entered
        [HideInInspector] public bool enterable = false;

        // The sprite renderer
        SpriteRenderer spriteRenderer;

        // The box collider
        BoxCollider2D boxCollider;

        /// <summary>
        /// Initializes the sprite renderer with the correct sprite and sets up component references
        /// </summary>
        /// <param name="doorSprites"> The opened and closed sprites of this door </param>
        /// <param name="connectedCell"> The cell this door connects to </param>
        /// <param name="direction"> The direction this door opens in </param>
        public void Initialize(DoorSprites doorSprites, MapCell connectedCell, Direction direction)
        {
            this.doorSprites = doorSprites;
            this.connectedCell = connectedCell;
            this.direction = direction;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = doorSprites.doorOpened;
            boxCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Sets the door to the open state
        /// </summary>
        public void Open()
        {
            spriteRenderer.sprite = doorSprites.doorOpened;
            boxCollider.isTrigger = true;

            bool xDirection = ((direction & Direction.Right) | (direction & Direction.Left)) != Direction.None;
            float xWidth = xDirection ? colliderWidth : 1.0f;

            bool yDirection = ((direction & Direction.Up) | (direction & Direction.Down)) != Direction.None;
            float yWidth = yDirection ? colliderWidth : 1.0f;

            float xOffset = (System.Convert.ToInt32((direction & Direction.Right) != Direction.None)) * ((1.0f - colliderWidth) / 2.0f);
            xOffset += -(System.Convert.ToInt32((direction & Direction.Left) != Direction.None)) * ((1.0f - colliderWidth) / 2.0f);

            float yOffset = (System.Convert.ToInt32((direction & Direction.Up) != Direction.None)) * ((1.0f - colliderWidth) / 2.0f);
            yOffset += -(System.Convert.ToInt32((direction & Direction.Down) != Direction.None)) * ((1.0f - colliderWidth) / 2.0f);

            boxCollider.size = new Vector2(xWidth, yWidth);
            boxCollider.offset = new Vector2(xOffset, yOffset);
        }

        /// <summary>
        /// Sets the door to the closed state
        /// </summary>
        public void Close()
        {
            spriteRenderer.sprite = doorSprites.doorClosed;
            boxCollider.isTrigger = false;
            // Make the box collider normal sized so the door acts like a wall
            boxCollider.offset = new Vector2(0, 0);
            boxCollider.size = new Vector2(1, 1);
        }

        /// <summary>
        /// Enters the other room
        /// </summary>
        public void Enter()
        {
            if (enterable)
            {
                FloorGenerator.currentRoom.Exit();

                // Get the opposite direction (since the bottom door of this room goes to the top door of the next room)
                int oppositeDirection = (int)direction;
                // Rotating the direction twice is equivalent to multiplying by 4 because bits 
                oppositeDirection *= 4;
                oppositeDirection = oppositeDirection % (int) Direction.All;
                connectedCell.room.GetComponent<Room>().Enter((Direction) oppositeDirection);
            }
        }

        /// <summary>
        /// Enters the other room
        /// </summary>
        /// <param name="collision"> The collision that entered this door </param>
        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Player") && GetComponentInChildren<BoxCollider2D>().isTrigger)
            {
                Enter();
            }
        }
    }

}
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

        // The direction the door goes in
        [HideInInspector] public Direction direction;

        // The room that this door connects with
        [HideInInspector] public MapCell connectedCell;

        // Whether or not this door can be entered
        [HideInInspector] public bool enterable = false;

        /// <summary>
        /// Initialize Collision
        /// </summary>
        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Walls");
            gameObject.tag = "Inanimate";
        }

        /// <summary>
        /// Sets the door to the open state
        /// </summary>
        public void Open()
        {
            GetComponent<SpriteRenderer>().sprite = doorSprites.doorOpened;
            GetComponentInChildren<BoxCollider2D>().isTrigger = true;
            // Make the box collider a bit smaller so the player can't get stuck on walls when trying to move through doors
            Vector2 doorSize = new Vector2();
            if ((direction & Direction.Right) != Direction.None)
            {
                doorSize = new Vector2(0.1f, 1.0f);
                transform.GetChild(0).localPosition = new Vector3(0.45f, 0);
            }
            else if ((direction & Direction.Up) != Direction.None)
            {
                doorSize = new Vector2(1f, 0.1f);
                transform.GetChild(0).localPosition = new Vector3(0, 0.45f);
            }
            else if ((direction & Direction.Left) != Direction.None)
            {
                doorSize = new Vector2(0.1f, 1.0f);
                transform.GetChild(0).localPosition = new Vector3(-0.45f, 0);
            }
            else if ((direction & Direction.Down) != Direction.None)
            {
                doorSize = new Vector2(1.0f, 0.1f);
                transform.GetChild(0).localPosition = new Vector3(0, -0.45f);
            }
            GetComponentInChildren<BoxCollider2D>().size = doorSize;
        }

        /// <summary>
        /// Sets the door to the closed state
        /// </summary>
        public void Close()
        {
            GetComponent<SpriteRenderer>().sprite = doorSprites.doorClosed;
            GetComponentInChildren<BoxCollider2D>().isTrigger = false;
            // Make the box collider normal sized so the door acts like a wall
            transform.GetChild(0).localPosition = new Vector3(0, 0);
            GetComponentInChildren<BoxCollider2D>().size = new Vector2(1, 1);
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
                oppositeDirection *= 2; // wait a second why not just *= 4
                oppositeDirection = oppositeDirection % (int)Direction.All;
                connectedCell.room.GetComponent<Room>().Enter((Direction)oppositeDirection);
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
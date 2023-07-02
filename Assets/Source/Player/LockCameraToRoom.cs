using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Cause the camera to follow the actor and snap to the center of rooms.
    /// </summary>
    public class LockCameraToRoom : MonoBehaviour
    {
        [Tooltip("The speed at which the camera snaps.")]
        public float speed = 10;

        [Tooltip("The extra height that is added onto the size of the rooms")]
        public float extraHeight;

        // The extra width (determined from the extra height and aspect ratio)
        private float extraWidth;

        // The height of the camera
        private float height;

        // The width of the camera (determined from the height and aspect ratio)
        private float width;

        // A reference to the player
        private GameObject player;

        // Whether or not the camera is currently snapping between rooms
        private bool snapping = true;

        // The minimum position of the camera
        private Vector2 minPosition;

        // The maximum position of the camera
        private Vector2 maxPosition;

        /// <summary>
        /// Initializes the height, floor generator, and player references
        /// </summary>
        private void Start()
        {
            Vector2 roomScale = FloorGenerator.cellSize;

            if (roomScale.y > roomScale.x * (1 / GetComponent<Camera>().aspect))
            {
                height = roomScale.y;
            }
            else
            {
                height = roomScale.x * (1 / GetComponent<Camera>().aspect);
            }

            height += extraHeight;
            GetComponent<Camera>().orthographicSize = height / 2;

            extraWidth = extraHeight * GetComponent<Camera>().aspect / 2;
            width = height * GetComponent<Camera>().aspect;

            FloorGenerator.onRoomChange.AddListener(OnRoomChange);
            player = Player.Get();

            DetermineMinAndMax(FloorGenerator.map.startRoom);
        }

        //TODO: DELETE
        #region Stuff to Delete
        private bool mapping;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                GetComponent<Camera>().orthographicSize *= 10;
                FloorGenerator.ShowLayout(false);
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                GetComponent<Camera>().orthographicSize /= 10;
                FloorGenerator.HideLayout();
            }
        }
        #endregion Stuff to Delete

        /// <summary>
        /// Updates the position.
        /// </summary>
        private void FixedUpdate()
        {
            if (FloorGenerator.currentRoom == null)
            {
                return;
            }
            
            if (!snapping && !mapping)
            {
                transform.position = GetCameraPosition();
            }
        }

        /// <summary>
        /// Gets where the camera should be for this frame
        /// </summary>
        /// <returns> The new camera position </returns>
        private Vector3 GetCameraPosition()
        {
            Vector3 playerPos = player.transform.position;
            Vector3 position = new Vector3();

            // Min and max correspond to the min and max positions of the corners of the camera frame, so we gotta add/subtract half the camera
            // .01 is only added because the player starts at exactly 0, 0 at that kinda screws things up
            if (playerPos.x - width / 2 < minPosition.x + 0.1)
            {
                position.x = minPosition.x + width / 2;
            }
            else if (playerPos.x + width / 2 > maxPosition.x - 0.1)
            {
                position.x = maxPosition.x - width  / 2;
            }
            else
            {
                position.x = playerPos.x;
            }

            if (playerPos.y - height / 2 < minPosition.y + 0.1)
            {
                position.y = minPosition.y + height / 2;
            }
            else if (playerPos.y + height / 2 > maxPosition.y - 0.1)
            {
                position.y = maxPosition.y - height / 2;
            }
            else
            {
                position.y = playerPos.y;
            }

            position.z = -1;

            return position;
        }

        /// <summary>
        /// Determines the min and max location of the camera
        /// </summary>
        /// <param name="room"> The room to use to determine min and max; if left blank, uses the floor generator's current room </param>
        private void DetermineMinAndMax(Room room = null)
        {
            Room roomToUse;
            if (room == null)
            {
                roomToUse = FloorGenerator.currentRoom;
            }
            else
            {
                roomToUse = room;
            }

            if (roomToUse == null)
            {
                return;
            }

            Vector2Int roomLocation = roomToUse.roomLocation;
            Vector2 roomWorldBottomLeftCellMiddleLocation = FloorGenerator.TransformMapToWorld(roomLocation, FloorGenerator.map.startRoom.roomLocation, FloorGenerator.cellSize);
            Vector2 roomWorldBottomLeftLocation = roomWorldBottomLeftCellMiddleLocation - FloorGenerator.cellSize / 2;
            Vector2 roomWorldTopRightCellMiddleLocation = FloorGenerator.TransformMapToWorld(roomLocation + roomToUse.roomType.sizeMultiplier - new Vector2Int(1, 1), FloorGenerator.map.startRoom.roomLocation, FloorGenerator.cellSize);
            Vector2 roomWorldTopRightLocation = roomWorldTopRightCellMiddleLocation - FloorGenerator.cellSize / 2 - Vector2.one;
            roomWorldTopRightLocation += FloorGenerator.cellSize;

            minPosition.x = roomWorldBottomLeftCellMiddleLocation.x - width / 2;
            minPosition.y = roomWorldBottomLeftLocation.y - extraHeight / 2 - 0.5f;
            maxPosition.x = roomWorldTopRightCellMiddleLocation.x + width / 2;
            maxPosition.y = roomWorldTopRightLocation.y + extraHeight / 2 + 0.5f;
        }

        /// <summary>
        /// Changes the min and max of the camera and moves the camera into the room smoothly
        /// </summary>
        private void OnRoomChange()
        {
            if (FloorGenerator.currentRoom == null) { return; }
            DetermineMinAndMax();
            StartCoroutine(MoveCameraToRoom());
        }

        /// <summary>
        /// Moves the camera into a room
        /// </summary>
        /// <returns> IEnumerator so the camera doesn't try to move regularly; it waits for this to finish </returns>
        private IEnumerator MoveCameraToRoom()
        {
            snapping = true;

            Vector3 position = GetCameraPosition();

            Vector2 vector2Transform = new Vector2(transform.position.x, transform.position.y);
            Vector2 vector2Position = new Vector2(position.x, position.y);

            while ((vector2Transform - vector2Position).magnitude > Time.fixedDeltaTime * speed)
            {
                Vector2 offset = (vector2Position - vector2Transform).normalized * Time.fixedDeltaTime * speed;
                transform.position = new Vector3(offset.x + transform.position.x, offset.y + transform.position.y, position.z);
                vector2Transform = new Vector2(transform.position.x, transform.position.y);
                vector2Position = new Vector2(position.x, position.y);
                yield return null;
            }

            snapping = false;
        }
    }
}
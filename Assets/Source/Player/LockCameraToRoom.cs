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

        // A reference to the floor generator
        private FloorGenerator floorGenerator;

        // A reference to the player
        private GameObject player;

        // Whether or not the camera is currently snapping between rooms
        private bool snapping;

        // The minimum position of the camera
        private Vector2 minPosition;

        // The maximum position of the camera
        private Vector2 maxPosition;

        /// <summary>
        /// Initializes the height, floor generator, and player references
        /// </summary>
        void Start()
        {
            Vector2 roomScale = FloorGenerator.floorGeneratorInstance.cellSize;

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

            floorGenerator = FloorGenerator.floorGeneratorInstance;
            floorGenerator.onRoomChange.AddListener(OnRoomChange);
            player = Player.Get();

            DetermineMinAndMax();
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        void FixedUpdate()
        {
            if (floorGenerator.currentRoom == null)
            {
                return;
            }

            /*if (floorGenerator.currentRoom.roomType == RoomType.Boss)
            {
                Vector3 playerPos = player.transform.position;
                newPosition = new Vector3();
                Vector2Int bossLocation = floorGenerator.currentRoom.roomLocation;
                Vector2 bossWorldBottomLeftLocation = FloorGenerator.TransformMapToWorld(bossLocation, floorGenerator.map.startCell.location, floorGenerator.roomSize) - floorGenerator.roomSize / 2;
                Vector2 bossWorldTopRightLocation = FloorGenerator.TransformMapToWorld(bossLocation + new Vector2Int(2, 2), floorGenerator.map.startCell.location, floorGenerator.roomSize) - floorGenerator.roomSize / 2 - Vector2.one;
                bossWorldTopRightLocation = bossWorldTopRightLocation + floorGenerator.roomSize;

                if (playerPos.x - height * GetComponent<Camera>().aspect / 2 < bossWorldBottomLeftLocation.x - extraHeight * GetComponent<Camera>().aspect / 2 + 0.5f)
                {
                    newPosition.x = bossWorldBottomLeftLocation.x + (height - extraHeight) * GetComponent<Camera>().aspect / 2 + 0.5f;
                }
                else if (playerPos.x + height * GetComponent<Camera>().aspect / 2 > bossWorldTopRightLocation.x + extraHeight * GetComponent<Camera>().aspect / 2 - 0.5f)
                {
                    newPosition.x = bossWorldTopRightLocation.x - (height - extraHeight) * GetComponent<Camera>().aspect / 2 - 0.5f;
                }
                else
                {
                    newPosition.x = playerPos.x;
                }

                if (playerPos.y - height / 2 < bossWorldBottomLeftLocation.y - extraHeight + 0.5f)
                {
                    newPosition.y = bossWorldBottomLeftLocation.y + (height - extraHeight) / 2 - 0.5f;
                }
                else if (playerPos.y + height / 2 > bossWorldTopRightLocation.y + extraHeight - 0.5f)
                {
                    newPosition.y = bossWorldTopRightLocation.y - (height - extraHeight) / 2 + 0.5f;
                }
                else
                {
                    newPosition.y = playerPos.y;
                }

                newPosition.z = -1;
            }*/
            //else
            //{
                //Vector2 pos2D = FloorGenerator.TransformMapToWorld(floorGenerator.currentRoom.roomLocation, floorGenerator.map.startRoom.roomLocation, floorGenerator.cellSize);
                //newPosition = new Vector3(pos2D.x, pos2D.y, -1);
            //}

            if (!snapping)
            {
                transform.position = GetCameraPosition();
            }
            //transform.position = Vector3.Lerp(transform.position, newPosition, Time.fixedDeltaTime * speed);
        }

        /// <summary>
        /// Gets the camera position for this frame
        /// </summary>
        /// <returns></returns>
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
        private void DetermineMinAndMax()
        {
            Vector2Int roomLocation = floorGenerator.currentRoom.roomLocation;
            Vector2 roomWorldBottomLeftCellMiddleLocation = FloorGenerator.TransformMapToWorld(roomLocation, floorGenerator.map.startRoom.roomLocation, floorGenerator.cellSize);
            Vector2 roomWorldBottomLeftLocation = roomWorldBottomLeftCellMiddleLocation - floorGenerator.cellSize / 2;
            Vector2 roomWorldTopRightCellMiddleLocation = FloorGenerator.TransformMapToWorld(roomLocation + floorGenerator.currentRoom.roomType.sizeMultiplier - new Vector2Int(1, 1), floorGenerator.map.startRoom.roomLocation, floorGenerator.cellSize);
            Vector2 roomWorldTopRightLocation = roomWorldTopRightCellMiddleLocation - floorGenerator.cellSize / 2 - Vector2.one;
            roomWorldTopRightLocation += floorGenerator.cellSize;

            minPosition.x = roomWorldBottomLeftCellMiddleLocation.x - width / 2;
            minPosition.y = roomWorldBottomLeftLocation.y - extraHeight / 2 - 0.5f;
            maxPosition.x = roomWorldTopRightCellMiddleLocation.x + width / 2;
            maxPosition.y = roomWorldTopRightLocation.y + extraHeight / 2 + 0.5f;

            Debug.Log("min position: " + minPosition);
            Debug.Log("Max position: " + maxPosition);
            Debug.Log("room world bottom left loctation: " + roomWorldBottomLeftLocation);
            Debug.Log("room world top left location: " + roomWorldTopRightLocation);
        }

        /// <summary>
        /// Changes the min and max of the camera and moves the camera into the room smoothly
        /// </summary>
        private void OnRoomChange()
        {
            DetermineMinAndMax();
        }
    }
}
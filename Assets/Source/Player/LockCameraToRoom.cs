using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cause the camera to follow the actor and snap to the center of rooms.
/// </summary>
public class LockCameraToRoom : MonoBehaviour
{
    [Tooltip("The speed at which the camera snaps.")]
    public float speed = 10;

    [Tooltip("The extra height that is added onto the size of the rooms")]
    public float extraHeight;

    // The height of the camera
    private float height;

    // A reference to the floor generator
    private FloorGenerator floorGenerator;

    // A reference to the player
    GameObject player;

    /// <summary>
    /// Initializes the height, floor generator, and player references
    /// </summary>
    void Start()
    {
        Vector2 roomScale = FloorGenerator.floorGeneratorInstance.roomSize;

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
        floorGenerator = FloorGenerator.floorGeneratorInstance;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    void Update()
    {
        Vector3 newPosition;

        if (floorGenerator.currentRoom == null)
        {
            return;
        }

        if (floorGenerator.currentRoom.roomType == RoomType.Boss)
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
        }
        else
        {
            Vector2 pos2D = FloorGenerator.TransformMapToWorld(floorGenerator.currentRoom.roomLocation, floorGenerator.map.startCell.location, floorGenerator.roomSize);
            newPosition = new Vector3(pos2D.x, pos2D.y, -1);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
    }
}

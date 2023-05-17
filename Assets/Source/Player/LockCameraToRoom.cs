using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cause the camera to follow the actor and snap to the center of rooms.
/// </summary>
public class LockCameraToRoom : MonoBehaviour
{
    // The size in units of the rooms.
    public Vector2 roomScale = new Vector2(10, 10);
    // The speed at which the camera snaps.
    public float speed = 10;

    // The size of a cell
    Vector2 cellSize;

    // The height of the camera
    float height;
    
    // player
    GameObject player;

    // Gets the room scale
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cellSize = ProceduralGeneration.proceduralGenerationInstance.cellSize;
        roomScale = cellSize * ProceduralGeneration.proceduralGenerationInstance.roomSize;
        if (roomScale.y > roomScale.x * (1 / GetComponent<Camera>().aspect))
        {
            height = roomScale.y;
        }
        else
        {
            height = roomScale.x * (1 / GetComponent<Camera>().aspect);
        }

        GetComponent<Camera>().orthographicSize = height / 2;
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    void Update()
    {
        var playerPos = player.transform.position; 
        Vector3 newPosition = new Vector3(Mathf.Round(playerPos.x / roomScale.x) * roomScale.x + ((roomScale.x % 2) * (cellSize.y / 2)), Mathf.Round(playerPos.y / roomScale.y) * roomScale.y + ((roomScale.y % 2) * (cellSize.y / 2)), -1);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
    }
}

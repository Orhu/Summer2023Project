using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cause the camera to follow the player and snap to the center of rooms.
/// </summary>
public class LockCameraToRoom : MonoBehaviour
{
    // The size in units of the rooms.
    public Vector2 roomScale = new Vector2(10, 10);
    // The speed at which the camera snaps.
    public float speed = 10;

    /// <summary>
    /// Updates the position.
    /// </summary>
    void Update()
    {
        Vector3 parentPosition = Player._instance.transform.position;
        Vector3 newPosition = new Vector3(Mathf.Round(parentPosition.x / roomScale.x) * roomScale.x, Mathf.Round(parentPosition.y / roomScale.y) * roomScale.y, -10);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
    }
}

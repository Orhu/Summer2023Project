using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraToRoom : MonoBehaviour
{
    public Vector2 roomScale = new Vector2(1000, 1000);
    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(Mathf.Floor(transform.position.x / roomScale.x), Mathf.Floor(transform.position.y / roomScale.y), -10);
        Debug.Log(newPosition);
        transform.SetPositionAndRotation(newPosition, Quaternion.identity);
    }
}

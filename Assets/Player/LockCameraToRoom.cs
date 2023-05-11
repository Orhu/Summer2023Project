using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraToRoom : MonoBehaviour
{
    public Vector2 roomScale = new Vector2(10, 10);
    public float speed = 10;
    // Update is called once per frame
    void Update()
    {
        Vector3 parentPosition = Player._instance.transform.position;
        Vector3 newPosition = new Vector3(Mathf.Round(parentPosition.x / roomScale.x) * roomScale.x, Mathf.Round(parentPosition.y / roomScale.y) * roomScale.y, -10);
        Debug.Log(newPosition + " <-- " + parentPosition / roomScale);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
    }
}

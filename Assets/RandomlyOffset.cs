using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomlyOffset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Bounds bounds = GetComponentInParent<Renderer>().bounds;
        transform.position = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y));
    }
}

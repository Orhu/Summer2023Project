using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [Tooltip("The amount of points that a room can spend on this layer")]
    public int points;

    [Tooltip("The tiles that belong to this layer")]
    public List<GameObject> tiles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

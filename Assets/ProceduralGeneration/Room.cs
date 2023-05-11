using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField] GameObject tilemap;
    [SerializeField] Vector2 size = new Vector2(11, 11);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetSize()
    {
        return size;
    }

    public GameObject GetTilemap()
    {
        return tilemap;
    }
}

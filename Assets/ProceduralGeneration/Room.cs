using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField] GameObject tilemap;
    [SerializeField] Vector2 size = new Vector2(11, 11);

    bool generated = false;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRoom();
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

    public void GenerateRoom()
    {
        if (generated)
        {
            return;
        }
        generated = true;
        RoomGenerationParameters roomParams = ProceduralGeneration.proceduralGenerationInstance.GetRoomGenerationParameters();
        Debug.Log("Generating Room");
        for (int i = 0; i < roomParams.numEnemies; i++)
        {
            Vector2 location = new Vector2(transform.position.x + (size.x / 2), transform.position.y + (size.y / 2));
            Debug.Log(location);
            Instantiate(roomParams.enemy, location, Quaternion.identity);
        }
    }
}

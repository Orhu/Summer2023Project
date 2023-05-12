using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    [SerializeField] GameObject tilemap;
    [SerializeField] Vector2 size = new Vector2(11, 11);

    BoxCollider2D roomBox;

    Vector2 location;
    bool generated = false;

    public void Copy(Room room)
    {
        tilemap = room.tilemap;
        size = room.size;
        location = room.location;
    }

    // Start is called before the first frame update
    void Start()
    {
        roomBox = gameObject.AddComponent<BoxCollider2D>();
        roomBox.size = new Vector3(size.x, size.y, 0);
        Debug.Log(roomBox.size);
        roomBox.isTrigger = true;
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
        for (int i = 0; i < roomParams.numEnemies; i++)
        {
            Vector2 enemyLocation = new Vector2(location.x + Random.Range(-1, 1), location.y + Random.Range(-1, 1));
            GameObject newEnemy = Instantiate(roomParams.enemy, enemyLocation, Quaternion.identity);
            newEnemy.SetActive(true);
        }
    }

    public Vector2 GetLocation()
    {
        return location;
    }

    public void SetLocation(Vector2 newLocation)
    {
        location = newLocation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collid");
        if (collision.gameObject == Player._instance)
        {
            GenerateRoom();
        }
    }
}

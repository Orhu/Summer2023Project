using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{

    [SerializeField] public GameObject tilemap;
    [SerializeField] public Vector2 size = new Vector2(11, 11);

    BoxCollider2D roomBox;

    bool generated = false;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = Instantiate(tilemap, transform);
        tilemap.SetActive(true);
        roomBox = gameObject.AddComponent<BoxCollider2D>();
        roomBox.transform.parent = this.transform;
        roomBox.transform.position = this.transform.position;
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
            Vector2 enemyLocation = new Vector2(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1));
            GameObject newEnemy = Instantiate(roomParams.enemy, enemyLocation, Quaternion.identity);
            newEnemy.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player._instance)
        {
            GenerateRoom();
        }
    }
}

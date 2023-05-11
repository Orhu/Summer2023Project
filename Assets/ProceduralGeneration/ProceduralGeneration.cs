using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] Vector2 mapSize;

    [SerializeField] List<Room> rooms;

    [SerializeField] RoomGenerationParameters roomGenerationParameters;

    public static ProceduralGeneration proceduralGenerationInstance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (proceduralGenerationInstance != null && proceduralGenerationInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        proceduralGenerationInstance = this;
    }

    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Room room = rooms[Random.Range(0, rooms.Count - 1)];
                Vector2 location;
                Vector2 tileSize;
                tileSize.x = room.GetTilemap().GetComponent<Grid>().cellSize.x;
                tileSize.y = room.GetTilemap().GetComponent<Grid>().cellSize.y;
                location.x = i * tileSize.x * room.GetSize().x;
                location.y = j * tileSize.y * room.GetSize().y;
                CreateRoom(room, location);
            }
        }
    }

    void CreateRoom(Room room, Vector2 location)
    {
        GameObject createdRoom = Instantiate(room.GetTilemap(), location, Quaternion.identity);
        createdRoom.SetActive(true);
        createdRoom.transform.parent = this.transform;
    }

    public RoomGenerationParameters GetRoomGenerationParameters()
    {
        return roomGenerationParameters;
    }

    public void AddRoomGenerationParameters(RoomGenerationParameters addedRoomParams)
    {
        roomGenerationParameters.Add(addedRoomParams);
    }
}

[System.Serializable]
public class RoomGenerationParameters
{
    [SerializeField] public int numEnemies;
    [SerializeField] public GameObject enemy;

    public void Add(RoomGenerationParameters other)
    {
        numEnemies += other.numEnemies;
    }

};
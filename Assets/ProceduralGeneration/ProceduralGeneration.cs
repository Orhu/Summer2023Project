using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] Vector2 mapSize;

    [SerializeField] List<GameObject> possibleTilemaps;

    [SerializeField] RoomGenerationParameters roomGenerationParameters;

    [SerializeField] GameObject room;

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
                GameObject tilemap = possibleTilemaps[Random.Range(0, possibleTilemaps.Count - 1)];
                Vector2 location;
                Vector2 tileSize;
                tileSize.x = tilemap.GetComponent<Grid>().cellSize.x;
                tileSize.y = tilemap.GetComponent<Grid>().cellSize.y;
                location.x = i * tileSize.x * room.GetComponent<Room>().GetSize().x;
                location.y = j * tileSize.y * room.GetComponent<Room>().GetSize().y;
                CreateRoom(tilemap, location);
            }
        }
    }

    void CreateRoom(GameObject tilemap, Vector2 location)
    {
        GameObject newRoom = Instantiate(room, location, Quaternion.identity);
        newRoom.transform.parent = this.transform;
        newRoom.GetComponent<Room>().tilemap = tilemap;
        newRoom.GetComponent<Room>().size = room.GetComponent<Room>().size;
        newRoom.SetActive(true);

        //Room newRoom = this.gameObject.AddComponent<Room>();
        //newRoom.Copy(room);
        //newRoom.SetLocation(location);
        //GameObject createdTilemap = Instantiate(newRoom.GetTilemap(), location, Quaternion.identity);
        //createdTilemap.SetActive(true);
        //createdTilemap.transform.parent = this.transform;
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
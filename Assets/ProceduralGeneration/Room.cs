using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The room, that handles enemy and trap generation when entered
/// </summary>
public class Room : MonoBehaviour
{
    // The tile map of this room; defines the shape of this room.
    // TODO: Make this determined by the room size and a bitmap of directions
    //[SerializeField] public GameObject tilemap;
    Tilemap tilemap;

    // The dimensions of this room
    public Vector2Int size = new Vector2Int(11, 11);

    // The size of the tiles
    // TODO: Actually implement this
    public Vector2 tilesize = new Vector2(1, 1);

    // The tile to use to create the walls
    // TODO: change this to actually use art, also make it so collider maps only generate for walls or whatever
    [SerializeField] public TileBase tile;

    // The collider that detects when this room has been entered
    BoxCollider2D roomBox;

    // Whether or not this room has been generated
    bool generated = false;

    /// <summary>
    /// Initializes the collision and the tilemap
    /// </summary>
    void Start()
    {
        CreateTilemap();

        roomBox = gameObject.AddComponent<BoxCollider2D>();
        roomBox.transform.parent = this.transform;
        roomBox.transform.position = this.transform.position;
        roomBox.size = new Vector3(size.x, size.y, 0);
        roomBox.isTrigger = true;
    }

    /// <summary>
    /// Creates a tilemap with the size of the room
    /// </summary>
    void CreateTilemap()
    {
        gameObject.AddComponent<Grid>();
        tilemap = gameObject.AddComponent<Tilemap>();

        Vector2Int endOffset = new Vector2Int();
        if (size.x % 2 == 0)
        {
            endOffset.x = size.x / 2 - 1;
        }
        else
        {
            endOffset.x = size.x / 2;
        }

        if (size.y % 2 == 0)
        {
            endOffset.y = size.y / 2 - 1;
        }
        else
        {
            endOffset.y = size.y / 2;
        }

        for (int x = 0; x < size.x; x++)
        {
            if (!ShouldBeDoor(new Vector2Int(x, 0)))
            {
                tilemap.SetTile(new Vector3Int(x - (size.x / 2), -(size.y / 2), 0), tile);
            }
            if (!ShouldBeDoor(new Vector2Int(x, size.y - 1)))
            {
                tilemap.SetTile(new Vector3Int(x - (size.x / 2), endOffset.y, 0), tile);
            }
        }

        for (int y = 1; y < size.y - 1; y++)
        {
            if (!ShouldBeDoor(new Vector2Int(0, y)))
            {
                tilemap.SetTile(new Vector3Int(-(size.x / 2), y - (size.y / 2), 0), tile);
            }
            if (!ShouldBeDoor(new Vector2Int(size.x - 1, y)))
            {
                tilemap.SetTile(new Vector3Int(endOffset.x, y - (size.y / 2), 0), tile);
            }
        }

        tilemap.CompressBounds();

        gameObject.AddComponent<TilemapCollider2D>();
        gameObject.AddComponent<TilemapRenderer>();
    }

    /// <summary>
    /// Checks whether the position on the tilemap should be a door
    /// </summary>
    /// <param name="pos"> The position on the tilemap to check </param>
    /// <returns></returns>
    bool ShouldBeDoor(Vector2Int pos)
    {
        if ((pos.x != 0 && pos.x != size.x - 1) && (pos.y != 0 && pos.y != size.y - 1))
        {
            return false;
        }

        if (pos.y == 0 || pos.y == size.y - 1)
        {
            if (size.x % 2 == 1)
            {
                if (pos.x == (size.x / 2))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (pos.x == ((size.x / 2) - 1) || pos.x == (size.x / 2))
                {
                    return true;
                }
                return false;
            }
        }

        if (size.y % 2 == 1)
        {
            if (pos.y == ((size.y / 2)))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (pos.y == ((size.y / 2) - 1) || pos.y == (size.y / 2))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Generates the enemies and traps and other things that appear in a room
    /// </summary>
    public void GenerateRoom()
    {
        // Don't generate if already generated
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

    /// <summary>
    /// Handles when the player enters the room
    /// </summary>
    /// <param name="collision"> The collider that entered the trigger </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player._instance.gameObject)
        {
            GenerateRoom();
        }
    }
}

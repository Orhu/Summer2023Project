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
    [SerializeField] public GameObject tilemap;

    // The dimensions of this room
    [SerializeField] public Vector2 size = new Vector2(11, 11);

    // The collider that detects when this room has been entered
    BoxCollider2D roomBox;

    // Whether or not this room has been generated
    bool generated = false;

    /// <summary>
    /// Initializes the collision and the tilemap
    /// </summary>
    void Start()
    {
        tilemap = Instantiate(tilemap, transform);
        tilemap.SetActive(true);

        roomBox = gameObject.AddComponent<BoxCollider2D>();
        roomBox.transform.parent = this.transform;
        roomBox.transform.position = this.transform.position;
        roomBox.size = new Vector3(size.x, size.y, 0);
        roomBox.isTrigger = true;
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

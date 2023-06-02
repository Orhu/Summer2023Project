using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates the exteriors of the rooms (walls, doors, and floors)
/// </summary>
public class RoomExteriorGenerator : MonoBehaviour
{
    /// <summary>
    /// Generates the exteriors of the rooms (walls, doors, and floors)
    /// </summary>
    /// <param name="roomTypesToExteriorParameters"> The parameters for each type of room </param>
    /// <param name="map"> The map to generate the rooms for </param>
    /// <param name="roomSize"> The size of a single room </param>
    public void Generate(RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorParameters, Map map, Vector2Int roomSize)
    {
        // Iterate over every room and create it
        for (int i = 0; i < map.mapSize.x; i++)
        {
            for (int j = 0; j < map.mapSize.y; j++)
            {
                if (map.map[i, j].type == RoomType.None)
                {
                    continue;
                }
                else if (map.map[i, j].type == RoomType.Boss)
                {
                    CreateBossRoom(map.map[i, j], roomTypesToExteriorParameters.At(RoomType.Boss), map, map.startCell, roomSize);
                }
                else
                {
                    CreateRoom(map.map[i, j], roomTypesToExteriorParameters.At(map.map[i, j].type), map.startCell, roomSize);
                }
            }
        }
    }

    /// <summary>
    /// Creates the room for the given cell
    /// </summary>
    /// <param name="createdCell"> The cell to create the room for </param>
    /// <param name="exteriorParameters"> The parameters for this type of room </param>
    /// <param name="startCell"> The starting cell (to get a reference for the midpoint of the map) </param>
    /// <param name="roomSize"> The size of a single room </param>
    private void CreateRoom(MapCell createdCell, RoomExteriorGenerationParameters exteriorParameters, MapCell startCell, Vector2Int roomSize)
    {
        // Instantiate the new room
        GameObject newRoom = new GameObject();
        createdCell.room = newRoom;
        newRoom.name = createdCell.type.ToString() + " Room " + createdCell.location.ToString();
        newRoom.transform.parent = transform;
        newRoom.transform.position = FloorGenerator.TransformMapToWorld(createdCell.location, startCell.location, roomSize);

        // Add the room component
        Room roomComponent = newRoom.AddComponent<Room>();
        roomComponent.roomLocation = createdCell.location;
        roomComponent.roomType = createdCell.type;
        roomComponent.roomSize = roomSize;
        roomComponent.roomGrid = new Tile[roomSize.x, roomSize.y];
        newRoom.AddComponent<TemplateGenerator>();

        // Add the box collider component
        BoxCollider2D roomBox = newRoom.AddComponent<BoxCollider2D>();
        roomBox.size = roomSize;
        roomBox.isTrigger = true;
        newRoom.layer = LayerMask.NameToLayer("RoomDetector");

        CreateWalls(createdCell, exteriorParameters, roomSize);

        //CreateDoors(newRoom, roomSize);

        //CreateFloor(newRoom, roomSize);
        
    }

    /// <summary>
    /// Initializes a tilemap (do not use for walls: Only things that will not have collision
    /// </summary>
    /// <param name="room"> The room to create the tilemap for </param>
    /// <param name="layer"> The layer the tilemap should have </param>
    /// <param name="name"> The name to give the tilemap </param>
    /// <returns> The tilemap </returns>
    private GameObject InitializeTilemap(GameObject room, int layer, string name)
    {
        GameObject tilemap = new GameObject();
        tilemap.name = name;
        tilemap.transform.parent = room.transform;
        tilemap.layer = layer;
        tilemap.AddComponent<Grid>();
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        // -0.5 to align the grid
        tilemap.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.0f);
        return tilemap;
    }

    /// <summary>
    /// Creates walls for a normally shaped room
    /// </summary>
    /// <param name="roomCell"> The room to create the walls for </param>
    /// <param name="exteriorParameters"> The parameters of this type of room </param>
    /// <param name="roomSize"> The size of a room </param>
    private void CreateWalls(MapCell roomCell, RoomExteriorGenerationParameters exteriorParameters, Vector2Int roomSize)
    {
        Room room = roomCell.room.GetComponent<Room>();

        GameObject wallContainer = new GameObject();
        wallContainer.transform.parent = roomCell.room.transform;
        wallContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);
        wallContainer.name = "Wall Container";

        // Add the corners
        Sprite randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[0, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, roomSize.y - 1), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, 0), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[roomSize.x - 1, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, roomSize.y - 1), wallContainer);

        // Add the top and bottom walls
        for (int i = 1; i < roomSize.x - 1; i++)
        {
            if (i >= roomSize.x / 2 - 1 && i <= roomSize.x / 2 + 1 && (roomCell.direction & Direction.Down) != Direction.None)
            {
                if (i != roomSize.x / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[i, 0] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(i, 0), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), wallContainer);
            }

            if (i >= roomSize.x / 2 - 1 && i <= roomSize.x / 2 + 1 && (roomCell.direction & Direction.Up) != Direction.None)
            {
                if (i != roomSize.x / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[i, roomSize.y - 1] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(i, roomSize.y - 1), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[i, roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, roomSize.y - 1), wallContainer);
            }
        }

        // Add the right and left walls
        for (int j = 1; j < roomSize.y - 1; j++)
        {
            if (j >= roomSize.y / 2 - 1 && j <= roomSize.y / 2 + 1 && (roomCell.direction & Direction.Left) != Direction.None)
            {
                if (j != roomSize.y / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[0, j] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(0, j), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), wallContainer);
            }

            if (j >= roomSize.y / 2 - 1 && j <= roomSize.y / 2 + 1 && (roomCell.direction & Direction.Right) != Direction.None)
            {
                if (j != roomSize.y / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[roomSize.x - 1, j] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(roomSize.x - 1, j), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[roomSize.x - 1, j] = CreateWallTile(randomWallSprite, new Vector2Int(roomSize.x - 1, j), wallContainer);
            }
        }
    }

    /// <summary>
    /// Creates a empty tile that has the given sprite, then gives it some collision
    /// </summary>
    /// <param name="sprite"> The wall sprite </param>
    /// <param name="location"> The location of the tile (in the wall container frame) </param>
    /// <param name="wallContainer"> The game object container that holds all the walls </param>
    /// <returns> The created wall </returns>
    private Tile CreateWallTile(Sprite sprite, Vector2Int location, GameObject wallContainer)
    {
        Tile tile = new Tile();
        tile.spawnedObject = new GameObject();
        tile.spawnedObject.transform.parent = wallContainer.transform;
        tile.spawnedObject.transform.localPosition = new Vector3(location.x, location.y, 0);
        tile.spawnedObject.AddComponent<BoxCollider2D>().size = new Vector2(1, 1);
        tile.spawnedObject.AddComponent<SpriteRenderer>().sprite = sprite;
        tile.spawnedObject.SetActive(true);
        tile.spawnedObject.name = "Wall";

        // TODO: Set tile cost

        tile.gridLocation = location;
        tile.walkable = false;

        // Set the type as blocker for no particular reason
        tile.type = TileType.Blocker;

        return tile;
    }

    /// <summary>
    /// Creates walls, doors, and floors for a boss room (which is not normally sized)
    /// </summary>
    /// <param name="createdCell"> One of the cells that should be part of the boss room </param>
    /// <param name="exteriorParameters"> The parameters for the boss room </param>
    /// <param name="map"> The map </param>
    /// <param name="startCell"> The start cell </param>
    /// <param name="roomSize"> The size of a (regular sized) room </param>
    private void CreateBossRoom(MapCell createdCell, RoomExteriorGenerationParameters exteriorParameters, Map map, MapCell startCell, Vector2Int roomSize)
    {
        if (createdCell.room != null)
        {
            return;
        }


        Vector2Int offsetFromCenter = new Vector2Int();
        offsetFromCenter.x = System.Convert.ToInt32(map.map[createdCell.location.x + 1, createdCell.location.y].type != RoomType.Boss);
        offsetFromCenter.x -= System.Convert.ToInt32(map.map[createdCell.location.x - 1, createdCell.location.y].type != RoomType.Boss);
        offsetFromCenter.y = System.Convert.ToInt32(map.map[createdCell.location.x, createdCell.location.y + 1].type != RoomType.Boss);
        offsetFromCenter.y -= System.Convert.ToInt32(map.map[createdCell.location.x, createdCell.location.y - 1].type != RoomType.Boss);

        MapCell centerCell = map.map[createdCell.location.x - offsetFromCenter.x, createdCell.location.y - offsetFromCenter.y];
        GameObject bossRoom = new GameObject();

        bossRoom.transform.parent = transform;
        bossRoom.name = createdCell.type.ToString() + " Room " + createdCell.location.ToString();
        bossRoom.transform.parent = transform;
        bossRoom.transform.position = FloorGenerator.TransformMapToWorld(centerCell.location, startCell.location, roomSize);

        Room room = bossRoom.AddComponent<Room>();
        room.roomLocation = centerCell.location - new Vector2Int(1, 1);
        room.roomType = RoomType.Boss;
        room.roomSize = roomSize * 3;
        room.roomGrid = new Tile[room.roomSize.x, room.roomSize.y];
        bossRoom.AddComponent<TemplateGenerator>();

        // Add the box collider component
        BoxCollider2D roomBox = bossRoom.AddComponent<BoxCollider2D>();
        roomBox.size = room.roomSize;
        roomBox.isTrigger = true;

        GameObject wallContainer = new GameObject();
        wallContainer.transform.parent = bossRoom.transform;
        wallContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);
        wallContainer.name = "Wall Container";

        // Add the corners
        Sprite randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[0, room.roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, room.roomSize.y - 1), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[room.roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(room.roomSize.x - 1, 0), wallContainer);

        randomCornerSprite = exteriorParameters.wallCornerSprites[Random.Range(0, exteriorParameters.wallCornerSprites.Count)];
        room.roomGrid[room.roomSize.x - 1, room.roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(room.roomSize.x - 1, room.roomSize.y - 1), wallContainer);

        // Add the top and bottom walls
        for (int i = 1; i < room.roomSize.x - 1; i++)
        {
            if (i >= room.roomSize.x / 2 - 1 && i <= room.roomSize.x / 2 + 1 && (map.map[centerCell.location.x, centerCell.location.y - 1].direction & Direction.Down) != Direction.None)
            {
                if (i != room.roomSize.x / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[i, 0] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(i, 0), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), wallContainer);
            }

            if (i >= room.roomSize.x / 2 - 1 && i <= room.roomSize.x / 2 + 1 && (map.map[centerCell.location.x, centerCell.location.y + 1].direction & Direction.Up) != Direction.None)
            {
                if (i != room.roomSize.x / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[i, room.roomSize.y - 1] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(i, room.roomSize.y - 1), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[i, room.roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, room.roomSize.y - 1), wallContainer);
            }
        }

        // Add the right and left walls
        for (int j = 1; j < room.roomSize.y - 1; j++)
        {
            if (j >= room.roomSize.y / 2 - 1 && j <= room.roomSize.y / 2 + 1 && (map.map[centerCell.location.x - 1, centerCell.location.y].direction & Direction.Left) != Direction.None)
            {
                if (j != room.roomSize.y / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[0, j] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(0, j), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), wallContainer);
            }

            if (j >= room.roomSize.y / 2 - 1 && j <= room.roomSize.y / 2 + 1 && (map.map[centerCell.location.x + 1, centerCell.location.y].direction & Direction.Right) != Direction.None)
            {
                if (j != room.roomSize.y / 2)
                {
                    Sprite randomNextToDoorSprite = exteriorParameters.nextToDoorSprites[Random.Range(0, exteriorParameters.nextToDoorSprites.Count)];
                    room.roomGrid[room.roomSize.x - 1, j] = CreateWallTile(randomNextToDoorSprite, new Vector2Int(room.roomSize.x -1 , j), wallContainer);
                }
            }
            else
            {
                Sprite randomWallSprite = exteriorParameters.wallSprites[Random.Range(0, exteriorParameters.wallSprites.Count)];
                room.roomGrid[room.roomSize.x - 1, j] = CreateWallTile(randomWallSprite, new Vector2Int(room.roomSize.x - 1, j), wallContainer);
            }
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                map.map[centerCell.location.x + i, centerCell.location.y + j].room = bossRoom;
            }
        }
    }
}

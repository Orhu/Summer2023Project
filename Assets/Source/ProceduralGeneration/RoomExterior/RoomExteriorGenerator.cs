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
                    CreateRoom(map.map[i, j], roomTypesToExteriorParameters.At(map.map[i, j].type), map, roomSize);
                }
            }
        }

        // Set the start room to active
        Room startRoom = map.startCell.room.GetComponent<Room>();

        for (int i = 0; i < startRoom.transform.childCount; i++)
        {
            startRoom.transform.GetChild(i).gameObject.SetActive(true);
        }

        FloorGenerator.floorGeneratorInstance.currentRoom = startRoom;

        startRoom.ActivateDoors();
    }

    /// <summary>
    /// Creates the room for the given cell
    /// </summary>
    /// <param name="createdCell"> The cell to create the room for </param>
    /// <param name="exteriorParameters"> The parameters for this type of room </param>
    /// <param name="map"> The map </param>
    /// <param name="roomSize"> The size of a single room </param>
    private void CreateRoom(MapCell createdCell, RoomExteriorGenerationParameters exteriorParameters, Map map, Vector2Int roomSize)
    {
        // Instantiate the new room
        GameObject newRoom = new GameObject();
        createdCell.room = newRoom;
        newRoom.name = createdCell.type.ToString() + " Room " + createdCell.location.ToString();
        newRoom.transform.parent = transform;
        newRoom.transform.position = FloorGenerator.TransformMapToWorld(createdCell.location, map.startCell.location, roomSize);

        // Add the room component
        Room roomComponent = newRoom.AddComponent<Room>();
        roomComponent.roomLocation = createdCell.location;
        roomComponent.roomType = createdCell.type;
        roomComponent.roomSize = roomSize;
        roomComponent.roomGrid = new Tile[roomSize.x, roomSize.y];
        roomComponent.livingEnemies = new List<GameObject>();
        newRoom.AddComponent<TemplateGenerator>();

        CreateWalls(createdCell, exteriorParameters, roomSize);

        CreateDoors(createdCell, map, roomSize);

        //CreateFloor(newRoom, roomSize);

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
        Sprite randomCornerSprite = exteriorParameters.bottomLeftWallCornerSprites[Random.Range(0, exteriorParameters.bottomLeftWallCornerSprites.Count)];
        room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), wallContainer);

        randomCornerSprite = exteriorParameters.topLeftWallCornerSprites[Random.Range(0, exteriorParameters.topLeftWallCornerSprites.Count)];
        room.roomGrid[0, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, roomSize.y - 1), wallContainer);

        randomCornerSprite = exteriorParameters.bottomRightWallCornerSprites[Random.Range(0, exteriorParameters.bottomRightWallCornerSprites.Count)];
        room.roomGrid[roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, 0), wallContainer);

        randomCornerSprite = exteriorParameters.topRightWallCornerSprites[Random.Range(0, exteriorParameters.topRightWallCornerSprites.Count)];
        room.roomGrid[roomSize.x - 1, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, roomSize.y - 1), wallContainer);

        // Add the top and bottom walls
        for (int i = 1; i < roomSize.x - 1; i++)
        {
            if (i != roomSize.x / 2 || (roomCell.direction & Direction.Down) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.bottomWallSprites[Random.Range(0, exteriorParameters.bottomWallSprites.Count)];
                room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), wallContainer);
            }

            if (i != roomSize.x / 2 || (roomCell.direction & Direction.Up) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.topWallSprites[Random.Range(0, exteriorParameters.topWallSprites.Count)];
                room.roomGrid[i, roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, roomSize.y - 1), wallContainer);
            }
        }

        // Add the right and left walls
        for (int j = 1; j < roomSize.y - 1; j++)
        {
            if (j != roomSize.y / 2 || (roomCell.direction & Direction.Left) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.leftWallSprites[Random.Range(0, exteriorParameters.leftWallSprites.Count)];
                room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), wallContainer);
            }

            if (j != roomSize.y / 2 || (roomCell.direction & Direction.Right) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.rightWallSprites[Random.Range(0, exteriorParameters.rightWallSprites.Count)];
                room.roomGrid[roomSize.x - 1, j] = CreateWallTile(randomWallSprite, new Vector2Int(roomSize.x - 1, j), wallContainer);
            }
        }

        wallContainer.SetActive(false);
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
        Tile tile = ScriptableObject.CreateInstance<Tile>();
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
    /// Creates doors for the room
    /// </summary>
    /// <param name="roomCell"> The room to create doors for </param>
    /// <param name="map"> The map </param>
    /// <param name="roomSize"> The size of the room </param>
    private void CreateDoors(MapCell roomCell, Map map, Vector2Int roomSize)
    {

        Room room = roomCell.room.GetComponent<Room>();

        GameObject doorContainer = new GameObject();
        doorContainer.name = "Door Container";
        doorContainer.transform.parent = roomCell.room.transform;
        doorContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);

        for (int i = 1; i <= (int) Direction.Down; i *= 2)
        {
            if ((roomCell.direction & (Direction) i) != Direction.None)
            {
                Vector2Int mapOffset = new Vector2Int();
                mapOffset.x = System.Convert.ToInt32(((Direction) i & Direction.Right) != Direction.None) - System.Convert.ToInt32(((Direction) i & Direction.Left) != Direction.None);
                mapOffset.y = System.Convert.ToInt32(((Direction) i & Direction.Up) != Direction.None) - System.Convert.ToInt32(((Direction) i & Direction.Down) != Direction.None);

                MapCell connectedCell = map.map[roomCell.location.x + mapOffset.x, roomCell.location.y + mapOffset.y];
                RoomExteriorGenerationParameters exteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.type);

                Vector2Int doorLocation = (mapOffset * roomSize / 2) + roomSize / 2;

                DoorSprites doorSprites = new DoorSprites();

                if (((Direction) i & Direction.Right) != Direction.None)
                {
                    doorSprites = exteriorParameters.rightDoorSprites[Random.Range(0, exteriorParameters.rightDoorSprites.Count)];
                }

                if (((Direction) i & Direction.Up) != Direction.None)
                {
                    doorSprites = exteriorParameters.topDoorSprites[Random.Range(0, exteriorParameters.topDoorSprites.Count)];
                }

                if (((Direction) i & Direction.Left) != Direction.None)
                {
                    doorSprites = exteriorParameters.leftDoorSprites[Random.Range(0, exteriorParameters.leftDoorSprites.Count)];
                }

                if (((Direction) i & Direction.Down) != Direction.None)
                {
                    doorSprites = exteriorParameters.bottomDoorSprites[Random.Range(0, exteriorParameters.bottomDoorSprites.Count)];
                }

                room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, (Direction) i, connectedCell, doorContainer);
                room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
            }
        }

        doorContainer.SetActive(false);

        room.OpenDoors();
        room.DeactivateDoors();
    }

    /// <summary>
    /// Creates a door tile
    /// </summary>
    /// <param name="doorSprites"> The sprites of the door </param>
    /// <param name="location"> The location in the room of the door </param>
    /// <param name="direction"> The direction the door is facing </param>
    /// <param name="connectedCell"> The room the door is connected to </param>
    /// <param name="doorContainer"> The container that holds the door tiles </param>
    /// <returns> The created tile </returns>
    private Tile CreateDoorTile(DoorSprites doorSprites, Vector2Int location, Direction direction, MapCell connectedCell, GameObject doorContainer)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.spawnedObject = new GameObject();
        tile.spawnedObject.transform.parent = doorContainer.transform;
        tile.spawnedObject.transform.localPosition = new Vector3(location.x, location.y, 0);
        BoxCollider2D doorCollision = tile.spawnedObject.AddComponent<BoxCollider2D>();
        doorCollision.size = new Vector2(1, 1);
        //doorCollision.isTrigger = true;
        tile.spawnedObject.AddComponent<SpriteRenderer>().sprite = doorSprites.doorOpened;
        Door door = tile.spawnedObject.AddComponent<Door>();
        door.doorSprites = doorSprites;
        door.connectedCell = connectedCell;
        door.direction = direction;
        tile.spawnedObject.SetActive(true);
        tile.spawnedObject.name = "Door";

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

        GameObject wallContainer = new GameObject();
        wallContainer.transform.parent = bossRoom.transform;
        wallContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);
        wallContainer.name = "Wall Container";

        // Add the corners
        Sprite randomCornerSprite = exteriorParameters.bottomLeftWallCornerSprites[Random.Range(0, exteriorParameters.bottomLeftWallCornerSprites.Count)];
        room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), wallContainer);

        randomCornerSprite = exteriorParameters.topLeftWallCornerSprites[Random.Range(0, exteriorParameters.topLeftWallCornerSprites.Count)];
        room.roomGrid[0, room.roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, room.roomSize.y - 1), wallContainer);

        randomCornerSprite = exteriorParameters.bottomRightWallCornerSprites[Random.Range(0, exteriorParameters.bottomRightWallCornerSprites.Count)];
        room.roomGrid[room.roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(room.roomSize.x - 1, 0), wallContainer);

        randomCornerSprite = exteriorParameters.topRightWallCornerSprites[Random.Range(0, exteriorParameters.topRightWallCornerSprites.Count)];
        room.roomGrid[room.roomSize.x - 1, room.roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(room.roomSize.x - 1, room.roomSize.y - 1), wallContainer);

        // Add the top and bottom walls
        for (int i = 1; i < room.roomSize.x - 1; i++)
        {
            if (i != roomSize.x / 2 || (map.map[centerCell.location.x, centerCell.location.y - 1].direction & Direction.Down) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.bottomWallSprites[Random.Range(0, exteriorParameters.bottomWallSprites.Count)];
                room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), wallContainer);
            }

            if (i != room.roomSize.x / 2 || (map.map[centerCell.location.x, centerCell.location.y + 1].direction & Direction.Up) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.topWallSprites[Random.Range(0, exteriorParameters.topWallSprites.Count)];
                room.roomGrid[i, room.roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, room.roomSize.y - 1), wallContainer);
            }
        }

        // Add the right and left walls
        for (int j = 1; j < room.roomSize.y - 1; j++)
        {
            if (j != room.roomSize.y / 2 || (map.map[centerCell.location.x - 1, centerCell.location.y].direction & Direction.Left) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.leftWallSprites[Random.Range(0, exteriorParameters.leftWallSprites.Count)];
                room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), wallContainer);
            }

            if (j != room.roomSize.y / 2 || (map.map[centerCell.location.x + 1, centerCell.location.y].direction & Direction.Right) == Direction.None)
            {
                Sprite randomWallSprite = exteriorParameters.topWallSprites[Random.Range(0, exteriorParameters.topWallSprites.Count)];
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

        wallContainer.SetActive(false);

        GameObject doorContainer = new GameObject();
        doorContainer.name = "Door Container";
        doorContainer.transform.parent = bossRoom.transform;
        doorContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);

        // liam plz don't kill me i know this is terrible, i'll fix it after prototype

        if ((map.map[centerCell.location.x + 1, centerCell.location.y].direction & Direction.Right) != Direction.None)
        {
            Vector2Int mapOffset = new Vector2Int(2, 0);

            MapCell connectedCell = map.map[centerCell.location.x + mapOffset.x, centerCell.location.y + mapOffset.y];
            RoomExteriorGenerationParameters doorExteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.type);

            Vector2Int doorLocation = ((mapOffset / 2) * (room.roomSize / 2)) + room.roomSize / 2;;

            DoorSprites doorSprites = new DoorSprites();

            doorSprites = doorExteriorParameters.rightDoorSprites[Random.Range(0, exteriorParameters.rightDoorSprites.Count)];
            
            room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, Direction.Right, connectedCell, doorContainer);
            room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
        }

        if ((map.map[centerCell.location.x, centerCell.location.y + 1].direction & Direction.Up) != Direction.None)
        {
            Vector2Int mapOffset = new Vector2Int(0, 2);

            MapCell connectedCell = map.map[centerCell.location.x + mapOffset.x, centerCell.location.y + mapOffset.y];
            RoomExteriorGenerationParameters doorExteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.type);

            Vector2Int doorLocation = ((mapOffset / 2) * (room.roomSize / 2)) + room.roomSize / 2;

            DoorSprites doorSprites = new DoorSprites();

            doorSprites = doorExteriorParameters.topDoorSprites[Random.Range(0, exteriorParameters.topDoorSprites.Count)];

            room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, Direction.Up, connectedCell, doorContainer);
            room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
        }

        if ((map.map[centerCell.location.x - 1, centerCell.location.y].direction & Direction.Left) != Direction.None)
        {
            Vector2Int mapOffset = new Vector2Int(-2, 0);

            MapCell connectedCell = map.map[centerCell.location.x + mapOffset.x, centerCell.location.y + mapOffset.y];
            RoomExteriorGenerationParameters doorExteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.type);

            Vector2Int doorLocation = ((mapOffset / 2) * (room.roomSize / 2)) + room.roomSize / 2;

            DoorSprites doorSprites = new DoorSprites();

            doorSprites = doorExteriorParameters.leftDoorSprites[Random.Range(0, exteriorParameters.leftDoorSprites.Count)];

            room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, Direction.Left, connectedCell, doorContainer);
            room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
        }

        if ((map.map[centerCell.location.x, centerCell.location.y - 1].direction & Direction.Down) != Direction.None)
        {
            Vector2Int mapOffset = new Vector2Int(0, -2);

            MapCell connectedCell = map.map[centerCell.location.x + mapOffset.x, centerCell.location.y + mapOffset.y];
            RoomExteriorGenerationParameters doorExteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.type);

            Vector2Int doorLocation = ((mapOffset / 2) * (room.roomSize / 2)) + room.roomSize / 2;

            DoorSprites doorSprites = new DoorSprites();

            doorSprites = doorExteriorParameters.bottomDoorSprites[Random.Range(0, exteriorParameters.bottomDoorSprites.Count)];

            room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, Direction.Down, connectedCell, doorContainer);
            room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
        }

        doorContainer.SetActive(false);

        room.OpenDoors();
        room.DeactivateDoors();
    }

}

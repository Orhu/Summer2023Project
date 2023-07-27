using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Generates the exteriors of the rooms (walls, doors, and floors)
    /// </summary>
    public class RoomExteriorGenerator : MonoBehaviour
    {
        /// <summary>
        /// Generates the exteriors of the rooms (walls, doors, and floors)
        /// </summary>
        /// <param name="roomTypesToExteriorParams"> The Params for each type of room </param>
        /// <param name="map"> The map to generate the rooms for </param>
        /// <param name="cellSize"> The size of a single cell </param>
        public void Generate(RoomTypesToRoomExteriorParams roomTypesToExteriorParams, Map map, Vector2Int cellSize)
        {
            HashSet<Room> generatedRooms = new HashSet<Room>();
            // Iterate over every room and create it
            for (int i = 0; i < map.mapSize.x; i++)
            {
                for (int j = 0; j < map.mapSize.y; j++)
                {
                    if (map.map[i, j].room == null || generatedRooms.Contains(map.map[i, j].room))
                    {
                        continue;
                    }

                    generatedRooms.Add(map.map[i, j].room);
                    if (roomTypesToExteriorParams.Contains(map.map[i, j].room.roomType))
                    {
                        CreateRoomExterior(map.map[i, j].room, roomTypesToExteriorParams.At(map.map[i, j].room.roomType), roomTypesToExteriorParams.defaultRoomExteriorParams, roomTypesToExteriorParams.wallTile, roomTypesToExteriorParams.doorTile, roomTypesToExteriorParams.floorTile, map, cellSize);
                    }
                    else
                    {
                        CreateRoomExterior(map.map[i, j].room, roomTypesToExteriorParams.defaultRoomExteriorParams, roomTypesToExteriorParams.defaultRoomExteriorParams, roomTypesToExteriorParams.wallTile, roomTypesToExteriorParams.doorTile, roomTypesToExteriorParams.floorTile, map, cellSize);
                    }
                }
            }

            roomTypesToExteriorParams.Reset();
        }

        /// <summary>
        /// Creates the room for the given cell
        /// </summary>
        /// <param name="createdRoom"> The cell to create the room for </param>
        /// <param name="exteriorParams"> The Params for this type of room </param>
        /// <param name="map"> The map </param>
        /// <param name="cellSize"> The size of a single cell </param>
        private void CreateRoomExterior(Room createdRoom, RoomExteriorParams exteriorParams, RoomExteriorParams defaultRoomExteriorParams, Tile wallTile, Tile doorTile, GameObject floorTile, Map map, Vector2Int cellSize)
        {
            createdRoom.cellSize = cellSize;
            createdRoom.roomGrid = new Tile[createdRoom.roomSize.x, createdRoom.roomSize.y];

            CreateWalls(map, createdRoom, exteriorParams, defaultRoomExteriorParams, wallTile);

            CreateDoors(map, createdRoom, doorTile);

            CreateFloor(map, createdRoom, exteriorParams, defaultRoomExteriorParams, floorTile);
        }

        /// <summary>
        /// Gets a random exterior param that's valid (so you can leave params blank to go to the default version)
        /// </summary>
        /// <typeparam name="T"> The type of the exterior param </typeparam>
        /// <param name="desiredThings"> The desired params </param>
        /// <param name="defaultThings"> The default params if the desired things ends up being null </param>
        /// <returns> A random valid exterior param </returns>
        private T GetRandomValidExteriorParam<T>(GenericWeightedThings<T> desiredThings, GenericWeightedThings<T> defaultThings)
        {
            if (desiredThings.things == null || desiredThings.things.Count == 0)
            {
                return defaultThings.GetRandomThing(FloorGenerator.random);
            }
            return desiredThings.GetRandomThing(FloorGenerator.random);
        }

        /// <summary>
        /// Creates walls for a rectangular room
        /// </summary>
        /// <param name="map"> The map </param>
        /// <param name="room"> The room to create the walls for </param>
        /// <param name="exteriorParams"> The Params of this type of room </param>
        private void CreateWalls(Map map, Room room, RoomExteriorParams exteriorParams, RoomExteriorParams defaultRoomExteriorParams, Tile wallTile)
        {
            Vector2Int roomSize = room.roomSize;

            GameObject wallContainer = new GameObject();
            wallContainer.transform.parent = room.transform;
            wallContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);
            wallContainer.name = "Wall Container";

            // Add the corners
            Sprite randomCornerSprite = GetRandomValidExteriorParam(exteriorParams.bottomLeftCornerSprites, defaultRoomExteriorParams.bottomLeftCornerSprites);
            room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), wallTile, wallContainer);

            randomCornerSprite = GetRandomValidExteriorParam(exteriorParams.topLeftCornerSprites, defaultRoomExteriorParams.topLeftCornerSprites); 
            room.roomGrid[0, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, roomSize.y - 1), wallTile, wallContainer);

            randomCornerSprite = GetRandomValidExteriorParam(exteriorParams.bottomRightCornerSprites, defaultRoomExteriorParams.bottomRightCornerSprites);
            room.roomGrid[roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, 0), wallTile, wallContainer);

            randomCornerSprite = GetRandomValidExteriorParam(exteriorParams.topRightCornerSprites, defaultRoomExteriorParams.topRightCornerSprites);
            room.roomGrid[roomSize.x - 1, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, roomSize.y - 1), wallTile, wallContainer);

            // Add the top and bottom walls
            for (int i = 1; i < roomSize.x - 1; i++)
            {
                Sprite randomWallSprite = null;
                MapCell roomCell = map.map[(i / room.cellSize.x) + room.roomLocation.x, room.roomLocation.y];
                if ((i % room.cellSize.x == room.cellSize.x / 2 - 1) && roomCell.direction.HasFlag(Direction.Down))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.bottomDoorLeftDoorFrameSprites, defaultRoomExteriorParams.bottomDoorLeftDoorFrameSprites);
                }
                else if ((i % room.cellSize.x == room.cellSize.x / 2 + 1) && roomCell.direction.HasFlag(Direction.Down))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.bottomDoorRightDoorFrameSprites, defaultRoomExteriorParams.bottomDoorRightDoorFrameSprites);
                }
                else if (i % room.cellSize.x != room.cellSize.x / 2 || !roomCell.direction.HasFlag(Direction.Down))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.bottomWallSprites, defaultRoomExteriorParams.bottomWallSprites);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), wallTile, wallContainer);
                }

                randomWallSprite = null;
                roomCell = map.map[(i / room.cellSize.x) + room.roomLocation.x, room.roomType.sizeMultiplier.y + room.roomLocation.y - 1];
                if ((i % room.cellSize.x == room.cellSize.x / 2 - 1) && roomCell.direction.HasFlag(Direction.Up))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.topDoorLeftDoorFrameSprites, defaultRoomExteriorParams.topDoorLeftDoorFrameSprites);
                }
                else if ((i % room.cellSize.x == room.cellSize.x / 2 + 1) && roomCell.direction.HasFlag(Direction.Up))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.topDoorRightDoorFrameSprites, defaultRoomExteriorParams.topDoorRightDoorFrameSprites);
                }
                else if (i % room.cellSize.x != room.cellSize.x / 2 || !roomCell.direction.HasFlag(Direction.Up))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.topWallSprites, defaultRoomExteriorParams.topWallSprites);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[i, roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, roomSize.y - 1), wallTile, wallContainer);
                }
            }

            // Add the right and left walls
            for (int j = 1; j < roomSize.y - 1; j++)
            {
                Sprite randomWallSprite = null;
                MapCell roomCell = map.map[room.roomLocation.x, (j / room.cellSize.y) + room.roomLocation.y];
                if ((j % room.cellSize.y == room.cellSize.y / 2 - 1) && roomCell.direction.HasFlag(Direction.Left))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.leftDoorBottomDoorFrameSprites, defaultRoomExteriorParams.leftDoorBottomDoorFrameSprites);
                }
                else if ((j % room.cellSize.y == room.cellSize.y / 2 + 1) && roomCell.direction.HasFlag(Direction.Left))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.leftDoorTopDoorFrameSprites, defaultRoomExteriorParams.leftDoorTopDoorFrameSprites);
                }
                else if (j % room.cellSize.y != room.cellSize.y / 2 || !roomCell.direction.HasFlag(Direction.Left))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.leftWallSprites, defaultRoomExteriorParams.leftWallSprites);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), wallTile, wallContainer);
                }

                randomWallSprite = null;
                roomCell = map.map[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, (j / room.cellSize.y) + room.roomLocation.y];
                if ((j % room.cellSize.y == room.cellSize.y / 2 - 1) && roomCell.direction.HasFlag(Direction.Right))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.rightDoorBottomDoorFrameSprites, defaultRoomExteriorParams.rightDoorBottomDoorFrameSprites);
                }
                else if ((j % room.cellSize.y == room.cellSize.y / 2 + 1) && roomCell.direction.HasFlag(Direction.Right))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.rightDoorTopDoorFrameSprites, defaultRoomExteriorParams.rightDoorTopDoorFrameSprites);
                }
                else if (j % room.cellSize.y != room.cellSize.y / 2 || !roomCell.direction.HasFlag(Direction.Right))
                {
                    randomWallSprite = GetRandomValidExteriorParam(exteriorParams.rightWallSprites, defaultRoomExteriorParams.rightWallSprites);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[roomSize.x - 1, j] = CreateWallTile(randomWallSprite, new Vector2Int(roomSize.x - 1, j), wallTile, wallContainer);
                }
            }
            
            wallContainer.SetActive(false);
        }

        /// <summary>
        /// Creates a empty tile that has the given sprite, then gives it some collision
        /// </summary>
        /// <param name="sprite"> The wall sprite </param>
        /// <param name="location"> The location of the tile (in the wall container frame) </param>
        /// <param name="wallTile"> The tile to instantiate </param>
        /// <param name="wallContainer"> The game object container that holds all the walls </param>
        /// <returns> The created wall </returns>
        private Tile CreateWallTile(Sprite sprite, Vector2Int location, Tile wallTile, GameObject wallContainer)
        {
            Tile tile = Instantiate(wallTile.gameObject).GetComponent<Tile>();
            tile.transform.parent = wallContainer.transform;
            tile.transform.localPosition = new Vector3(location.x, location.y, 0);
            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                throw new System.Exception("The wall tile " + wallTile + " must have a sprite renderer!");
            }
            spriteRenderer.sprite = sprite;
            tile.gameObject.SetActive(true);
            tile.gridLocation = location;
            return tile;
        }

        /// <summary>
        /// Creates doors for the room
        /// </summary>
        /// <param name="map"> The map </param>
        /// <param name="room"> The room to create doors for </param>
        private void CreateDoors(Map map, Room room, Tile doorTile)
        {
            GameObject doorContainer = new GameObject();
            doorContainer.name = "Door Container";
            doorContainer.transform.parent = room.transform;
            doorContainer.transform.localPosition = new Vector3(-room.roomSize.x / 2, -room.roomSize.y / 2, 0);

            foreach (MapCell roomCell in room.GetEdgeCells(map.map))
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (direction == Direction.None || direction == Direction.All)
                    {
                        continue;
                    }

                    if (roomCell.direction.HasFlag(direction))
                    {
                        Vector2Int mapOffset = new Vector2Int();
                        mapOffset.x = System.Convert.ToInt32(direction == Direction.Right) - System.Convert.ToInt32(direction == Direction.Left);
                        mapOffset.y = System.Convert.ToInt32(direction == Direction.Up) - System.Convert.ToInt32(direction == Direction.Down);
                        MapCell connectedCell = map.map[roomCell.location.x + mapOffset.x, roomCell.location.y + mapOffset.y];

                        int currentRoomIndex = FloorGenerator.floorParams.exteriorParams.roomTypesToRoomExteriorParams.FindIndex(iRoomType => room.roomType == iRoomType.roomType);
                        int connectedRoomIndex = FloorGenerator.floorParams.exteriorParams.roomTypesToRoomExteriorParams.FindIndex(iRoomType => connectedCell.room.roomType == iRoomType.roomType);

                        RoomExteriorParams exteriorParams;
                        if (currentRoomIndex == -1 && connectedRoomIndex == -1)
                        {
                            exteriorParams = FloorGenerator.floorParams.exteriorParams.defaultRoomExteriorParams;
                        }
                        else
                        {
                            if (currentRoomIndex > connectedRoomIndex)
                            {
                                exteriorParams = FloorGenerator.floorParams.exteriorParams.At(room.roomType);
                            }
                            else
                            {
                                exteriorParams = FloorGenerator.floorParams.exteriorParams.At(connectedCell.room.roomType);
                            }
                        }

                        Vector2Int doorLocation = (mapOffset * room.cellSize / 2) + room.cellSize / 2 + (roomCell.location - room.roomLocation) * room.cellSize;

                        DoorSprites doorSprites = new DoorSprites();

                        if (direction == Direction.Right)
                        {
                            doorSprites = GetRandomValidExteriorParam(exteriorParams.rightDoorSprites, FloorGenerator.floorParams.exteriorParams.defaultRoomExteriorParams.rightDoorSprites);
                        }
                        else if (direction == Direction.Up)
                        {
                            doorSprites = GetRandomValidExteriorParam(exteriorParams.topDoorSprites, FloorGenerator.floorParams.exteriorParams.defaultRoomExteriorParams.topDoorSprites);
                        }
                        else if (direction == Direction.Left)
                        {
                            doorSprites = GetRandomValidExteriorParam(exteriorParams.leftDoorSprites, FloorGenerator.floorParams.exteriorParams.defaultRoomExteriorParams.leftDoorSprites);
                        }
                        else if (direction == Direction.Down)
                        {
                            doorSprites = GetRandomValidExteriorParam(exteriorParams.bottomDoorSprites, FloorGenerator.floorParams.exteriorParams.defaultRoomExteriorParams.bottomDoorSprites);
                        }

                        room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, direction, connectedCell, doorTile, doorContainer);
                        room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].GetComponent<Door>());
                    }
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
        /// <param name="doorTile"> The door tile to instantiate </param>
        /// <param name="doorContainer"> The container that holds the door tiles </param>
        /// <returns> The created tile </returns>
        private Tile CreateDoorTile(DoorSprites doorSprites, Vector2Int location, Direction direction, MapCell connectedCell, Tile doorTile, GameObject doorContainer)
        {
            Tile tile = Instantiate(doorTile.gameObject).GetComponent<Tile>();
            tile.transform.parent = doorContainer.transform;
            tile.transform.localPosition = new Vector3(location.x, location.y, 0);

            Door door = tile.GetComponent<Door>();
            if (door == null)
            {
                throw new System.Exception("The door tile must have a door component!");
            }
            door.Initialize(doorSprites, connectedCell, direction);

            tile.gameObject.SetActive(true);
            tile.gridLocation = location;

            return tile;
        }

        /// <summary>
        /// Creates a floor for a room
        /// </summary>
        /// <param name="map"> The map </param>
        /// <param name="room"> The room to create the floor for </param>
        /// <param name="exteriorParams"> The exterior generation Params of this room </param>
        private void CreateFloor(Map map, Room room, RoomExteriorParams exteriorParams, RoomExteriorParams defaultRoomExteriorParams, GameObject floorTile)
        {
            Vector2Int roomSize = room.roomSize;

            GameObject floorContainer = new GameObject("Floor");
            floorContainer.transform.parent = room.transform;
            floorContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);

            for (int i = 1; i < roomSize.x - 1; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    Sprite floorSprite = GetRandomValidExteriorParam(exteriorParams.floorSprites, defaultRoomExteriorParams.floorSprites);
                    CreateFloorTile(floorSprite, new Vector2Int(i, j), floorTile, floorContainer);
                }
            }

            // Add the tiles under the left and right doors if they exist

            for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
            {
                if (map.map[room.roomLocation.x, j + room.roomLocation.y].direction.HasFlag(Direction.Left))
                {
                    Sprite floorSprite = GetRandomValidExteriorParam(exteriorParams.floorSprites, defaultRoomExteriorParams.floorSprites);
                    CreateFloorTile(floorSprite, new Vector2Int(0, (room.cellSize.y * j) + room.cellSize.y / 2), floorTile, floorContainer);
                }
                if (map.map[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, j + room.roomLocation.y].direction.HasFlag(Direction.Right))
                {
                    Sprite floorSprite = GetRandomValidExteriorParam(exteriorParams.floorSprites, defaultRoomExteriorParams.floorSprites);
                    CreateFloorTile(floorSprite, new Vector2Int(roomSize.x - 1, (room.cellSize.y * j) + room.cellSize.y / 2), floorTile, floorContainer);
                }
            }

            floorContainer.SetActive(false);
        }

        /// <summary>
        /// Creates a floor tile
        /// </summary>
        /// <param name="sprite"> The sprite to create the tile with </param>
        /// <param name="location"> The location within in the room to create the tile at </param>
        /// <param name="floorTile"> The floor prefab to instantiate </param>
        /// <param name="floorContainer"> The container to hold the tiles </param>
        /// <returns> The created floor tile </returns>
        private GameObject CreateFloorTile(Sprite sprite, Vector2Int location, GameObject floorTile, GameObject floorContainer)
        {
            floorTile = Instantiate(floorTile);
            floorTile.transform.parent = floorContainer.transform;
            floorTile.transform.localPosition = new Vector3(location.x, location.y, 0);
            floorTile.GetComponent<SpriteRenderer>().sprite = sprite;
            floorTile.SetActive(true);
            return floorTile;
        }
    }
}
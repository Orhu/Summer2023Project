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
                    if (map.map[i, j].room == null)
                    {
                        continue;
                    }

                    CreateRoomExterior(map.map[i, j].room, roomTypesToExteriorParameters.At(map.map[i, j].room.roomType), map, roomSize);
                }
            }
        }

        /// <summary>
        /// Creates the room for the given cell
        /// </summary>
        /// <param name="createdRoom"> The cell to create the room for </param>
        /// <param name="exteriorParameters"> The parameters for this type of room </param>
        /// <param name="map"> The map </param>
        /// <param name="roomSize"> The size of a single room </param>
        private void CreateRoomExterior(Room createdRoom, RoomExteriorGenerationParameters exteriorParameters, Map map, Vector2Int roomSize)
        {
            createdRoom.cellSize = roomSize * createdRoom.roomType.sizeMultiplier;
            createdRoom.roomGrid = new Tile[createdRoom.roomSize.x, createdRoom.roomSize.y];

            CreateWalls(map, createdRoom, exteriorParameters);

            CreateDoors(map, createdRoom);

            CreateFloor(map, createdRoom, exteriorParameters);

            // Reset the exterior parameters (leave things as they were)
            exteriorParameters.Reset();
        }

        /// <summary>
        /// Creates walls for a rectangular room
        /// </summary>
        /// <param name="room"> The room to create the walls for </param>
        /// <param name="exteriorParameters"> The parameters of this type of room </param>
        private void CreateWalls(Map map, Room room, RoomExteriorGenerationParameters exteriorParameters)
        {
            Vector2Int roomSize = room.roomSize;

            GameObject wallContainer = new GameObject();
            wallContainer.transform.parent = room.transform;
            wallContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);
            wallContainer.name = "Wall Container";

            // Add the corners
            Sprite randomCornerSprite = exteriorParameters.bottomLeftWallCornerSprites.GetRandomThing(FloorGenerator.random);
            room.roomGrid[0, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(0, 0), exteriorParameters.wallTile, wallContainer);

            randomCornerSprite = exteriorParameters.topLeftWallCornerSprites.GetRandomThing(FloorGenerator.random);
            room.roomGrid[0, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(0, roomSize.y - 1), exteriorParameters.wallTile, wallContainer);

            randomCornerSprite = exteriorParameters.bottomRightWallCornerSprites.GetRandomThing(FloorGenerator.random);
            room.roomGrid[roomSize.x - 1, 0] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, 0), exteriorParameters.wallTile, wallContainer);

            randomCornerSprite = exteriorParameters.topRightWallCornerSprites.GetRandomThing(FloorGenerator.random);
            room.roomGrid[roomSize.x - 1, roomSize.y - 1] = CreateWallTile(randomCornerSprite, new Vector2Int(roomSize.x - 1, roomSize.y - 1), exteriorParameters.wallTile, wallContainer);

            // Add the top and bottom walls
            for (int i = 1; i < roomSize.x - 1; i++)
            {
                Sprite randomWallSprite = null;
                MapCell roomCell = map.map[(i / (room.roomType.sizeMultiplier.x * room.cellSize.x)) + room.roomLocation.x, room.roomLocation.y];
                if ((i == room.cellSize.x / 2 - 1) && (roomCell.direction & Direction.Down) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.besideBottomDoorLeftSprites.GetRandomThing(FloorGenerator.random);
                }
                else if ((i == room.cellSize.x / 2 + 1) && (roomCell.direction & Direction.Down) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.besideBottomDoorRightSprites.GetRandomThing(FloorGenerator.random);
                }
                else if (i != room.cellSize.x / 2 || (roomCell.direction & Direction.Down) == Direction.None)
                {
                    randomWallSprite = exteriorParameters.bottomWallSprites.GetRandomThing(FloorGenerator.random);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[i, 0] = CreateWallTile(randomWallSprite, new Vector2Int(i, 0), exteriorParameters.wallTile, wallContainer);
                }

                randomWallSprite = null;
                roomCell = map.map[(i / (room.roomType.sizeMultiplier.x * room.cellSize.x)) + room.roomLocation.x, room.roomType.sizeMultiplier.y + room.roomLocation.y - 1];
                if ((i == room.cellSize.x / 2 - 1) && (roomCell.direction & Direction.Up) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.besideTopDoorLeftSprites.GetRandomThing(FloorGenerator.random);
                }
                else if ((i == room.cellSize.x / 2 + 1) && (roomCell.direction & Direction.Up) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.besideTopDoorRightSprites.GetRandomThing(FloorGenerator.random);
                }
                else if (i != room.cellSize.x / 2 || (roomCell.direction & Direction.Up) == Direction.None)
                {
                    randomWallSprite = exteriorParameters.topWallSprites.GetRandomThing(FloorGenerator.random);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[i, roomSize.y - 1] = CreateWallTile(randomWallSprite, new Vector2Int(i, roomSize.y - 1), exteriorParameters.wallTile, wallContainer);
                }
            }

            // Add the right and left walls
            for (int j = 1; j < roomSize.y - 1; j++)
            {
                Sprite randomWallSprite = null;
                MapCell roomCell = map.map[room.roomLocation.x, (j / (room.roomType.sizeMultiplier.y * room.cellSize.y)) + room.roomLocation.y];
                if ((j == room.cellSize.y / 2 - 1) && (roomCell.direction & Direction.Left) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.belowLeftDoorSprites.GetRandomThing(FloorGenerator.random);
                }
                else if ((j == room.cellSize.y / 2 + 1) && (roomCell.direction & Direction.Left) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.aboveLeftDoorSprites.GetRandomThing(FloorGenerator.random);
                }
                else if (j != room.cellSize.y / 2 || (roomCell.direction & Direction.Left) == Direction.None)
                {
                    randomWallSprite = exteriorParameters.leftWallSprites.GetRandomThing(FloorGenerator.random);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[0, j] = CreateWallTile(randomWallSprite, new Vector2Int(0, j), exteriorParameters.wallTile, wallContainer);
                }

                randomWallSprite = null;
                roomCell = map.map[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, (j / (room.roomType.sizeMultiplier.y * room.cellSize.y)) + room.roomLocation.y];
                if ((j == room.cellSize.y / 2 - 1) && (roomCell.direction & Direction.Right) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.belowRightDoorSprites.GetRandomThing(FloorGenerator.random);
                }
                else if ((j == room.cellSize.y / 2 + 1) && (roomCell.direction & Direction.Right) != Direction.None)
                {
                    randomWallSprite = exteriorParameters.aboveRightDoorSprites.GetRandomThing(FloorGenerator.random);
                }
                else if (j != room.cellSize.y / 2 || (roomCell.direction & Direction.Right) == Direction.None)
                {
                    randomWallSprite = exteriorParameters.rightWallSprites.GetRandomThing(FloorGenerator.random);
                }
                if (randomWallSprite != null)
                {
                    room.roomGrid[roomSize.x - 1, j] = CreateWallTile(randomWallSprite, new Vector2Int(roomSize.x - 1, j), exteriorParameters.wallTile, wallContainer);
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
            Tile tile = wallTile.ShallowCopy();
            tile.spawnedObject = Instantiate(tile.spawnedObject);
            tile.spawnedObject.transform.parent = wallContainer.transform;
            tile.spawnedObject.transform.localPosition = new Vector3(location.x, location.y, 0);
            tile.spawnedObject.GetComponent<SpriteRenderer>().sprite = sprite;
            tile.spawnedObject.SetActive(true);
            tile.gridLocation = location;
            return tile;
        }

        /// <summary>
        /// Creates doors for the room
        /// </summary>
        /// <param name="map"> The map </param>
        /// <param name="room"> The room to create doors for </param>
        private void CreateDoors(Map map, Room room)
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
                        RoomExteriorGenerationParameters exteriorParameters = FloorGenerator.floorGeneratorInstance.roomTypesToExteriorGenerationParameters.At(connectedCell.room.roomType);

                        Vector2Int doorLocation = (mapOffset * room.cellSize / 2) + room.cellSize / 2 + (roomCell.location - room.roomLocation) * room.cellSize;

                        DoorSprites doorSprites = new DoorSprites();

                        if (direction == Direction.Right)
                        {
                            doorSprites = exteriorParameters.rightDoorSprites.GetRandomThing(FloorGenerator.random);
                        }
                        else if (direction ==  Direction.Up)
                        {
                            doorSprites = exteriorParameters.topDoorSprites.GetRandomThing(FloorGenerator.random);
                        }
                        else if (direction == Direction.Left)
                        {
                            doorSprites = exteriorParameters.leftDoorSprites.GetRandomThing(FloorGenerator.random);
                        }
                        else if (direction == Direction.Down)
                        {
                            doorSprites = exteriorParameters.bottomDoorSprites.GetRandomThing(FloorGenerator.random);
                        }

                        room.roomGrid[doorLocation.x, doorLocation.y] = CreateDoorTile(doorSprites, doorLocation, direction, connectedCell, exteriorParameters.doorTile, doorContainer);
                        room.doors.Add(room.roomGrid[doorLocation.x, doorLocation.y].spawnedObject.GetComponent<Door>());
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
            Tile tile = doorTile.ShallowCopy();
            tile.spawnedObject = Instantiate(tile.spawnedObject);
            tile.spawnedObject.transform.parent = doorContainer.transform;
            tile.spawnedObject.transform.localPosition = new Vector3(location.x, location.y, 0);

            Door door = tile.spawnedObject.GetComponent<Door>();
            door.Initialize(doorSprites, connectedCell, direction);

            tile.spawnedObject.SetActive(true);
            tile.gridLocation = location;

            return tile;
        }

        /// <summary>
        /// Creates a floor for a room
        /// </summary>
        /// <param name="createdCell"></param>
        /// <param name="exteriorGenerationParameters"></param>
        private void CreateFloor(Map map, Room room, RoomExteriorGenerationParameters exteriorGenerationParameters)
        {
            Vector2Int roomSize = room.roomSize;

            GameObject floorContainer = new GameObject("Floor");
            floorContainer.transform.parent = room.transform;
            floorContainer.transform.localPosition = new Vector3(-roomSize.x / 2, -roomSize.y / 2, 0);

            for (int i = 1; i < roomSize.x - 1; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    Sprite floorSprite = exteriorGenerationParameters.floorSprites.GetRandomThing(FloorGenerator.random);
                    CreateFloorTile(floorSprite, new Vector2Int(i, j), floorContainer);
                }
            }

            // Add the tiles under the left and right doors if they exist

            for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
            {
                if (map.map[room.roomLocation.x, j + room.roomLocation.y].direction.HasFlag(Direction.Left))
                {
                    Sprite floorSprite = exteriorGenerationParameters.floorSprites.GetRandomThing(FloorGenerator.random);
                    CreateFloorTile(floorSprite, new Vector2Int(0, (room.cellSize.y * j) + room.cellSize.y / 2), floorContainer);
                }
                if (map.map[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, j + room.roomLocation.y].direction.HasFlag(Direction.Right))
                {
                    Sprite floorSprite = exteriorGenerationParameters.floorSprites.GetRandomThing(FloorGenerator.random);
                    CreateFloorTile(floorSprite, new Vector2Int(roomSize.x - 1, (room.cellSize.y * j) + room.cellSize.y / 2), floorContainer);
                }
            }

            floorContainer.SetActive(false);
        }

        /// <summary>
        /// Creates a floor tile
        /// </summary>
        /// <param name="sprite"> The sprite to create the tile with </param>
        /// <param name="location"> The location within in the room to create the tile at </param>
        /// <param name="floorContainer"> The container to hold the tiles </param>
        /// <returns> The created floor tile </returns>
        private GameObject CreateFloorTile(Sprite sprite, Vector2Int location, GameObject floorContainer)
        {
            GameObject floorTile = new GameObject();
            floorTile.transform.parent = floorContainer.transform;
            floorTile.transform.localPosition = new Vector3(location.x, location.y, 0);
            floorTile.AddComponent<SpriteRenderer>().sprite = sprite;
            floorTile.SetActive(true);
            floorTile.name = "Floor";
            floorTile.GetComponent<SpriteRenderer>().sortingLayerName = "Floors";

            return floorTile;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Generates the layout of a floor
    /// </summary>
    public class LayoutGenerator : MonoBehaviour
    {
        /// <summary>
        /// Generates the layout.
        /// </summary>
        /// <param name="layoutParameters"> The layout generation parameters. </param>
        public Map Generate(LayoutGenerationParameters layoutParameters)
        {
            int numNormalRooms = 0;
            int numDeadEndRooms = 0;
            Dictionary<RoomType, int> normalRooms = new Dictionary<RoomType, int>();
            Dictionary<RoomType, int> deadEndRooms = new Dictionary<RoomType, int>();
            RoomType startRoomType = null;

            foreach (RoomTypeToLayoutParameters roomTypeToLayoutParameters in layoutParameters.roomTypesTolayoutParameters.roomTypesToLayoutParameters)
            {
                if (roomTypeToLayoutParameters.roomType.startRoom)
                {
                    if (startRoomType == null)
                    {
                        startRoomType = roomTypeToLayoutParameters.roomType;
                    }
                    else
                    {
                        Debug.LogWarning("Only one start room will be generated! Disregarding this room type: " + roomTypeToLayoutParameters.roomType.displayName);
                    }

                    continue;
                }

                int variance = roomTypeToLayoutParameters.numRooomsVariance;
                int numRooms = roomTypeToLayoutParameters.numRooms;
                numRooms += FloorGenerator.random.Next(-variance, variance + 1);
                if (roomTypeToLayoutParameters.roomType.deadEnd)
                {
                    numDeadEndRooms += numRooms;
                    deadEndRooms.Add(roomTypeToLayoutParameters.roomType, numRooms);
                }
                else
                {
                    numNormalRooms += numRooms;
                    normalRooms.Add(roomTypeToLayoutParameters.roomType, numRooms);
                }
            }
            
            // Initialize the gen map
            Vector2Int mapSize = DetermineMapSize(numNormalRooms + numDeadEndRooms);
            MapCell[,] genMap = InitializeGenMap(mapSize);

            GameObject roomContainer = new GameObject();
            roomContainer.transform.parent = transform;
            roomContainer.name = "Room Container";

            // Create the starting room
            MapCell startCell;
            Room startRoom = GenerateStartRoom(genMap, roomContainer, mapSize, startRoomType, out startCell);

            // Get all the branchable cells (which will start out as all the edge normal cells, then cells will be removed from them as it goes along
            List<MapCell> branchableCells = GenerateNormalCells(genMap, roomContainer, startRoom, normalRooms, layoutParameters);

            if (!GenerateDeadEnds(genMap, branchableCells))
            {
                return Generate(layoutParameters);
            }

            return CreateMap(genMap, startCell, mapSize);
        }

        private Vector2Int DetermineMapSize(int numRooms)
        {
            // Make the map size to be able to hold all the rooms in a row in any direction (worst case scenario)
            // +7 because boss room is 3x3, then you have the exit room, and finally you have the start room in the middle. 
            // Then a couple extra for safety (even though it should be very unlikely to happen). 
            // TODO: Change boos room size to not be hard coded lol
            return new Vector2Int(numRooms * 2 + 7, numRooms * 2 + 7);
        }

        /// <summary>
        /// Initializes the gen map with MapCells that have their locations set correctly
        /// </summary>
        /// <param name="numRooms"> The number of normal rooms and special rooms that will appear in the layout </param>
        /// <returns> The map size </returns>
        private MapCell[,] InitializeGenMap(Vector2Int mapSize)
        {
            MapCell[,] genMap = new MapCell[mapSize.x, mapSize.y];

            // Iterate over the map size and set each cell to have the correct location
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    genMap[i, j] = new MapCell();
                    genMap[i, j].location = new Vector2Int(i, j);
                }
            }

            return genMap;
        }

        /// <summary>
        /// Creates a room with the given room type
        /// </summary>
        /// <param name="roomContainer"> The game object to hold all the room game objects in </param>
        /// <param name="roomType"> The room type of the room </param>
        /// <returns> The created room </returns>
        private Room CreateRoom(GameObject roomContainer, RoomType roomType)
        {
            GameObject newRoom = new GameObject();
            newRoom.name = roomType.displayName + " Room";
            newRoom.transform.parent = transform;
            Room roomComponent = newRoom.AddComponent<Room>();
            roomComponent.roomType = roomType;
            return roomComponent;
        }

        /// <summary>
        /// Gets a random offset for the room type (where it will fit)
        /// </summary>
        /// <param name="genMap"> The currently generated map </param>
        /// <param name="room"> The room  to get the random offset of </param>
        /// <param name="generatedCell"> The cell that's being generated </param>
        /// <returns> The random offset and whether or not this room type actually fits in this location </returns>
        private bool GenerateRandomRoom(MapCell[,] genMap, Room room, MapCell generatedCell, bool deadEnd = false, int maxNumRooms = -1, int currentNumRooms = -1)
        {
            // Get the possible offsets
            List<Vector2Int> possibleOffsets = new List<Vector2Int>();

            if (!room.roomType.useRandomOffset)
            {
                room.roomLocation = generatedCell.location + room.roomType.offset;
                return (room.roomType.offset, CheckIfRoomFits(genMap, room));
            }

            bool checkHorizontalOffset = !(generatedCell.direction.HasFlag(Direction.Left) || generatedCell.direction.HasFlag(Direction.Right));
            bool checkVerticalOffset = !(generatedCell.direction.HasFlag(Direction.Up) || generatedCell.direction.HasFlag(Direction.Down));
            for (int i = 0; i < room.roomType.sizeMultiplier.x && (i == 0 || checkHorizontalOffset); i++)
            {
                for (int j = 0; j < room.roomType.sizeMultiplier.y && (j == 0 || checkVerticalOffset); j++)
                {
                    // -i and -j because 0, 0 is bottom left, and we need to move the room down left in order to not lose the room
                    Vector2Int offset = new Vector2Int(-i, -j);
                    room.roomLocation = generatedCell.location + offset;
                    if (CheckIfRoomFits(genMap, room))
                    {
                        possibleOffsets.Add(offset);
                    }
                }
            }

            if (possibleOffsets.Count == 0)
            {
                return (new Vector2Int(0, 0), false);
            }

            return (possibleOffsets[FloorGenerator.random.Next(0, possibleOffsets.Count - 1)], true);
        }

        /// <summary>
        /// Checks if a room fits in it's current location. Also checks whether an attached room would be able to fit.
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room being generated </param>
        /// <returns> Whether or not the room fits at the location </returns>
        private bool CheckIfRoomFits(MapCell[,] genMap, Room room)
        {
            for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
            {
                for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                {
                    if (genMap[i + room.roomLocation.x, j + room.roomLocation.y].visited)
                    {
                        return false;
                    }
                }
            }

            if (room.roomType.attachedRoom != null)
            {
                if (!room.roomType.deadEnd)
                {
                    Debug.LogWarning("Rooms that are not dead ends cannot have attached rooms! Attached room on " + room.roomType.displayName + " will not be generated.");
                    return true;
                }

                return GetPossibleAttachedRoomCells(genMap, room).Count > 0;
            }

            return true;
        }

        /// <summary>
        /// Checks if a room type fits in the map with the given bottom left location. Does not check attached rooms of this type.
        /// </summary>
        /// <param name="genMap"> The current generated map. </param>
        /// <param name="roomType"> The room type to check. </param>
        /// <param name="bottomLeftLocation"> The bottom left location of the room type. </param>
        /// <returns> Whether or not this room type will fit. </returns>
        private bool CheckIfRoomTypeFits(MapCell[,] genMap, RoomType roomType, Vector2Int bottomLeftLocation)
        {
            for (int i = 0; i < roomType.sizeMultiplier.x; i++)
            {
                for (int j = 0; j < roomType.sizeMultiplier.y; j++)
                {
                    if (genMap[i + bottomLeftLocation.x, j + bottomLeftLocation.y].visited)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a list of the possible cells where an attached room could spawn
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to attach another room to </param>
        /// <returns> A list of the possible cells where an attached room could spawn </returns>
        private List<MapCell> GetPossibleAttachedRoomCells(MapCell[,] genMap, Room room)
        {
            if (room.roomType.attachmentLocation == RoomType.AttachmentLocation.NA)
            {
                return GetUnvisitedNeighbors(genMap, room, false);
            }

            // Get the cells that currently have doors
            List<MapCell> doorCells = new List<MapCell>();
            for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
            {
                for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                {
                    if (genMap[i + room.roomLocation.x, j + room.roomLocation.y].direction != Direction.None)
                    {
                        doorCells.Add(genMap[i + room.roomLocation.x, j + room.roomLocation.y]);
                    }
                }
            }

            List<MapCell> possibleCells = new List<MapCell>();

            // Lord forgive me for what I have done
            Vector2Int mapLocationToCheck = new Vector2Int();
            System.Action<Vector2Int> checkGenMap = (location) =>
            {
                if (!genMap[location.x, location.y].visited && CheckIfRoomTypeFits(genMap, room.roomType.attachedRoom, location))
                {
                    possibleCells.Add(genMap[location.x, location.y]);
                }
            };

            foreach (MapCell doorCell in doorCells)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (doorCell.direction.HasFlag(direction))
                    {
                        switch (room.roomType.attachmentLocation)
                        {
                            case RoomType.AttachmentLocation.SoftOpposite:
                                if (direction == Direction.Right || direction == Direction.Left)
                                {
                                    for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                                    {
                                        mapLocationToCheck = new Vector2Int();
                                        if (direction == Direction.Right)
                                        {
                                            mapLocationToCheck.x = room.roomLocation.x - 1;
                                        }
                                        else if (direction == Direction.Left)
                                        {
                                            mapLocationToCheck.x = room.roomLocation.x + room.roomType.sizeMultiplier.x;
                                        }
                                        mapLocationToCheck.y = room.roomLocation.y + j;

                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                if (direction == Direction.Up || direction == Direction.Down)
                                {
                                    for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
                                    {
                                        mapLocationToCheck = new Vector2Int();
                                        if (direction == Direction.Up)
                                        {
                                            mapLocationToCheck.y = room.roomLocation.y - 1;
                                        }
                                        else if (direction == Direction.Left)
                                        {
                                            mapLocationToCheck.y = room.roomLocation.y + room.roomType.sizeMultiplier.y;
                                        }
                                        mapLocationToCheck.x = room.roomLocation.y + i;

                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                break;

                            case RoomType.AttachmentLocation.HardOpposite:
                                mapLocationToCheck = new Vector2Int();
                                if (direction == Direction.Right)
                                {
                                    mapLocationToCheck.x = room.roomLocation.x - 1;
                                    mapLocationToCheck.y = doorCell.location.y;
                                }
                                else if (direction == Direction.Up)
                                {
                                    mapLocationToCheck.x = doorCell.location.x;
                                    mapLocationToCheck.y = room.roomLocation.y - 1;
                                }
                                else if (direction == Direction.Left)
                                {
                                    mapLocationToCheck.x = room.roomLocation.x + room.roomType.sizeMultiplier.x;
                                    mapLocationToCheck.y = doorCell.location.y;
                                }
                                else if (direction == Direction.Down)
                                {
                                    mapLocationToCheck.x = doorCell.location.x;
                                    mapLocationToCheck.y = room.roomLocation.y + room.roomType.sizeMultiplier.y;
                                }
                                checkGenMap(mapLocationToCheck);
                                break;

                            case RoomType.AttachmentLocation.SoftAdjacent:
                                if (direction == Direction.Right || direction == Direction.Left)
                                {
                                    for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                                    {
                                        if (j != doorCell.location.y)
                                        {
                                            mapLocationToCheck = new Vector2Int();
                                            if (direction == Direction.Right)
                                            {
                                                mapLocationToCheck.x = doorCell.location.x + 1;
                                            }
                                            else if (direction == Direction.Left)
                                            {
                                                mapLocationToCheck.x = room.roomLocation.x - 1;
                                            }
                                            mapLocationToCheck.y = room.roomLocation.y + j;

                                            checkGenMap(mapLocationToCheck);
                                        }
                                    }
                                }
                                if (direction == Direction.Up || direction == Direction.Down)
                                {
                                    for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
                                    {
                                        if (i != doorCell.location.x)
                                        {
                                            mapLocationToCheck = new Vector2Int();
                                            if (direction == Direction.Up)
                                            {
                                                mapLocationToCheck.y = doorCell.location.y + 1;
                                            }
                                            else if (direction == Direction.Down)
                                            {
                                                mapLocationToCheck.y = room.roomLocation.y - 1;
                                            }
                                            mapLocationToCheck.x = room.roomLocation.x + i;

                                            checkGenMap(mapLocationToCheck);
                                        }
                                    }
                                }
                                break;

                            case RoomType.AttachmentLocation.HardAdjacent:
                                if (direction == Direction.Right || direction == Direction.Left)
                                {
                                    mapLocationToCheck = new Vector2Int();
                                    if (direction == Direction.Right)
                                    {
                                        mapLocationToCheck.x = doorCell.location.x + 1;
                                    }
                                    else if (direction == Direction.Left)
                                    {
                                        mapLocationToCheck.x = room.roomLocation.x - 1;
                                    }
                                    mapLocationToCheck.y = doorCell.location.y - 1;
                                    if (mapLocationToCheck.y >= room.roomLocation.y)
                                    {
                                        checkGenMap(mapLocationToCheck);
                                    }
                                    mapLocationToCheck.y = doorCell.location.y + 1;
                                    if (mapLocationToCheck.y <= room.roomLocation.y + room.roomType.sizeMultiplier.y - 1)
                                    {
                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                else if (direction == Direction.Up || direction == Direction.Down)
                                {
                                    mapLocationToCheck = new Vector2Int();
                                    mapLocationToCheck.x = doorCell.location.x - 1;
                                    if (direction == Direction.Up)
                                    {
                                        mapLocationToCheck.y = doorCell.location.y + 1;
                                    }
                                    else if (direction == Direction.Down)
                                    {
                                        mapLocationToCheck.y = room.roomLocation.y - 1;
                                    }
                                    if (mapLocationToCheck.x >= room.roomLocation.x)
                                    {
                                        checkGenMap(mapLocationToCheck);
                                    }
                                    mapLocationToCheck.x = doorCell.location.x + 1;
                                    if (mapLocationToCheck.x <= room.roomLocation.x + room.roomType.sizeMultiplier.x - 1)
                                    {
                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                break;
                            case RoomType.AttachmentLocation.Perpendicular:
                                if (direction == Direction.Right || direction == Direction.Left)
                                {
                                    for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
                                    {
                                        mapLocationToCheck = new Vector2Int();
                                        mapLocationToCheck.x = i + room.roomLocation.x;
                                        mapLocationToCheck.y = room.roomLocation.y - 1;
                                        checkGenMap(mapLocationToCheck);
                                        mapLocationToCheck.y = room.roomLocation.y + room.roomType.sizeMultiplier.y;
                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                else if (direction == Direction.Up || direction == Direction.Down)
                                {
                                    for (int j = 0; j < room.roomType.sizeMultiplier.x; j++)
                                    {
                                        mapLocationToCheck = new Vector2Int();
                                        mapLocationToCheck.x = room.roomLocation.x - 1;
                                        mapLocationToCheck.y = j + room.roomLocation.y;
                                        checkGenMap(mapLocationToCheck);
                                        mapLocationToCheck.x = room.roomLocation.x + room.roomType.sizeMultiplier.x;
                                        checkGenMap(mapLocationToCheck);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            return possibleCells;
        }

        /// <summary>
        /// Sets the cells contained in a room to hold a reference to the room, and sets the cells to visited
        /// </summary>
        /// <param name="genMap"> The current generated map to set the cells of </param>
        /// <param name="room"> The room to set the cells of </param>
        private void SetRoomCells(MapCell[,] genMap, Room room)
        {
            foreach (Vector2Int location in room.roomMapCellLocations)
            {
                MapCell cell = genMap[location.x, location.y];
                cell.room = room.gameObject;
                cell.visited = true;
            }
        }

        /// <summary>
        /// Initializes the starting room in the center of the map
        /// </summary>
        /// <param name="genMap"> The map that is being generated </param>
        /// <param name="mapSize"> The size of the map </param>
        /// <param name="startRoomType"> The room type to generate </param>
        /// <param name="startCell"> The out param for the start cell (aka the middle of the map) </param>
        /// <returns> A reference to the start room </returns>
        private Room GenerateStartRoom(MapCell[,] genMap, GameObject roomContainer, Vector2Int mapSize, RoomType startRoomType, out MapCell startCell)
        {
            Vector2Int startPos = new Vector2Int((mapSize.x + 1) / 2, (mapSize.y + 1) / 2);
            startCell = genMap[startPos.x, startPos.y];
            Room startRoom = CreateRoom(roomContainer, startRoomType);
            bool fits = GenerateRandomRoom(genMap, startRoom, startCell, true);
            if (!fits)
            {
                Debug.Log("The start room somehow doesn't fit in the map");
            }
            SetRoomCells(genMap, startRoom);

            // The start room is hard-coded to have 4 doors (maybe someday I will make this not hard-coded but attached rooms are enough for me)
            genMap[startRoom.roomLocation.x + startRoom.roomType.sizeMultiplier.x - 1, startRoom.roomLocation.y + (startRoom.roomType.sizeMultiplier.y / 2)].direction |= Direction.Right;
            genMap[startRoom.roomLocation.x + (startRoom.roomType.sizeMultiplier.x / 2), startRoom.roomLocation.y + startRoom.roomType.sizeMultiplier.y - 1].direction |= Direction.Up;
            genMap[startRoom.roomLocation.x, startRoom.roomLocation.y + (startRoom.roomType.sizeMultiplier.y / 2)].direction |= Direction.Left;
            genMap[startRoom.roomLocation.x + (startRoom.roomType.sizeMultiplier.x / 2), startRoom.roomLocation.y].direction |= Direction.Down;
            return startRoom;
        }

        /// <summary>
        /// Gets the edge cells of a room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the edge cells of </param>
        /// <returns> The list of edge cells </returns>
        private List<MapCell> GetEdgeCells(MapCell[,] genMap, Room room)
        {
            List<MapCell> edgeCells = new List<MapCell>();

            if (room.roomType.sizeMultiplier == new Vector2Int(1, 1))
            {
                edgeCells.Add(genMap[room.roomLocation.x, room.roomLocation.y]);
                return edgeCells;
            }

            if (room.roomType.sizeMultiplier.x == 1)
            {
                for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                {
                    edgeCells.Add(genMap[room.roomLocation.x, room.roomLocation.y + j]);
                }

                return edgeCells;
            }

            if (room.roomType.sizeMultiplier.y == 1)
            {
                for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
                {
                    edgeCells.Add(genMap[room.roomLocation.x + i, room.roomLocation.y]);
                }

                return edgeCells;
            }

            for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
            {
                edgeCells.Add(genMap[room.roomLocation.x + i, room.roomLocation.y]);
                edgeCells.Add(genMap[room.roomLocation.x + i, room.roomLocation.y + room.roomType.sizeMultiplier.y - 1]);
            }

            for (int j = 1; j < room.roomType.sizeMultiplier.y - 1; j++)
            {
                edgeCells.Add(genMap[room.roomLocation.x, room.roomLocation.y + j]);
                edgeCells.Add(genMap[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, room.roomLocation.y + j]);
            }

            return edgeCells;
        }

        /// <summary>
        /// Generates all the normal rooms in the map
        /// </summary>
        /// <param name="genMap"> The map being generated </param>
        /// <param name="startingRoom"> The starting cell </param>
        /// <param name="numNormalCells"> The number of normal cells to generate </param>
        /// <returns> A list of all the branchable cells </returns>
        private List<MapCell> GenerateNormalCells(LayoutGenerationParameters layoutParameters, MapCell[,] genMap, GameObject roomContainer, Room startRoom, Dictionary<RoomType, int> normalRooms)
        {
            // Track the branchable cells created
            List<MapCell> branchableCells = new List<MapCell>();

            // Track the cells that still need to be generated
            Queue<MapCell> cellsToGenerate = new Queue<MapCell>();
            int newRoomsCount = 0;

            // Track the room types and their counts
            Dictionary<RoomType, int> roomTypeCounts = new Dictionary<RoomType, int>();

            int totalNormalRooms = 0;
            foreach (KeyValuePair<RoomType, int> entry in normalRooms)
            {
                totalNormalRooms += entry.Value;
            }

            // Add the neighbors of the start cell to the cells to generate
            List<MapCell> initialNeighbors = GetUnvisitedNeighbors(genMap, startRoom, true);
            while (initialNeighbors.Count != 0)
            {
                MapCell randomInitialNeighbor = initialNeighbors[FloorGenerator.random.Next(0, initialNeighbors.Count)];
                randomInitialNeighbor.visited = true;
                cellsToGenerate.Enqueue(randomInitialNeighbor);
                initialNeighbors.Remove(randomInitialNeighbor);
                newRoomsCount++;
            }

            // Generate all the cells using BFS
            while (cellsToGenerate.Count != 0)
            {
                // Generate the current cell
                MapCell currentCell = cellsToGenerate.Peek();

                List<RoomType> possibleRoomTypes = normalRooms.Keys.ToList();
                Room newRoom;
                bool fits;
                do
                {
                    newRoom = CreateRoom(roomContainer, possibleRoomTypes[FloorGenerator.random.Next(0, possibleRoomTypes.Count)]);
                    possibleRoomTypes.Remove(newRoom.roomType);
                    fits = GenerateRandomRoom(genMap, newRoom, currentCell);
                }
                while (!fits && possibleRoomTypes.Count > 0);

                if (possibleRoomTypes.Count == 0)
                {
                    // Unfortunately it's possible for this to happen if you don't have enough single-celled rooms. We're not worrying about that for now though  :')
                    // Potential fix idea: Force create single rooms (even if it exceeds the # of rooms wanted)
                    throw new System.Exception("Unable to generate layout! The rooms do not fit.");
                }

                if (roomTypeCounts.ContainsKey(newRoom.roomType))
                {
                    roomTypeCounts[newRoom.roomType]++;
                }
                else
                {
                    roomTypeCounts.Add(newRoom.roomType, 1);
                }

                if (roomTypeCounts[newRoom.roomType] == normalRooms[newRoom.roomType])
                {
                    normalRooms.Remove(newRoom.roomType);
                }

                SetRoomCells(genMap, newRoom);

                // Remove the current cell from the queue
                cellsToGenerate.Dequeue();

                // Add the neighbors of the cell to the queue
                List<MapCell> neighbors = GetUnvisitedNeighbors(genMap, newRoom, true);
                while (neighbors.Count != 0)
                {
                    MapCell randomNeighbor = neighbors[FloorGenerator.random.Next(0, neighbors.Count)];
                    randomNeighbor.visited = true;
                    cellsToGenerate.Enqueue(randomNeighbor);
                    neighbors.Remove(randomNeighbor);
                    newRoomsCount++;
                }

                // Add the edges of this room to the branchable cells
                foreach (MapCell edgeCell in GetEdgeCells(genMap, newRoom))
                {
                    branchableCells.Add(edgeCell);
                }
            }

            return branchableCells;
        }

        /// <summary>
        /// Gets all the unvisited neighbors of the given room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="roomType"> The room to get the neighbors of </param>
        /// <param name="useDirection"> Whether or not to use the directions that the given room opens in to get the neighbors </param>
        /// <returns> The unvisited neighbors </returns>
        private List<MapCell> GetUnvisitedNeighbors(MapCell[,] genMap, Room room, bool useDirection)
        {
            List<MapCell> neighbors = new List<MapCell>();

            List<MapCell> edges = GetEdgeCells(genMap, room);
            foreach (MapCell edge in edges)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    bool checkDirection = !useDirection || (useDirection && edge.direction.HasFlag(direction));

                    Vector2Int locationOffset = new Vector2Int();
                    locationOffset.x = System.Convert.ToInt32(direction.HasFlag(Direction.Right)) - System.Convert.ToInt32(direction.HasFlag(Direction.Left));
                    locationOffset.y = System.Convert.ToInt32(direction.HasFlag(Direction.Up)) - System.Convert.ToInt32(direction.HasFlag(Direction.Down));
                    bool locationOutsideRoom = locationOffset.x < room.roomLocation.x || locationOffset.x >= room.roomLocation.x + room.roomType.sizeMultiplier.x;
                    locationOutsideRoom &= locationOffset.y < room.roomLocation.y || locationOffset.y >= room.roomLocation.y + room.roomType.sizeMultiplier.y;

                    if (checkDirection && locationOutsideRoom && !genMap[room.roomLocation.x + locationOffset.x, room.roomLocation.y + locationOffset.y].visited)
                    {
                        neighbors.Add(genMap[room.roomLocation.x + locationOffset.x, room.roomLocation.y + locationOffset.y]);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Gets a random direction constrained by the constraint
        /// </summary>
        /// <param name="constraint"> The constraint </param>
        /// <returns> The random direction </returns>
        private Direction GetRandomConstrainedDirection(DirectionConstraint constraint, int preferredNumDoors, float strictnessNumDoors)
        {
            Direction direction = Direction.None;
            int numDirections = 0;

            // Store the possible (new) directions this cell can open in
            List<Direction> possibleDirections = new List<Direction>();
            possibleDirections.Add(Direction.Right);
            possibleDirections.Add(Direction.Up);
            possibleDirections.Add(Direction.Left);
            possibleDirections.Add(Direction.Down);

            // Add all the directions this cell must have
            for (int i = 1; i <= (int)Direction.Down; i *= 2)
            {
                if ((constraint.mustHave & (Direction)i) != Direction.None)
                {
                    possibleDirections.Remove((Direction)i);
                    numDirections++;
                    direction |= (Direction)i;
                }
            }

            // This should never happen but you never know
            if (numDirections > constraint.maxDirections)
            {
                throw new System.Exception("A room was generated that must have more directions than it's allowed");
            }

            // While it's still possible to open in another direction
            while (possibleDirections.Count != 0)
            {
                // If we're at the max number of directions this room is allowed to have, then stop adding more directions
                if (numDirections >= constraint.maxDirections)
                {
                    break;
                }

                // Choose a random possible direction
                Direction randomDirection = possibleDirections[FloorGenerator.random.Next(0, possibleDirections.Count)];

                // If this room must not have this direction, then remove it from the possible directions and try again
                if ((constraint.mustNotHave & randomDirection) != Direction.None)
                {
                    possibleDirections.Remove(randomDirection);
                    continue;
                }

                // Get the "distance" from the number of directions this cell already has to the number of directions is preferred
                int distance = preferredNumDoors - numDirections;

                // Using the likelyhood from the distance, determine whether or not to add this direction            
                if (CalculateLikelyhoodOfAddingDirection(distance, strictnessNumDoors) > FloorGenerator.random.NextDouble())
                {
                    direction |= randomDirection;
                    numDirections++;
                }

                // Whether the direction was added or not, remove it from the possible new directions
                possibleDirections.Remove(randomDirection);
            }
            return direction;
        }

        /// <summary>
        /// Calculates the likelyhood of a room having another door, using the distance from the number of doors the room
        /// already has to the desired number of doors
        /// </summary>
        /// <param name="distance"> The distance from the number of directions the room already has to the preferred number of doors </param>
        /// <returns> The likelyood </returns>
        private float CalculateLikelyhoodOfAddingDoor(int distance, float strictnessNumDoors)
        {
            // Get the amount to scale the range of atan by so that it has a range of 100
            float scaleRange = (50.0f * 2.0f) / (Mathf.PI);

            // Set the horizontal offset of the atan function so that when the distance is 0, the function doesn't always spit out 50
            float horizontalOffset = 0.25f;

            // Set the vertical offset so that the function ranges from 0 to 100 instead of -50 to 50
            float verticalOffset = 50;

            // Get the value of the atan function, using the strictness to control how steep the function is
            float atanVal = Mathf.Atan(strictnessNumDoors * (distance - horizontalOffset));

            // Divide by 100 to convert the percent to decimal
            return (scaleRange * atanVal + verticalOffset) / 100;
        }

        /// <summary>
        /// Gets the constraint the given MapCell must adhere to
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="cell"> The cell to get the constraint of </param>
        /// <param name="maxNewCells"> The maximum number of new cells to add. -1 to not check </param>
        /// <param name="currentNewCells"> The number of current new cells already added </param>
        /// <returns> The constraint </returns>
        private DirectionConstraint GetDirectionConstraint(MapCell[,] genMap, MapCell cell, int maxNewCells = -1, int currentNewCells = -1)
        {
            DirectionConstraint directionConstraint = new DirectionConstraint();

            // Store the opposite direction of the current direction we're checking
            int oppositeDirectionToCheck = (int)Direction.Left;
            for (int directionToCheck = 1; directionToCheck <= (int)Direction.Down; directionToCheck *= 2)
            {
                // Get the location offset of the neighboring cell baesd on the direction
                Vector2Int locationOffset = new Vector2Int();
                locationOffset.x = System.Convert.ToInt32(((Direction)directionToCheck & Direction.Right) != Direction.None) - System.Convert.ToInt32(((Direction)directionToCheck & Direction.Left) != Direction.None);
                locationOffset.y = System.Convert.ToInt32(((Direction)directionToCheck & Direction.Up) != Direction.None) - System.Convert.ToInt32(((Direction)directionToCheck & Direction.Down) != Direction.None);

                // Get the neighbor cell
                MapCell neighbor = genMap[cell.location.x + locationOffset.x, cell.location.y + locationOffset.y];

                // If the neighbor is visited, then it affects the constraint
                if (neighbor.visited)
                {
                    // If the neighbor has a door opening in the opposite direction, then the constraint must have that direction
                    if ((neighbor.direction & (Direction)oppositeDirectionToCheck) != Direction.None)
                    {
                        directionConstraint.mustHave |= (Direction)directionToCheck;
                    }
                    // If it doesn't, then the constraint must not have that direction 
                    else
                    {
                        directionConstraint.mustNotHave |= (Direction)directionToCheck;
                    }
                }

                // Update the opposite direction to stay in sync 
                oppositeDirectionToCheck *= 2;
                if (oppositeDirectionToCheck > (int)Direction.Down)
                {
                    oppositeDirectionToCheck = (int)Direction.Right;
                }
            }

            // If we do care about the number of max new cells, then set the max number
            if (maxNewCells != -1)
            {
                // Count the number of directions the room must have (because they don't count towards new rooms)
                int count = 0;
                for (int i = 1; i < (int)Direction.All; i *= 2)
                {
                    count += ((int) directionConstraint.mustHave & i) / i;
                }

                directionConstraint.maxDirections = maxNewCells - currentNewCells + count;
            }

            return directionConstraint;
        }

        /// <summary>
        /// Generates the special cells by branching off the branchable cells
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="branchableCells"> The cells that can be branched off of</param>
        /// <param name="numSpecialCells"> The number of special cells to generate </param>
        /// <returns> Whether or not all the special cells were successfully generated </returns>
        private bool GenerateSpecialCells(MapCell[,] genMap, List<MapCell> branchableCells, int numSpecialCells)
        {
            for (int i = 0; i < numSpecialCells; i++)
            {
                while (true)
                {
                    // Check that it's still possible to branch
                    if (branchableCells.Count == 0)
                    {
                        // This should never happen, but if it does then sadge
                        return false;
                    }

                    // Choose a random cell to branch off of
                    MapCell branchCell = branchableCells[FloorGenerator.random.Next(0, branchableCells.Count)];

                    // Get its unvisited neighbors
                    List<MapCell> neighbors = GetUnvisitedNeighbors(genMap, branchCell, false);

                    // Check that there actually are any neighbors
                    if (neighbors.Count == 0)
                    {
                        branchableCells.Remove(branchCell);
                        continue;
                    }

                    // Choose a random neighbor to be the special room
                    MapCell specialCell = neighbors[FloorGenerator.random.Next(0, neighbors.Count)];
                    specialCell.visited = true;
                    specialCell.type = RoomType.Special;

                    // Figure out what direction it's in
                    if (specialCell.location.x > branchCell.location.x)
                    {
                        specialCell.direction = Direction.Left;
                        branchCell.direction |= Direction.Right;
                    }
                    else if (specialCell.location.y > branchCell.location.y)
                    {
                        specialCell.direction = Direction.Down;
                        branchCell.direction |= Direction.Up;
                    }
                    else if (specialCell.location.x < branchCell.location.x)
                    {
                        specialCell.direction = Direction.Right;
                        branchCell.direction |= Direction.Left;
                    }
                    else
                    {
                        specialCell.direction = Direction.Up;
                        branchCell.direction |= Direction.Down;
                    }

                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Takes the genMap and sets it up for use (by resetting whether the cells have been visited or not)
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <returns> The map </returns>
        private Map CreateMap(MapCell[,] genMap, MapCell startCell, Vector2Int mapSize)
        {
            MapCell[,] map = genMap;
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    map[i, j].visited = false;
                }
            }

            Map createdMap = new Map();
            createdMap.map = map;
            createdMap.mapSize = mapSize;
            createdMap.startCell = startCell;
            return createdMap;
        }
    }
}
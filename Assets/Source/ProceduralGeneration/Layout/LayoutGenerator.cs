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
        #region INITIALIZATION

        /// <summary>
        /// Determines the size of the map
        /// </summary>
        /// <param name="normalRooms"> The normal rooms and their counts </param>
        /// <param name="deadEndRooms"> The dead end rooms and their counts </param>
        /// <param name="startSize"> The size of the starting room </param>
        /// <returns> The map size </returns>
        private Vector2Int DetermineMapSize(Dictionary<RoomType, int> normalRooms, Dictionary<RoomType, int> deadEndRooms, Vector2Int startSize)
        {
            // The length of all the rooms lined up together
            int length = 0;
            foreach (KeyValuePair<RoomType, int> room in normalRooms)
            {
                length += System.Math.Max(room.Key.sizeMultiplier.x, room.Key.sizeMultiplier.y) * room.Value;
            }
            foreach (KeyValuePair<RoomType, int> room in deadEndRooms)
            {
                length += System.Math.Max(room.Key.sizeMultiplier.x, room.Key.sizeMultiplier.y) * room.Value;
            }

            // Multiplied by 2 because rooms can extend out in any direction from the middle, + start room size 
            return new Vector2Int(length * 2 + 1, length * 2 + System.Math.Max(startSize.x, startSize.y));
        }

        /// <summary>
        /// Initializes the gen map with MapCells that have their locations set correctly
        /// </summary>
        /// <param name="mapSize"> The map size </param>
        /// <returns> The initial gen map </returns>
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
        /// Takes the genMap and sets it up for use (by resetting whether the cells have been visited or not)
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <returns> The map </returns>
        private Map CreateMap(MapCell[,] genMap, Room startRoom, Vector2Int mapSize)
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
            createdMap.startRoom = startRoom;
            return createdMap;
        }

        #endregion

        #region MULTI_ROOM_GENERATION

        /// <summary>
        /// Generates the layout.
        /// </summary>
        /// <param name="layoutParams"> The layout generation Params. </param>
        /// <param name="templateCounts"> The room types and their associated template counts </param>
        /// <param name="generateCallCount"> The number of times generate has been called </param>
        public Map Generate(LayoutParams layoutParams, Dictionary<RoomType, int> templateCounts, int generateCallCount = 0)
        {
            Dictionary<RoomType, int> normalRooms = new Dictionary<RoomType, int>();
            Dictionary<RoomType, int> deadEndRooms = new Dictionary<RoomType, int>();
            List<RoomType> emergencyRooms = new List<RoomType>();
            RoomType startRoomType = null;

            GenericWeightedThings<RoomType> bossRooms = new GenericWeightedThings<RoomType>();

            foreach (RoomTypeToLayoutParams roomTypeToLayoutParams in layoutParams.roomTypesToLayoutParams.roomTypesToLayoutParams)
            {
                if (roomTypeToLayoutParams.roomType.startRoom)
                {
                    if (startRoomType == null)
                    {
                        startRoomType = roomTypeToLayoutParams.roomType;
                    }
                    else
                    {
                        Debug.LogWarning("Only one start room will be generated! Disregarding this room type: " + roomTypeToLayoutParams.roomType.displayName);
                    }

                    continue;
                }

                if (roomTypeToLayoutParams.roomType.bossRoom)
                {
                    bossRooms.Add(roomTypeToLayoutParams.roomType, templateCounts[roomTypeToLayoutParams.roomType], 1, true);
                }
                else
                {
                    int variance = roomTypeToLayoutParams.layoutParams.numRoomsVariance;
                    int numRooms = roomTypeToLayoutParams.layoutParams.numRooms;
                    numRooms += FloorGenerator.random.Next(-variance, variance + 1);
                    if (numRooms > 0)
                    {
                        if (roomTypeToLayoutParams.roomType.deadEnd)
                        {
                            deadEndRooms.Add(roomTypeToLayoutParams.roomType, numRooms);
                        }
                        else
                        {
                            normalRooms.Add(roomTypeToLayoutParams.roomType, numRooms);
                        }
                    }
                }

                if (roomTypeToLayoutParams.roomType.emergencyRoom)
                {
                    if (roomTypeToLayoutParams.roomType.sizeMultiplier != new Vector2Int(1, 1))
                    {
                        Debug.LogWarning("Emergency rooms may not have a size multiplier other than 1, 1! Disregarding " + roomTypeToLayoutParams.roomType.displayName + " as a valid emergency room");
                    }
                    else if (roomTypeToLayoutParams.roomType.attachedRoom != null)
                    {
                        Debug.LogWarning("Emergency rooms may not have an attached room! Disregarding " + roomTypeToLayoutParams.roomType.displayName + " as a valid emergency room");
                    }
                    else
                    {
                        emergencyRooms.Add(roomTypeToLayoutParams.roomType);
                    }
                }

                if (emergencyRooms.Count == 0)
                {
                    Debug.LogWarning("There are no emergency rooms! There might be a chance that generation will fail.");
                }
            }

            for (int i = 0; i < layoutParams.mapLayoutParams.numBossRooms; i++)
            {
                RoomType randomBossRoom = bossRooms.GetRandomThing(FloorGenerator.random);
                if (deadEndRooms.ContainsKey(randomBossRoom))
                {
                    deadEndRooms[randomBossRoom]++;
                }
                else
                {
                    deadEndRooms.Add(randomBossRoom, 1);
                }
            }

            // Initialize the gen map
            Vector2Int mapSize = DetermineMapSize(normalRooms, deadEndRooms, startRoomType.sizeMultiplier);
            MapCell[,] genMap = InitializeGenMap(mapSize);

            GameObject roomContainer = new GameObject();
            roomContainer.transform.parent = transform;
            roomContainer.name = "Room Container";

            // Create the starting room
            Room startRoom = GenerateStartRoom(genMap, roomContainer, mapSize, startRoomType);

            // Get all the branchable cells (which will start out as all the edge normal cells, then cells will be removed from them as it goes along
            List<MapCell> branchableCells = GenerateNormalRooms(genMap, roomContainer, startRoom, normalRooms, emergencyRooms, layoutParams.mapLayoutParams.preferredNumDoors, layoutParams.mapLayoutParams.strictnessNumDoors);

            if (!GenerateDeadEnds(genMap, roomContainer, startRoom.startLocation, branchableCells, deadEndRooms))
            {
                if (generateCallCount == 2)
                {
                    throw new System.Exception("Failed to generate all the dead ends three times in a row! Aborting generation. Please remove some dead end rooms or add more normal rooms.");
                }
                return Generate(layoutParams, templateCounts, generateCallCount + 1);
            }

            return CreateMap(genMap, startRoom, mapSize);
        }

        /// <summary>
        /// Initializes the starting room in the center of the map
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="roomContainer"> The room container </param>
        /// <param name="mapSize"> The map size </param>
        /// <param name="startRoomType"> The type of the starting room </param>
        /// <returns> The starting room </returns>
        private Room GenerateStartRoom(MapCell[,] genMap, GameObject roomContainer, Vector2Int mapSize, RoomType startRoomType)
        {
            Vector2Int startPos = new Vector2Int((mapSize.x + 1) / 2, (mapSize.y + 1) / 2);
            MapCell startCell = genMap[startPos.x, startPos.y];
            Room startRoom = CreateRoom(roomContainer, startRoomType, startCell.location);
            bool fits = GenerateRandomRoomLayout(genMap, startRoom, startCell, true);
            if (!fits)
            {
                throw new System.Exception("The start room somehow doesn't fit in the map");
            }
            return startRoom;
        }

        /// <summary>
        /// Generates all the normal rooms in the map
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="roomContainer"> The room container </param>
        /// <param name="startRoom"> The start room </param>
        /// <param name="normalRooms"> The normal rooms to be created and the number of them that should exist </param>
        /// <param name="emergencyRooms"> The emergency rooms, to be created if no other room fits in a location </param>
        /// <param name="preferredNumDoors"> The preferred number of doors for rooms to have </param>
        /// <param name="strictnessNumDoors"> The strictness with which the rooms adhere to the preferred number of doors </param>
        /// <returns> A list of branchable cells </returns>
        private List<MapCell> GenerateNormalRooms(MapCell[,] genMap, GameObject roomContainer, Room startRoom, Dictionary<RoomType, int> normalRooms, List<RoomType> emergencyRooms, int preferredNumDoors, float strictnessNumDoors)
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
                roomTypeCounts.Add(entry.Key, 0);
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

                // Unmark the cell as visited (it will be set again during generate random room layout, but need to be false to check if the room type fits)
                currentCell.visited = false;

                List<RoomType> possibleRoomTypes = normalRooms.Keys.ToList();
                Room newRoom;
                bool fits;
                do
                {
                    newRoom = CreateRoom(roomContainer, possibleRoomTypes[FloorGenerator.random.Next(0, possibleRoomTypes.Count)], startRoom.startLocation);
                    fits = GenerateRandomRoomLayout(genMap, newRoom, currentCell, false, preferredNumDoors, strictnessNumDoors, totalNormalRooms, newRoomsCount, cellsToGenerate.Count);
                    if (!fits)
                    {
                        possibleRoomTypes.Remove(newRoom.roomType);
                    }
                }
                while (!fits && possibleRoomTypes.Count > 0);

                if (possibleRoomTypes.Count == 0)
                {
                    if (emergencyRooms.Count == 0)
                    {
                        Debug.LogError("Generation failed due to lack of emergency rooms!");
                    }
                    // Unfortunately it's possible for this to happen if you don't have enough single-celled rooms. Creating an emergency room in the case makes it so it doesn't completely fail.
                    newRoom = CreateRoom(roomContainer, emergencyRooms[FloorGenerator.random.Next(0, possibleRoomTypes.Count)], startRoom.startLocation);
                    GenerateRandomRoomLayout(genMap, newRoom, currentCell, false, preferredNumDoors, strictnessNumDoors, totalNormalRooms, newRoomsCount, cellsToGenerate.Count);
                    Debug.LogWarning("An emergency room was created! Consider adding more single-celled rooms to the generation settings.");
                }
                else
                {
                    roomTypeCounts[newRoom.roomType]++;
                    if (roomTypeCounts[newRoom.roomType] >= normalRooms[newRoom.roomType])
                    {
                        normalRooms.Remove(newRoom.roomType);
                    }
                }

                if (newRoom.roomType.attachedRoom != null)
                {
                    List<MapCell> possibleCells = GetPossibleAttachedRoomCells(genMap, newRoom);
                    MapCell cell = possibleCells[FloorGenerator.random.Next(0, possibleCells.Count)];
                    Room attachedRoom = CreateRoom(roomContainer, newRoom.roomType.attachedRoom, startRoom.startLocation);
                    BranchRoom(genMap, newRoom, cell);
                    // Attached rooms must be a dead end
                    GenerateRandomRoomLayout(genMap, attachedRoom, cell, true);
                }

                // Add the edges of this room to the branchable cells
                foreach (MapCell edgeCell in GetEdgeCells(genMap, newRoom))
                {
                    branchableCells.Add(edgeCell);
                }

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
            }

            return branchableCells;
        }

        /// <summary>
        /// Generates the dead ends in the gen map
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="roomContainer"> The room container to put spawned rooms into </param>
        /// <param name="startLocation"> The location of the start room in the map </param>
        /// <param name="branchableCells"> The cells that are possible to branch off of </param>
        /// <param name="deadEnds"> The dead end rooms that need to be created </param>
        /// <returns> Whether or not the dead ends were successfully generated </returns>
        private bool GenerateDeadEnds(MapCell[,] genMap, GameObject roomContainer, Vector2Int startLocation, List<MapCell> branchableCells, Dictionary<RoomType, int> deadEnds)
        {
            Dictionary<RoomType, int> deadEndCounts = new Dictionary<RoomType, int>();

            foreach (KeyValuePair<RoomType, int> entry in deadEnds)
            {
                deadEndCounts.Add(entry.Key, 0);
            }

            while (deadEnds.Count > 0)
            {
                RoomType randomType = deadEnds.ElementAt(FloorGenerator.random.Next(0, deadEnds.Count)).Key;
                Room newRoom = CreateRoom(roomContainer, randomType, startLocation);

                List<MapCell> branchableCellsInstance = new List<MapCell>(branchableCells);

                while (branchableCellsInstance.Count > 0)
                {
                    MapCell branchCell = branchableCells[FloorGenerator.random.Next(0, branchableCells.Count)];

                    List<MapCell> possibleNewRoomCells = GetUnvisitedNeighbors(genMap, branchCell, false);
                    if (possibleNewRoomCells.Count == 0)
                    {
                        branchableCellsInstance.Remove(branchCell);
                        branchableCells.Remove(branchCell);
                        continue;
                    }

                    while (possibleNewRoomCells.Count > 0)
                    {
                        MapCell newRoomCell = possibleNewRoomCells[FloorGenerator.random.Next(0, possibleNewRoomCells.Count)];
                        BranchRoom(genMap, branchCell.room, newRoomCell);
                        if (!GenerateRandomRoomLayout(genMap, newRoom, newRoomCell, true))
                        {
                            UnbranchRoom(genMap, branchCell.room, newRoomCell);
                            possibleNewRoomCells.Remove(newRoomCell);
                            continue;
                        }

                        deadEndCounts[randomType]++;
                        if (deadEndCounts[randomType] == deadEnds[randomType])
                        {
                            deadEnds.Remove(randomType);
                        }

                        if (newRoom.roomType.attachedRoom != null)
                        {
                            List<MapCell> possibleCells = GetPossibleAttachedRoomCells(genMap, newRoom);
                            MapCell cell = possibleCells[FloorGenerator.random.Next(0, possibleCells.Count)];
                            Room attachedRoom = CreateRoom(roomContainer, newRoom.roomType.attachedRoom, startLocation);
                            BranchRoom(genMap, newRoom, cell);
                            // Attached rooms must be a dead end
                            GenerateRandomRoomLayout(genMap, attachedRoom, cell, true);
                        }

                        break;
                    }

                    if (possibleNewRoomCells.Count == 0)
                    {
                        branchableCellsInstance.Remove(branchCell);
                        continue;
                    }

                    break;
                }

                if (branchableCellsInstance.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region SINGLE_ROOM_GENERATION

        /// <summary>
        /// Creates a room with the given room type
        /// </summary>
        /// <param name="roomContainer"> The game object to hold all the room game objects in </param>
        /// <param name="roomType"> The room type of the room </param>
        /// <param name="startLocation"> The location of the start room in the map </param>
        /// <returns> The created room </returns>
        private Room CreateRoom(GameObject roomContainer, RoomType roomType, Vector2Int startLocation)
        {
            GameObject newRoom = new GameObject();
            newRoom.name = roomType.displayName + " Room";
            newRoom.transform.parent = roomContainer.transform;
            newRoom.AddComponent<TemplateGenerator>(); // Should probably make a prefab but whatevs
            Room roomComponent = newRoom.AddComponent<Room>();
            roomComponent.roomType = roomType;
            roomComponent.startLocation = startLocation;
            return roomComponent;
        }

        /// <summary>
        /// Gets a random layout for the room type
        /// </summary>
        /// <param name="genMap"> The currently generated map </param>
        /// <param name="room"> The room generate the layout for </param>
        /// <param name="generatedCell"> The cell that's being generated </param>
        /// <param name="deadEnd"> Whether or not this room should be generated as a dead end </param>
        /// <param name="preferredNumDoors"> The preferred number of doors for this room (NA if dead end) </param>
        /// <param name="strictnessNumDoors"> The strictness with which the room adheres to the preferred number of doors (NA if dead end) </param>
        /// <param name="targetNumRooms"> The target number of rooms to achieve (NA if dead end) </param>
        /// <param name="currentNumRooms"> The current number of rooms achieved (NA if dead end) </param>
        /// <param name="activeBranches"> The number of active branches in generation (active branches meaning branches that haven't dead ended yet, NA if dead end) </param>
        /// <returns> Whether or not this room actually fits in this location </returns>
        private bool GenerateRandomRoomLayout(MapCell[,] genMap, Room room, MapCell generatedCell, bool deadEnd = false, int preferredNumDoors = -1, float strictnessNumDoors = -1, int targetNumRooms = -1, int currentNumRooms = -1, int activeBranches = -1)
        {
            // Get the possible offsets
            List<Vector2Int> possibleOffsets = new List<Vector2Int>();

            if (room.roomType.useRandomOffset)
            {
                for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
                {
                    for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                    {
                        // -i and -j because 0, 0 is bottom left, and we need to move the room down left in order to not lose the room
                        possibleOffsets.Add(new Vector2Int(-i, -j));
                    }
                }
            }
            else
            {
                possibleOffsets.Add(room.roomType.offset);
            }


            while (possibleOffsets.Count > 0)
            {
                Vector2Int offset = possibleOffsets[FloorGenerator.random.Next(0, possibleOffsets.Count)];
                room.roomLocation = offset + generatedCell.location;

                int numDoors = 0;

                List<(MapCell, Direction)> directionsToReset = new List<(MapCell, Direction)>();

                foreach ((MapCell, Direction) requiredDirection in GetRequiredDirections(genMap, room))
                {
                    if (!requiredDirection.Item1.direction.HasFlag(requiredDirection.Item2))
                    {
                        directionsToReset.Add((requiredDirection.Item1, requiredDirection.Item2));
                    }
                    requiredDirection.Item1.direction |= requiredDirection.Item2;
                    numDoors++;
                }

                if (!CheckIfRoomFits(genMap, room))
                {
                    foreach ((MapCell, Direction) resetDirection in directionsToReset)
                    {
                        resetDirection.Item1.direction ^= resetDirection.Item2;
                    }
                    possibleOffsets.Remove(offset);
                    continue;
                }

                if (!deadEnd)
                {
                    SetRandomDirections(GetPossibleNewDirections(genMap, room), numDoors, preferredNumDoors, strictnessNumDoors, targetNumRooms, currentNumRooms, activeBranches);
                }

                SetRoomCells(genMap, room);
                return true;
            }

            return false;
        }

        #region ROOM_FITTING

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
            System.Action<Vector2Int> checkGenMap = (cellLocation) =>
            {
                if (genMap[cellLocation.x, cellLocation.y].visited)
                {
                    return;
                }

                for (int i = 0; i < room.roomType.attachedRoom.sizeMultiplier.x; i++)
                {
                    for (int j = 0; j < room.roomType.attachedRoom.sizeMultiplier.y; j++)
                    {
                        // -i and -j because 0, 0 is bottom left, and we need to move the room down left in order to not lose the room
                        Vector2Int offset = new Vector2Int(-i, -j);

                        // Double check that this attached room location doesn't overlap the room. This is needed because the room isn't set to visited yet
                        Vector2Int attachedMin = cellLocation + offset;
                        Vector2Int attachedMax = cellLocation + offset + room.roomType.attachedRoom.sizeMultiplier - new Vector2Int(1, 1);

                        Vector2Int roomMin = room.roomLocation;
                        Vector2Int roomMax = room.roomLocation + room.roomType.sizeMultiplier - new Vector2Int(1, 1);

                        bool overlapping = attachedMax.x >= roomMin.x && roomMax.x >= attachedMin.x;
                        overlapping &= attachedMax.y >= roomMin.y && roomMax.y >= attachedMin.y;
                        if (overlapping)
                        {
                            continue;
                        }

                        if (CheckIfRoomTypeFits(genMap, room.roomType.attachedRoom, cellLocation + offset) && !possibleCells.Contains(genMap[cellLocation.x, cellLocation.y]))
                        {
                            possibleCells.Add(genMap[cellLocation.x, cellLocation.y]);
                            return;
                        }
                    }
                }
            };

            foreach (MapCell doorCell in doorCells)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (direction == Direction.None || direction == Direction.All)
                    {
                        continue;
                    }
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
                                        else if (direction == Direction.Down)
                                        {
                                            mapLocationToCheck.y = room.roomLocation.y + room.roomType.sizeMultiplier.y;
                                        }
                                        mapLocationToCheck.x = room.roomLocation.x + i;

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

        #endregion ROOM_FITTING

        #region DIRECTION_GENERATION

        /// <summary>
        /// Gets the directions that the room is required to have
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the required directions for </param>
        /// <returns> The list of required directions </returns>
        private List<(MapCell, Direction)> GetRequiredDirections(MapCell[,] genMap, Room room)
        {
            List<(MapCell, Direction)> requiredDirections = new List<(MapCell, Direction)>();

            if (room.roomType.startRoom)
            {
                // The start room is hard-coded to have 4 doors (maybe someday I will make this not hard-coded but attached rooms are enough for me)
                requiredDirections.Add((genMap[room.roomLocation.x + room.roomType.sizeMultiplier.x - 1, room.roomLocation.y + (room.roomType.sizeMultiplier.y / 2)], Direction.Right));
                requiredDirections.Add((genMap[room.roomLocation.x + (room.roomType.sizeMultiplier.x / 2), room.roomLocation.y + room.roomType.sizeMultiplier.y - 1], Direction.Up));
                requiredDirections.Add((genMap[room.roomLocation.x, room.roomLocation.y + (room.roomType.sizeMultiplier.y / 2)], Direction.Left));
                requiredDirections.Add((genMap[room.roomLocation.x + (room.roomType.sizeMultiplier.x / 2), room.roomLocation.y], Direction.Down));
                return requiredDirections;
            }

            List<MapCell> neighbors = GetVisitedNeighbors(genMap, room);
            foreach (MapCell neighbor in neighbors)
            {
                if (neighbor.location.x == room.roomLocation.x + room.roomType.sizeMultiplier.x && neighbor.direction.HasFlag(Direction.Left))
                {
                    requiredDirections.Add((genMap[neighbor.location.x - 1, neighbor.location.y], Direction.Right));
                }
                else if (neighbor.location.y == room.roomLocation.y + room.roomType.sizeMultiplier.y && neighbor.direction.HasFlag(Direction.Down))
                {
                    requiredDirections.Add((genMap[neighbor.location.x, neighbor.location.y - 1], Direction.Up));
                }
                else if (neighbor.location.x == room.roomLocation.x - 1 && neighbor.direction.HasFlag(Direction.Right))
                {
                    requiredDirections.Add((genMap[neighbor.location.x + 1, neighbor.location.y], Direction.Left));
                }
                else if (neighbor.location.y == room.roomLocation.y - 1 && neighbor.direction.HasFlag(Direction.Up))
                {
                    requiredDirections.Add((genMap[neighbor.location.x, neighbor.location.y + 1], Direction.Down));
                }
            }

            return requiredDirections;
        }

        /// <summary>
        /// Gets the possible new directions 
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the possible directions for </param>
        /// <returns> A list of possible new directions </returns>
        private List<(MapCell, Direction)> GetPossibleNewDirections(MapCell[,] genMap, Room room)
        {
            List<(MapCell, Direction)> possibleDirections = new List<(MapCell, Direction)>();
            foreach (MapCell neighbor in GetUnvisitedNeighbors(genMap, room, false))
            {
                if (neighbor.visited && neighbor.direction != Direction.None)
                {
                    continue;
                }

                if (neighbor.location.x == room.roomLocation.x + room.roomType.sizeMultiplier.x && !genMap[neighbor.location.x - 1, neighbor.location.y].direction.HasFlag(Direction.Right))
                {
                    possibleDirections.Add((genMap[neighbor.location.x - 1, neighbor.location.y], Direction.Right));
                }
                else if (neighbor.location.y == room.roomLocation.y + room.roomType.sizeMultiplier.y && !genMap[neighbor.location.x, neighbor.location.y - 1].direction.HasFlag(Direction.Up))
                {
                    possibleDirections.Add((genMap[neighbor.location.x, neighbor.location.y - 1], Direction.Up));
                }
                else if (neighbor.location.x == room.roomLocation.x - 1 && !genMap[neighbor.location.x + 1, neighbor.location.y].direction.HasFlag(Direction.Left))
                {
                    possibleDirections.Add((genMap[neighbor.location.x + 1, neighbor.location.y], Direction.Left));
                }
                else if (neighbor.location.y == room.roomLocation.y - 1 && !genMap[neighbor.location.x, neighbor.location.y + 1].direction.HasFlag(Direction.Down))
                {
                    possibleDirections.Add((genMap[neighbor.location.x, neighbor.location.y + 1], Direction.Down));
                }
            }

            return possibleDirections;
        }

        /// <summary>
        /// Sets random cells to have random directions
        /// </summary>
        /// <param name="possibleDirections"> The cells and their possible directions </param>
        /// <param name="currentNumDoors"> The current number of doors in the room </param>
        /// <param name="preferredNumDoors"> The preferred number of doors in the room </param>
        /// <param name="strictnessNumDoors"> The strictness of the number of doors in the room </param>
        /// <param name="targetNumRooms"> The target number of rooms </param>
        /// <param name="currentNumRooms"> The current number of created rooms </param>
        /// <param name="activeBranches"> The number of active branches </param>
        private void SetRandomDirections(List<(MapCell, Direction)> possibleDirections, int currentNumDoors, int preferredNumDoors, float strictnessNumDoors, int targetNumRooms, int currentNumRooms, int activeBranches)
        {

            int initialNumDoors = currentNumDoors;
            // While it's still possible to open in another direction
            while (possibleDirections.Count != 0)
            {
                // If we're at the max number of directions this room is allowed to have, then stop adding more directions
                if (currentNumDoors - initialNumDoors >= targetNumRooms - currentNumRooms)
                {
                    break;
                }

                // Choose a random possible direction
                (MapCell, Direction) randomDirection = possibleDirections[FloorGenerator.random.Next(0, possibleDirections.Count)];

                // Get the "distance" from the number of directions this cell already has to the number of directions is preferred
                int distance = preferredNumDoors - currentNumDoors;

                // Make sure to add the direction if we still need more rooms
                bool mustHaveDirection = false;
                if (activeBranches <= 1)
                {
                    mustHaveDirection = true;
                }

                // Using the likelyhood from the distance, determine whether or not to add this direction            
                if (CalculateLikelyhoodOfAddingDoor(distance, strictnessNumDoors) > FloorGenerator.random.NextDouble() || mustHaveDirection)
                {
                    randomDirection.Item1.direction |= randomDirection.Item2;
                    currentNumDoors++;
                }

                // Whether the direction was added or not, remove it from the possible new directions
                possibleDirections.Remove(randomDirection);
            }
        }

        /// <summary>
        /// Calculates the likelyhood of a room having another door, using the distance from the number of doors the room
        /// already has to the desired number of doors
        /// </summary>
        /// <param name="distance"> The distance from the number of directions the room already has to the preferred number of doors </param>
        /// <param name="strictnessNumDoors"> The structness with which the rooms adhere to the preferred number of doors </param>
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

        #endregion DIRECTION_GENERATION

        #region CELL_MANIPULATION

        /// <summary>
        /// Sets the cells contained in a room to hold a reference to the room, and sets the cells to visited
        /// </summary>
        /// <param name="genMap"> The current generated map to set the cells of </param>
        /// <param name="room"> The room to set the cells of </param>
        private void SetRoomCells(MapCell[,] genMap, Room room)
        {
            for (int i = 0; i < room.roomType.sizeMultiplier.x; i++)
            {
                for (int j = 0; j < room.roomType.sizeMultiplier.y; j++)
                {
                    MapCell cell = genMap[i + room.roomLocation.x, j + room.roomLocation.y];
                    cell.room = room;
                    cell.visited = true;
                }
            }
        }

        /// <summary>
        /// Gets the edge cells of a room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the edge cells of </param>
        /// <returns> The list of edge cells </returns>
        private List<MapCell> GetEdgeCells(MapCell[,] genMap, Room room)
        {
            return room.GetEdgeCells(genMap);
        }

        /// <summary>
        /// Changes the room's direction so it connects to the given cell
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to branch </param>
        /// <param name="cell"> The cell to connect to </param>
        private void BranchRoom(MapCell[,] genMap, Room room, MapCell cell)
        {
            if (cell.location.x == room.roomLocation.x + room.roomType.sizeMultiplier.x)
            {
                genMap[cell.location.x - 1, cell.location.y].direction |= Direction.Right;
            }
            else if (cell.location.y == room.roomLocation.y + room.roomType.sizeMultiplier.y)
            {
                genMap[cell.location.x, cell.location.y - 1].direction |= Direction.Up;
            }
            else if (cell.location.x == room.roomLocation.x - 1)
            {
                genMap[cell.location.x + 1, cell.location.y].direction |= Direction.Left;
            }
            else if (cell.location.y == room.roomLocation.y - 1)
            {
                genMap[cell.location.x, cell.location.y + 1].direction |= Direction.Down;
            }
        }

        /// <summary>
        /// Changes the room's direction so it no longer connects to the given cell
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to unbranch </param>
        /// <param name="cell"> The cell to un-connect to </param>
        private void UnbranchRoom(MapCell[,] genMap, Room room, MapCell cell)
        {
            if (cell.location.x == room.roomLocation.x + room.roomType.sizeMultiplier.x && genMap[cell.location.x - 1, cell.location.y].direction.HasFlag(Direction.Right))
            {
                genMap[cell.location.x - 1, cell.location.y].direction ^= Direction.Right;
            }
            else if (cell.location.y == room.roomLocation.y + room.roomType.sizeMultiplier.y && genMap[cell.location.x, cell.location.y - 1].direction.HasFlag(Direction.Up))
            {
                genMap[cell.location.x, cell.location.y - 1].direction ^= Direction.Up;
            }
            else if (cell.location.x == room.roomLocation.x - 1 && genMap[cell.location.x + 1, cell.location.y].direction.HasFlag(Direction.Left))
            {
                genMap[cell.location.x + 1, cell.location.y].direction ^= Direction.Left;
            }
            else if (cell.location.y == room.roomLocation.y - 1 && genMap[cell.location.x, cell.location.y + 1].direction.HasFlag(Direction.Down))
            {
                genMap[cell.location.x, cell.location.y + 1].direction ^= Direction.Down;
            }
        }

        #endregion CELL_MANIPULATION

        #region NEIGHBOR_GETTING

        /// <summary>
        /// Gets all the unvisited neighbors of the given room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the neighbors of </param>
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
                    if (direction == Direction.None || direction == Direction.All)
                    {
                        continue;
                    }
                    bool checkDirection = !useDirection || (useDirection && edge.direction.HasFlag(direction));

                    Vector2Int locationOffset = new Vector2Int();
                    locationOffset.x = System.Convert.ToInt32(direction.HasFlag(Direction.Right)) - System.Convert.ToInt32(direction.HasFlag(Direction.Left));
                    locationOffset.y = System.Convert.ToInt32(direction.HasFlag(Direction.Up)) - System.Convert.ToInt32(direction.HasFlag(Direction.Down));
                    bool locationOutsideRoom = locationOffset.x + edge.location.x < room.roomLocation.x || locationOffset.x + edge.location.x >= room.roomLocation.x + room.roomType.sizeMultiplier.x;
                    locationOutsideRoom |= locationOffset.y + edge.location.y < room.roomLocation.y || locationOffset.y + edge.location.y >= room.roomLocation.y + room.roomType.sizeMultiplier.y;

                    MapCell neighbor = genMap[edge.location.x + locationOffset.x, edge.location.y + locationOffset.y];
                    if (checkDirection && locationOutsideRoom && !neighbor.visited)
                    {
                        neighbors.Add(neighbor); ;
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Gets the unvisited neighbors of a cell (as opposed to a room)
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="cell"> The cell to get the neighbors of </param>
        /// <param name="useDirection"> Whether or not to use the directions the cell opens in to get the neighbors </param>
        /// <returns> The unvisited neighbors </returns>
        private List<MapCell> GetUnvisitedNeighbors(MapCell[,] genMap, MapCell cell, bool useDirection)
        {
            List<MapCell> neighbors = new List<MapCell>();

            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                if (direction == Direction.None || direction == Direction.All)
                {
                    continue;
                }
                bool checkDirection = !useDirection || (useDirection && cell.direction.HasFlag(direction));

                Vector2Int locationOffset = new Vector2Int();
                locationOffset.x = System.Convert.ToInt32(direction.HasFlag(Direction.Right)) - System.Convert.ToInt32(direction.HasFlag(Direction.Left));
                locationOffset.y = System.Convert.ToInt32(direction.HasFlag(Direction.Up)) - System.Convert.ToInt32(direction.HasFlag(Direction.Down));

                if (checkDirection && !genMap[cell.location.x + locationOffset.x, cell.location.y + locationOffset.y].visited)
                {
                    neighbors.Add(genMap[cell.location.x + locationOffset.x, cell.location.y + locationOffset.y]);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Gets all the visited neighbors of the given room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="room"> The room to get the neighbors of </param>
        /// <returns> The visited neighbors </returns>
        private List<MapCell> GetVisitedNeighbors(MapCell[,] genMap, Room room)
        {
            List<MapCell> neighbors = new List<MapCell>();

            List<MapCell> edges = GetEdgeCells(genMap, room);
            foreach (MapCell edge in edges)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (direction == Direction.None || direction == Direction.All)
                    {
                        continue;
                    }
                    Vector2Int locationOffset = new Vector2Int();
                    locationOffset.x = System.Convert.ToInt32(direction.HasFlag(Direction.Right)) - System.Convert.ToInt32(direction.HasFlag(Direction.Left));
                    locationOffset.y = System.Convert.ToInt32(direction.HasFlag(Direction.Up)) - System.Convert.ToInt32(direction.HasFlag(Direction.Down));
                    bool locationOutsideRoom = locationOffset.x + edge.location.x < room.roomLocation.x || locationOffset.x + edge.location.x >= room.roomLocation.x + room.roomType.sizeMultiplier.x;
                    locationOutsideRoom |= locationOffset.y + edge.location.y < room.roomLocation.y || locationOffset.y + edge.location.y >= room.roomLocation.y + room.roomType.sizeMultiplier.y;

                    MapCell neighbor = genMap[edge.location.x + locationOffset.x, edge.location.y + locationOffset.y];

                    if (locationOutsideRoom && neighbor.visited && !neighbors.Contains(neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        #endregion

        #endregion
    }
}
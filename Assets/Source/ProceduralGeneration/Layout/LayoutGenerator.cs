using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Generates the layout of a floor
    /// </summary>
    public class LayoutGenerator : MonoBehaviour
    {
        /// <summary>
        /// Generates the layout
        /// </summary>
        public Map Generate(LayoutGenerationParameters layoutParameters)
        {
            int numNormalRooms = 0;
            int numDeadEndRooms = 0;
            List<RoomType> normalRooms = new List<RoomType>();
            List<RoomType> deadEndRooms = new List<RoomType>();

            foreach (RoomTypeToLayoutParameters roomTypeToLayoutParameters in layoutParameters.roomTypesTolayoutParameters.roomTypesToLayoutParameters)
            {
                int variance = roomTypeToLayoutParameters.numRooomsVariance;
                int numRooms = roomTypeToLayoutParameters.numRooms;
                numRooms += FloorGenerator.random.Next(-variance, variance + 1);
                if (roomTypeToLayoutParameters.roomType.deadEnd)
                {
                    numDeadEndRooms += numRooms;
                    deadEndRooms.Add(roomTypeToLayoutParameters.roomType);
                }
                else
                {
                    numNormalRooms += numRooms;
                    normalRooms.Add(roomTypeToLayoutParameters.roomType);
                }
            }
            
            // Initialize the gen map
            Vector2Int mapSize = DetermineMapSize(numNormalRooms + numDeadEndRooms);
            MapCell[,] genMap = InitializeGenMap(mapSize);

            // Create the starting cell
            MapCell startCell = GenerateStartCell(genMap, mapSize);

            // Get all the branchable cells (which will start out as all the normal cells, then cells will be removed from them as it goes along
            List<MapCell> branchableCells = GenerateNormalCells(genMap, startCell, numNormalCells, layoutParameters);

            // If somehow (this should never happen) The boss or special cells failed to generate, just start over
            if (!GenerateBossAndExitCells(genMap, branchableCells))
            {
                // This should never happen butttt y'know just in case
                return Generate(layoutParameters);
            }
            if (!GenerateSpecialCells(genMap, branchableCells, layoutParameters.numSpecialRooms))
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
        /// Initializes the starting cell in the center of the map
        /// </summary>
        /// <param name="genMap"> The map that is being generated </param>
        /// <returns> The start cell </returns>
        private MapCell GenerateStartCell(MapCell[,] genMap, Vector2Int mapSize)
        {
            Vector2Int startPos = new Vector2Int((mapSize.x + 1) / 2, (mapSize.y + 1) / 2);
            MapCell startRoom = genMap[startPos.x, startPos.y];
            startRoom.visited = true;
            startRoom.type = RoomType.Start;
            startRoom.direction = Direction.All;
            return startRoom;
        }

        /// <summary>
        /// Generates all the normal cells in the map
        /// </summary>
        /// <param name="genMap"> The map being generated </param>
        /// <param name="startCell"> The starting cell </param>
        /// <param name="numNormalCells"> The number of normal cells to generate </param>
        /// <returns> A list of all the normal cells </returns>
        private List<MapCell> GenerateNormalCells(MapCell[,] genMap, MapCell startCell, int numNormalCells, LayoutGenerationParameters layoutParameters)
        {
            // Track the normal cells created
            List<MapCell> normalCells = new List<MapCell>();

            // Track the cells that still need to be generated
            Queue<MapCell> cellsToGenerate = new Queue<MapCell>();
            int newCellsCount = 0;

            // Add the neighbors of the start cell to the cells to generate
            List<MapCell> initialNeighbors = GetUnvisitedNeighbors(genMap, startCell, true);
            while (initialNeighbors.Count != 0)
            {
                MapCell randomInitialNeighbor = initialNeighbors[FloorGenerator.random.Next(0, initialNeighbors.Count)];
                randomInitialNeighbor.visited = true;
                cellsToGenerate.Enqueue(randomInitialNeighbor);
                initialNeighbors.Remove(randomInitialNeighbor);
                newCellsCount++;
            }

            // Generate all the cells using BFS
            while (cellsToGenerate.Count != 0)
            {
                // Generate the current cell
                MapCell currentCell = cellsToGenerate.Peek();
                normalCells.Add(currentCell);
                currentCell.type = RoomType.Normal;

                // The direction should not make it so the number of added neighbors exceeds the numNormalCells limit
                DirectionConstraint constraint = GetDirectionConstraint(genMap, currentCell, numNormalCells, newCellsCount);
                currentCell.direction = GetRandomConstrainedDirection(constraint, layoutParameters.preferredNumDoors, layoutParameters.strictnessNumDoors);

                // Remove the current cell from the queue
                cellsToGenerate.Dequeue();

                // Add the neighbors of the cell to the queue
                List<MapCell> neighbors = GetUnvisitedNeighbors(genMap, currentCell, true);
                while (neighbors.Count != 0)
                {
                    MapCell randomNeighbor = neighbors[FloorGenerator.random.Next(0, neighbors.Count)];
                    randomNeighbor.visited = true;
                    cellsToGenerate.Enqueue(randomNeighbor);
                    neighbors.Remove(randomNeighbor);
                    newCellsCount++;
                }
            }

            return normalCells;
        }

        /// <summary>
        /// Gets all the unvisited neighbors of the given cell
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="cell"> The cell to get the neighbors of </param>
        /// <param name="useDirection"> Whether or not to use the directions that the given cell opens in to get the neighbors </param>
        /// <returns> The unvisited neighbors </returns>
        private List<MapCell> GetUnvisitedNeighbors(MapCell[,] genMap, MapCell cell, bool useDirection)
        {
            List<MapCell> neighbors = new List<MapCell>();

            for (int i = 1; i <= (int)Direction.Down; i *= 2)
            {
                bool checkDirection = !useDirection || (useDirection && (cell.direction & (Direction)i) != Direction.None);
                Vector2Int locationOffset = new Vector2Int();
                locationOffset.x = System.Convert.ToInt32(((Direction)i & Direction.Right) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Left) != Direction.None);
                locationOffset.y = System.Convert.ToInt32(((Direction)i & Direction.Up) != Direction.None) - System.Convert.ToInt32(((Direction)i & Direction.Down) != Direction.None);
                if (checkDirection && !genMap[cell.location.x + locationOffset.x, cell.location.y + locationOffset.y].visited)
                {
                    neighbors.Add(genMap[cell.location.x + locationOffset.x, cell.location.y + locationOffset.y]);
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
        /// Calculates the likelyhood of a cell having another door, using the distance from the number of directions the cell
        /// already has to the desired number of directions
        /// </summary>
        /// <param name="distance"> The distance from the number of directions the cell already has to the preferred number of directions </param>
        /// <returns> The likelyood </returns>
        private float CalculateLikelyhoodOfAddingDirection(int distance, float strictnessNumDoors)
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
                    count += ((int)directionConstraint.mustHave & i) / i;
                }

                directionConstraint.maxDirections = maxNewCells - currentNewCells + count;
            }

            return directionConstraint;
        }

        /// <summary>
        /// Generates the boss and exit cells by branching off one the branchable cells
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="branchableCells"> The cells that can be branched off of </param>
        /// <returns> Whether or not the boss room was successfully generated </returns>
        private bool GenerateBossAndExitCells(MapCell[,] genMap, List<MapCell> branchableCells)
        {
            // While the cells haven't been generated
            while (true)
            {
                if (branchableCells.Count == 0)
                {
                    // This should never happen, but if it does then sadge
                    return false;
                }

                // Choose a random cell to branch off of
                MapCell branchCell = branchableCells[FloorGenerator.random.Next(0, branchableCells.Count)];

                List<Direction> directions = new List<Direction>();
                directions.Add(Direction.Right);
                directions.Add(Direction.Up);
                directions.Add(Direction.Left);
                directions.Add(Direction.Down);

                // See if the boss room can fit in any of the directions
                while (directions.Count > 0)
                {
                    Direction randomDirection = directions[FloorGenerator.random.Next(0, directions.Count)];

                    // Check directions (yes yes it's hard coded and 1000% terrible I know)

                    bool checkRight = (randomDirection & Direction.Right) != Direction.None;
                    if (checkRight && Check3x3Grid(genMap, branchCell.location + new Vector2Int(2, 0)) && !genMap[branchCell.location.x + 4, branchCell.location.y].visited)
                    {
                        Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(2, 0));
                        genMap[branchCell.location.x + 1, branchCell.location.y].direction = Direction.Left;
                        genMap[branchCell.location.x + 3, branchCell.location.y].direction = Direction.Right;
                        genMap[branchCell.location.x + 4, branchCell.location.y].visited = true;
                        genMap[branchCell.location.x + 4, branchCell.location.y].type = RoomType.Exit;
                        genMap[branchCell.location.x + 4, branchCell.location.y].direction = Direction.Left;
                        branchCell.direction |= Direction.Right;
                        return true;
                    }

                    bool checkUp = (randomDirection & Direction.Up) != Direction.None;
                    if (checkUp && Check3x3Grid(genMap, branchCell.location + new Vector2Int(0, 2)) && !genMap[branchCell.location.x, branchCell.location.y + 4].visited)
                    {
                        Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(0, 2));
                        genMap[branchCell.location.x, branchCell.location.y + 1].direction = Direction.Down;
                        genMap[branchCell.location.x, branchCell.location.y + 3].direction = Direction.Up;
                        genMap[branchCell.location.x, branchCell.location.y + 4].visited = true;
                        genMap[branchCell.location.x, branchCell.location.y + 4].type = RoomType.Exit;
                        genMap[branchCell.location.x, branchCell.location.y + 4].direction = Direction.Down;
                        branchCell.direction |= Direction.Up;
                        return true;
                    }

                    bool checkLeft = (randomDirection & Direction.Left) != Direction.None;
                    if (checkLeft && Check3x3Grid(genMap, branchCell.location + new Vector2Int(-2, 0)) && !genMap[branchCell.location.x - 4, branchCell.location.y].visited)
                    {
                        Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(-2, 0));
                        genMap[branchCell.location.x - 1, branchCell.location.y].direction = Direction.Right;
                        genMap[branchCell.location.x - 3, branchCell.location.y].direction = Direction.Left;
                        genMap[branchCell.location.x - 4, branchCell.location.y].visited = true;
                        genMap[branchCell.location.x - 4, branchCell.location.y].type = RoomType.Exit;
                        genMap[branchCell.location.x - 4, branchCell.location.y].direction = Direction.Right;
                        branchCell.direction |= Direction.Left;
                        return true;
                    }

                    bool checkDown = (randomDirection & Direction.Down) != Direction.None;
                    if (checkDown && Check3x3Grid(genMap, branchCell.location + new Vector2Int(0, -2)) && !genMap[branchCell.location.x, branchCell.location.y - 4].visited)
                    {
                        Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(0, -2));
                        genMap[branchCell.location.x, branchCell.location.y - 1].direction = Direction.Up;
                        genMap[branchCell.location.x, branchCell.location.y - 3].direction = Direction.Down;
                        genMap[branchCell.location.x, branchCell.location.y - 4].visited = true;
                        genMap[branchCell.location.x, branchCell.location.y - 4].type = RoomType.Exit;
                        genMap[branchCell.location.x, branchCell.location.y - 4].direction = Direction.Up;
                        branchCell.direction |= Direction.Down;
                        return true;
                    }

                    directions.Remove(randomDirection);
                }

                // If the boss room couldn't fit, then remove this cell from branchable cells and try again
                branchableCells.Remove(branchCell);
            }
        }

        /// <summary>
        /// This function is sad and hopefully I will change this eventually
        /// Checks a 3x3 grid on the genMap, making sure it's clear
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="middlePoint"> The middle point of the 3x3 grid </param>
        /// <returns> Whether or not this 3x3 is clear </returns>
        private bool Check3x3Grid(MapCell[,] genMap, Vector2Int middlePoint)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (genMap[middlePoint.x + i, middlePoint.y + j].visited)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// This function is also sad
        /// Sets a 3x3 to be the boss room
        /// </summary>
        /// <param name="genMap"> The current generated map </param>
        /// <param name="middlePoint"> The middle point of the 3x3 grid </param>
        private void Set3x3GridToBoss(MapCell[,] genMap, Vector2Int middlePoint)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    genMap[middlePoint.x + i, middlePoint.y + j].visited = true;
                    genMap[middlePoint.x + i, middlePoint.y + j].type = RoomType.Boss;
                }
            }
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

        /// <summary>
        /// Saves the given map to a file
        /// </summary>
        /// <param name="mapToSave"> The map to save </param>
        public void SaveMap(Map mapToSave)
        {
            string mapString = "";
            for (int i = 0; i < mapToSave.mapSize.y; i++)
            {
                string line = "";
                for (int j = 0; j < mapToSave.mapSize.x; j++)
                {
                    if (mapToSave.map[j, i].type == RoomType.None)
                    {
                        line += "-";
                    }
                    else if (mapToSave.map[j, i].type == RoomType.Normal)
                    {
                        line += "*";
                    }
                    else if (mapToSave.map[j, i].type == RoomType.Start)
                    {
                        line += "P";
                    }
                    else if (mapToSave.map[j, i].type == RoomType.Special)
                    {
                        line += "S";
                    }
                    else if (mapToSave.map[j, i].type == RoomType.Boss)
                    {
                        line += "B";
                    }
                    else if (mapToSave.map[j, i].type == RoomType.Exit)
                    {
                        line += "E";
                    }
                    else
                    {
                        line += " ";
                    }
                }
                mapString = line + "\n" + mapString;
            }
            System.IO.File.WriteAllText("map.txt", mapString);

            Debug.Log("Saved to file map.txt");
        }
    }
}
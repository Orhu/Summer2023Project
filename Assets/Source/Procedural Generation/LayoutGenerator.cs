using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [Tooltip("The parameters for controlling the layout generation")]
    public LayoutGenerationParameters parameters;

    // The map of the rooms
    [HideInInspector]
    public MapCell[,] map;

    // The starting cell
    [HideInInspector]
    public MapCell startCell;

    // The map size
    [HideInInspector]
    public Vector2Int mapSize;

    void Start()
    {
        Generate();
        PrintMap();
    }

    void Generate()
    {
        // Get the number of normal cells
        int numNormalCells = parameters.numNormalRooms + Random.Range(0, parameters.numNormalRoomsVariance * 2 + 1) - parameters.numNormalRoomsVariance;

        // Initialize the gen map
        MapCell[,] genMap = InitializeGenMap(numNormalCells + parameters.numSpecialRooms);

        // Create the starting cell
        startCell = GenerateStartCell(genMap);
       
        // Get all the branchable cells (which will start out as all the normal cells, then cells will be removed from them as it goes along
        List<MapCell> branchableCells = GenerateNormalCells(genMap, startCell, numNormalCells);

        // If somehow (this should never happen) The boss or special cells failed to generate, just start over
        if (!GenerateBossAndExitCells(genMap, branchableCells))
        {
            // This should never happen butttt y'know just in case
            Generate();
        }
        if (!GenerateSpecialCells(genMap, branchableCells, parameters.numSpecialRooms))
        {
            Generate();
        }

        CreateMap(genMap);
    }

    MapCell[,] InitializeGenMap(int numRooms)
    {
        // Make the map size to be able to hold all the rooms in a row in any direction (worst case scenario)
        // +5 because boss room is 3x3, then you have the exit room, and finally you have the start room in the middle.
        // TODO: Change boos room size to not be hard coded lol
        mapSize = new Vector2Int(numRooms * 2 + 7, numRooms * 2 + 7);

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

    MapCell GenerateStartCell(MapCell[,] genMap)
    {
        Vector2Int startPos = new Vector2Int((mapSize.x + 1) / 2, (mapSize.y + 1) / 2);
        MapCell startRoom = genMap[startPos.x, startPos.y];
        startRoom.visited = true;
        startRoom.type = RoomType.Start;
        startRoom.direction = Direction.All;
        return startRoom;
    }

    List<MapCell> GenerateNormalCells(MapCell[,] genMap, MapCell startCell, int numNormalCells)
    {
        Debug.Log("Num normal cells: " + numNormalCells);
        // Track the normal cells created
        List<MapCell> normalCells = new List<MapCell>();

        // Track the cells that still need to be generated
        Queue<MapCell> cellsToGenerate = new Queue<MapCell>();
        int newCellsCount = 0;

        // Add the neighbors of the start cell to the cells to generate
        List<MapCell> initialNeighbors = GetUnvisitedNeighbors(genMap, startCell, true);
        while (initialNeighbors.Count != 0)
        {
            MapCell randomInitialNeighbor = initialNeighbors[Random.Range(0, initialNeighbors.Count)];
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

            Debug.Log("Generating normal cell: " + currentCell.location);

            // The direction should not make it so the number of added neighbors exceeds the numNormalCells limit
            currentCell.direction = GetRandomConstrainedDirection(GetDirectionConstraint(genMap, currentCell, numNormalCells, newCellsCount));

            // Remove the current cell from the queue
            cellsToGenerate.Dequeue();

            // Add the neighbors of the cell to the queue
            List<MapCell> neighbors = GetUnvisitedNeighbors(genMap, currentCell, true);
            while (neighbors.Count != 0)
            {
                MapCell randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                randomNeighbor.visited = true;
                cellsToGenerate.Enqueue(randomNeighbor);
                neighbors.Remove(randomNeighbor);
                newCellsCount++;
            }
        }

        return normalCells;
    }

    List<MapCell> GetUnvisitedNeighbors(MapCell[,] genMap, MapCell cell, bool useDirection)
    {
        List<MapCell> neighbors = new List<MapCell>();

        bool checkRightDirection = !useDirection || (useDirection && (cell.direction & Direction.Right) != Direction.None);
        if (checkRightDirection && !genMap[cell.location.x + 1, cell.location.y].visited)
        {
            neighbors.Add(genMap[cell.location.x + 1, cell.location.y]);
        }

        bool checkUpDirection = !useDirection || (useDirection && (cell.direction & Direction.Up) != Direction.None);
        if (checkUpDirection && !genMap[cell.location.x, cell.location.y + 1].visited)
        {
            neighbors.Add(genMap[cell.location.x, cell.location.y + 1]);
        }

        bool checkLeftDirection = !useDirection || (useDirection && (cell.direction & Direction.Left) != Direction.None);
        if (checkLeftDirection && !genMap[cell.location.x - 1, cell.location.y].visited)
        {
            neighbors.Add(genMap[cell.location.x - 1, cell.location.y]);
        }

        bool checkDownDirection = !useDirection || (useDirection && (cell.direction & Direction.Down) != Direction.None);
        if (checkDownDirection && !genMap[cell.location.x, cell.location.y - 1].visited)
        {
            neighbors.Add(genMap[cell.location.x, cell.location.y - 1]);
        }

        return neighbors;
    }

    int CountNumDirections(Direction direction)
    {
        int count = 0;
        for (int i = 1; i < (int)Direction.All; i *= 2)
        {
            count += ((int) direction & i) / i;
        }
        return count;
    }

    Direction GetRandomConstrainedDirection(DirectionConstraint constraint)
    {
        Direction direction = Direction.None;
        int numDirections = 0;

        List<Direction> possibleDirections = new List<Direction>();
        possibleDirections.Add(Direction.Right);
        possibleDirections.Add(Direction.Up);
        possibleDirections.Add(Direction.Left);
        possibleDirections.Add(Direction.Down);

        for (int i = 1; i <= (int) Direction.Down; i *= 2)
        {
            if ((constraint.mustHave & (Direction) i) != Direction.None)
            {
                possibleDirections.Remove((Direction) i);
                numDirections++;
                direction |= (Direction) i;
            }
        }

        // This should never happen but you never know
        if (numDirections > constraint.maxDirections)
        {
            throw new System.Exception("A room was generated that must have more directions than it's allowed");
        }

        while (possibleDirections.Count != 0)
        {
            if (numDirections >= constraint.maxDirections)
            {
                break;
            }

            Direction randomDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];

            if ((constraint.mustNotHave & randomDirection) != Direction.None)
            {
                possibleDirections.Remove(randomDirection);
                continue;
            }

            int distance = parameters.preferredNumDoors - numDirections;
            float likelyhood = 0;

            // Temp -- remove this maybe ? idk
            if (distance == 0 && parameters.strictnessNumDoors == 1)
            {
                likelyhood = 0;
            }
            else
            {
                likelyhood = 0.5f + (distance / 4.0f * (parameters.strictnessNumDoors * 2.0f));
            }
            
            if (likelyhood > Random.value)
            {
                direction |= randomDirection;
                numDirections++;
            }

            possibleDirections.Remove(randomDirection);
        }
        return direction;
    }

    /*Direction GetRandomConstrainedDirection(DirectionConstraint constraint, int maxDirections = -1, int minDirections = - 1)
    {
        
        // Get a random direction that adheres to the must have and must not have constraints
        Direction direction = (Direction) Random.Range((int) Direction.None, (int) Direction.All + 1);
        direction |= constraint.mustHave;
        Direction invertedMustNotHave = ~constraint.mustNotHave;
        direction &= invertedMustNotHave;

        List<Direction> directionsList = new List<Direction>();
        directionsList.Add(Direction.Right);
        directionsList.Add(Direction.Up);
        directionsList.Add(Direction.Left);
        directionsList.Add(Direction.Down);

        List<Direction> directions = new List<Direction>();

        // Count the number of directions
        int count = 0;
        for (int i = 1; i < (int) Direction.All; i *= 2)
        {
            count += ((int) direction & i) / i;
            if (((int) direction & i) != 0)
            {
                directions.Add((Direction) i);
            }
        }

        // If it doesn't constrain to the max number of directions, then remove random directions
        while (count > constraint.maxDirections)
        {
            if (directions.Count == 0)
            {
                throw new System.Exception("A room was generated that must have more directions than it's allowed");
            }

            // Get the random direction to remove
            Direction removedDirection = directions[Random.Range(0, directions.Count)];

            // Make sure this direction is not required
            if ((removedDirection & constraint.mustHave) != Direction.None)
            {
                directions.Remove(removedDirection);
                continue;
            }

            // Remove the direction
            Direction invertedRemoved = ~removedDirection;
            direction &= invertedRemoved;
            directions.Remove(removedDirection);
            count--;
        }

        return direction;
    }*/

    DirectionConstraint GetDirectionConstraint(MapCell[,] genMap, MapCell cell, int maxNewCells = -1, int currentNewCells = -1)
    {
        DirectionConstraint directionConstraint = new DirectionConstraint();

        // Check the right neighbor
        MapCell rightNeighbor = genMap[cell.location.x + 1, cell.location.y];

        if (rightNeighbor.visited)
        {
            // If it's been visited and it has a door there, make sure to generate a door in that direction
            if ((rightNeighbor.direction & Direction.Left) != Direction.None)
            {
                directionConstraint.mustHave |= Direction.Right;
            }
            // If it doesn't have a door there (or hasn't been generated yet), make sure to not put a door there
            else
            {
                directionConstraint.mustNotHave |= Direction.Right;
            }
        }
        
        // Check the up neighbor
        MapCell upNeighbor = genMap[cell.location.x, cell.location.y + 1];
        if (upNeighbor.visited)
        {
            if ((upNeighbor.direction & Direction.Down) != Direction.None)
            {
                directionConstraint.mustHave |= Direction.Up;
            }
            else
            {
                directionConstraint.mustNotHave |= Direction.Up;
            }
        }
        
        // Check the left neighbor
        MapCell leftNeighbor = genMap[cell.location.x - 1, cell.location.y];
        if (leftNeighbor.visited)
        {
            if ((leftNeighbor.direction & Direction.Right) != Direction.None)
            {
                directionConstraint.mustHave |= Direction.Left;
            }
            else
            {
                directionConstraint.mustNotHave |= Direction.Left;
            }
        }
        
        // Check the down neighbor
        MapCell downNeighbor = genMap[cell.location.x, cell.location.y - 1];
        if (downNeighbor.visited)
        {
            if ((downNeighbor.direction & Direction.Up) != Direction.None)
            {
                directionConstraint.mustHave |= Direction.Down;
            }
            else
            {
                directionConstraint.mustNotHave |= Direction.Down;
            }
        }

        // If we do care about the number of max new cells, then set the max number
        if (maxNewCells != -1)
        {
            // Count the number of directions the room must have (because they don't count towards new rooms)
            int count = 0;
            for (int i = 1; i < (int) Direction.All; i *= 2)
            {
                count += ((int) directionConstraint.mustHave & i) / i;
            }

            directionConstraint.maxDirections = maxNewCells - currentNewCells + count;
        }

        return directionConstraint;
    }

    bool GenerateBossAndExitCells(MapCell[,] genMap, List<MapCell> branchableCells)
    {
        while (true)
        {
            if (branchableCells.Count == 0)
            {
                // This should never happen, but if it does then sadge
                return false;
            }

            MapCell branchCell = branchableCells[Random.Range(0, branchableCells.Count)];

            List<Direction> directions = new List<Direction>();
            directions.Add(Direction.Right);
            directions.Add(Direction.Up);
            directions.Add(Direction.Left);
            directions.Add(Direction.Down);

            while (directions.Count > 0)
            {
                Direction randomDirection = directions[Random.Range(0, directions.Count)];

                // Check directions (yes yes it's hard coded and 1000% terrible I know)

                bool checkRight = (randomDirection & Direction.Right) != Direction.None;
                if (checkRight && Check3x3Grid(genMap, branchCell.location + new Vector2Int(2, 0)) && !genMap[branchCell.location.x + 4, branchCell.location.y].visited)
                {
                    Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(2, 0));
                    genMap[branchCell.location.x + 4, branchCell.location.y].visited = true;
                    genMap[branchCell.location.x + 4, branchCell.location.y].type = RoomType.Exit;
                    branchCell.direction |= Direction.Right;
                    return true;
                }

                bool checkUp = (randomDirection & Direction.Up) != Direction.None;
                if (checkUp && Check3x3Grid(genMap, branchCell.location + new Vector2Int(0, 2)) && !genMap[branchCell.location.x, branchCell.location.y + 4].visited)
                {
                    Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(0, 2));
                    genMap[branchCell.location.x, branchCell.location.y + 4].visited = true;
                    genMap[branchCell.location.x, branchCell.location.y + 4].type = RoomType.Exit;
                    branchCell.direction |= Direction.Up;
                    return true;
                }

                bool checkLeft = (randomDirection & Direction.Left) != Direction.None;
                if (checkLeft && Check3x3Grid(genMap, branchCell.location + new Vector2Int(-2, 0)) && !genMap[branchCell.location.x - 4, branchCell.location.y].visited)
                {
                    Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(-2, 0));
                    genMap[branchCell.location.x - 4, branchCell.location.y].visited = true;
                    genMap[branchCell.location.x - 4, branchCell.location.y].type = RoomType.Exit;
                    branchCell.direction |= Direction.Left;
                    return true;
                }

                bool checkDown = (randomDirection & Direction.Down) != Direction.None;
                if (checkDown && Check3x3Grid(genMap, branchCell.location + new Vector2Int(0, -2)) && !genMap[branchCell.location.x, branchCell.location.y - 4].visited)
                {
                    Set3x3GridToBoss(genMap, branchCell.location + new Vector2Int(0, -2));
                    genMap[branchCell.location.x, branchCell.location.y - 4].visited = true;
                    genMap[branchCell.location.x, branchCell.location.y - 4].type = RoomType.Exit;
                    branchCell.direction |= Direction.Down;
                    return true;
                }

                directions.Remove(randomDirection);
            }

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
    bool Check3x3Grid(MapCell[,] genMap, Vector2Int middlePoint)
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
    void Set3x3GridToBoss(MapCell[,] genMap, Vector2Int middlePoint)
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

    bool GenerateSpecialCells(MapCell[,] genMap, List<MapCell> branchableCells, int numSpecialCells)
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
                MapCell branchCell = branchableCells[Random.Range(0, branchableCells.Count)];

                // Get its unvisited neighbors
                List<MapCell> neighbors = GetUnvisitedNeighbors(genMap, branchCell, false);

                // Check that there actually are any neighbors
                if (neighbors.Count == 0)
                {
                    branchableCells.Remove(branchCell);
                    continue;
                }

                // Choose a random neighbor to be the special room
                MapCell specialCell = neighbors[Random.Range(0, neighbors.Count)];
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
                    branchCell.direction = Direction.Left;
                }
                else
                {
                    specialCell.direction = Direction.Up;
                    branchCell.direction = Direction.Down;
                }

                break;
            }
        }

        return true;
    }

    void CreateMap(MapCell[,] genMap)
    {
        map = genMap;
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                map[i, j].visited = false;
            }
        }
    }

    void PrintMap(bool printOnlyVisited = false)
    {
        PrintMap(map, printOnlyVisited);
    }

    void PrintMap(MapCell[,] mapToPrint, bool printOnlyVisited = false)
    {
        Debug.Log("Map length: " + mapToPrint.Length);
        Debug.Log("Printing generated map:");
        string mapString = "\n";
        for (int i = 0; i < mapSize.y; i++)
        {
            string line = "";
            for (int j = 0; j < mapSize.x; j++)
            {
                if (!printOnlyVisited || mapToPrint[j, i].visited)
                {
                    if (mapToPrint[j, i].type == RoomType.None)
                    {
                        line += "-";
                    }
                    else if (mapToPrint[j, i].type == RoomType.Normal)
                    {
                        line += "*";
                    }
                    else if (mapToPrint[j, i].type == RoomType.Start)
                    {
                        line += "P";
                    }
                    else if (mapToPrint[j, i].type == RoomType.Special)
                    {
                        line += "S";
                    }
                    else if (mapToPrint[j, i].type == RoomType.Boss)
                    {
                        line += "B";
                    }
                    else
                    {
                        line += "E";
                    }
                }
                else
                {
                    line += " ";
                }
            }
            mapString  = line + "\n" + mapString;
        }
        System.IO.File.WriteAllText("map.txt", mapString);

        Debug.Log("Saved to file map.txt");
    }
}
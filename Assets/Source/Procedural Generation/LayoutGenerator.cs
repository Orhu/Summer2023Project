using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [Tooltip("The parameters that affect the layout generation")]
    public LayoutGenerationParameters parameters;

    // The map of the rooms
    [HideInInspector]
    public MapCell[,] map;

    /// <summary>
    /// Generates the layout
    /// </summary>
    void Generate()
    {
        MapCell[,] genMap = InitializeGenMap();

        // Generate start room
        Vector2Int startPos = new Vector2Int(parameters.mapSize.x / 2, parameters.mapSize.y / 2);
        MapCell startCell = genMap[startPos.x, startPos.y];
        startCell.direction = Direction.All;
        Dictionary<Direction, int> directionDict = new Dictionary<Direction, int>();
        directionDict.Add(Direction.Right, GetRandomDoorPos(false));
        directionDict.Add(Direction.Up, GetRandomDoorPos(true));
        directionDict.Add(Direction.Left, GetRandomDoorPos(false));
        directionDict.Add(Direction.Down, GetRandomDoorPos(true));
        startCell.directionToDoorPos = directionDict;
        CreateRoom(startCell, parameters.startRoom);

        // Get the neighbors around the start room
        Queue<MapCell> cellsToGenerate = new Queue<MapCell>();
        List<MapCell> initialNeighbors = GetUnvisitedNeighbors(startPos, genMap);
        foreach (MapCell mapCell in initialNeighbors)
        {
            cellsToGenerate.Enqueue(mapCell);
        }

        // Generate the layout, keeping track of the dead ends
        List<MapCell> deadEnds = new List<MapCell>();
        while (cellsToGenerate.Count != 0)
        {
            MapCell currentCell = cellsToGenerate.Peek();

            // Get the door positions
            currentCell.directionToDoorPos = CreateRandomDirectionToDoorPosDict(GetDirectionConstraint(currentCell.location, genMap));
            currentCell.direction = DirectionToDoorDictToDirection(currentCell.directionToDoorPos);

            // Check if it's a dead end
            if (IsDeadEnd(currentCell.direction))
            {
                deadEnds.Add(currentCell);
            }

            // Instatiate the room
            CreateRoom(currentCell, parameters.normalRoom);
            currentCell.generated = true;

            // Remove the current cell from the cells to generate
            cellsToGenerate.Dequeue();

            // Add all the neighbors to the cells to generate
            List<MapCell> newNeighbors = GetUnvisitedNeighbors(currentCell.location, currentCell.direction, genMap);
            for (int i = 0; i < newNeighbors.Count; i++)
            {
                newNeighbors[i].added = true;
                cellsToGenerate.Enqueue(newNeighbors[i]);
            }
        }

        // Decide which dead ends will be the special rooms
        while (parameters.specialRooms.Count != 0 && deadEnds.Count > 0)
        {
            MapCell specialRoom = deadEnds[Random.Range(0, deadEnds.Count)];
            Destroy(specialRoom.room.gameObject);
            specialRoom.room = CreateRoom(specialRoom, parameters.specialRooms[0]);
            deadEnds.Remove(specialRoom);
            parameters.specialRooms.RemoveAt(0);
        }

        // What do we do if we run out of dead ends?
    }

    /// <summary>
    /// Initializes the generation map, to be used while generating
    /// </summary>
    /// <returns> The generation map </returns>
    MapCell[,] InitializeGenMap()
    {
        MapCell[,] returnVal = new MapCell[parameters.mapSize.x, parameters.mapSize.y];
        for (int i = 0; i < parameters.mapSize.x; i++)
        {
            for (int j = 0; j < parameters.mapSize.y; j++)
            {
                returnVal[i, j].location = new Vector2Int(i, j);
            }
        }
        return returnVal;
    }

    /// <summary>
    /// Transforms the mapLoc into the world frame
    /// </summary>
    /// <param name="mapLoc"> The map location </param>
    /// <returns> The world location </returns>
    Vector2 MapLocToWorldLoc(Vector2Int mapLoc)
    {
        return new Vector2(mapLoc.x - (parameters.mapSize.x / 2) * parameters.roomSize.x, mapLoc.y - (parameters.mapSize.y / 2) * parameters.roomSize.y);
    }

    /// <summary>
    /// Gets all the unvisited neighbors next to the mapLoc
    /// </summary>
    /// <param name="mapLoc"> The location to get neighbors around </param>
    /// <param name="genMap"> The current generated map </param>
    /// <returns> The unvisted neighbors </returns>
    List<MapCell> GetUnvisitedNeighbors(Vector2Int mapLoc, MapCell[,] genMap)
    {
        return GetUnvisitedNeighbors(mapLoc, Direction.All, genMap);
    }

    /// <summary>
    /// Gets the unvisited neighbors next to the mapLoc in the directions given
    /// </summary>
    /// <param name="mapLoc"> The location to get neighbors around</param>
    /// <param name="direction"> The directions to get the neighbors in </param>
    /// <param name="genMap"> The current generated map </param>
    /// <returns> The unvisited neighbors </returns>
    List<MapCell> GetUnvisitedNeighbors(Vector2Int mapLoc, Direction direction, MapCell[,] genMap)
    {
        List<MapCell> returnVal = new List<MapCell>();

        bool rightDirection = (direction & Direction.Right) != Direction.None;
        bool onRightEdge = mapLoc.x == parameters.mapSize.x - 1;
        if (rightDirection && !onRightEdge && genMap[mapLoc.x + 1, mapLoc.y].added == false)
        {
            returnVal.Add(genMap[mapLoc.x + 1, mapLoc.y]);
        }

        bool upDirection = (direction & Direction.Up) != Direction.None;
        bool onUpEdge = mapLoc.y == parameters.mapSize.x - 1;
        if (upDirection && !onUpEdge && genMap[mapLoc.x, mapLoc.y + 1].added == false)
        {
            returnVal.Add(genMap[mapLoc.x, mapLoc.y + 1]);
        }

        bool leftDirection = (direction & Direction.Left) != Direction.None;
        bool onLeftEdge = mapLoc.x == 0;
        if (leftDirection && !onLeftEdge && genMap[mapLoc.x - 1, mapLoc.y].added == false)
        {
            returnVal.Add(genMap[mapLoc.x - 1, mapLoc.y]);
        }

        bool downDirection = (direction & Direction.Down) != Direction.None;
        bool onDownEdge = mapLoc.y == 0;
        if (downDirection && !onDownEdge && genMap[mapLoc.x, mapLoc.y - 1].added == false)
        {
            returnVal.Add(genMap[mapLoc.x, mapLoc.y - 1]);
        }

        return returnVal;
    }

    /// <summary>
    /// Gets the directions this position must have and must not have
    /// </summary>
    /// <param name="mapLoc"> The location to get the constraints of </param>
    /// <returns> The constraint </returns>
    DirectionConstraint GetDirectionConstraint(Vector2Int mapLoc, MapCell[,] genMap)
    {
        DirectionConstraint constraint = new DirectionConstraint();

        bool onRightEdge = mapLoc.x == parameters.mapSize.x - 1;
        if (!onRightEdge && (genMap[mapLoc.x + 1, mapLoc.y].generated))
        {
            if ((genMap[mapLoc.x + 1, mapLoc.y].direction & Direction.Left) != Direction.None)
            {
                constraint.mustHave.Add(Direction.Right, genMap[mapLoc.x + 1, mapLoc.y].directionToDoorPos[Direction.Left]);
            }
            else
            {
                constraint.mustNotHave |= Direction.Right;
            }
        }

        bool onUpEdge = mapLoc.y == parameters.mapSize.y - 1;
        if (!onUpEdge && (genMap[mapLoc.x, mapLoc.y + 1].generated))
        {
            if ((genMap[mapLoc.x, mapLoc.y + 1].direction & Direction.Down) != Direction.None)
            {
                constraint.mustHave.Add(Direction.Up, genMap[mapLoc.x + 1, mapLoc.y].directionToDoorPos[Direction.Down]);
            }
            else
            {
                constraint.mustNotHave |= Direction.Up;
            }
        }

        bool onLeftEdge = mapLoc.x == 0;
        if (!onLeftEdge && (genMap[mapLoc.x - 1, mapLoc.y].generated))
        {
            if ((genMap[mapLoc.x - 1, mapLoc.y].direction & Direction.Right) != Direction.None)
            {
                constraint.mustHave.Add(Direction.Left, genMap[mapLoc.x + 1, mapLoc.y].directionToDoorPos[Direction.Right]);
            }
            else
            {
                constraint.mustNotHave |= Direction.Left;
            }
        }

        bool onDownEdge = mapLoc.y == 0;
        if (!onDownEdge && (genMap[mapLoc.x, mapLoc.y - 1].generated))
        {
            if ((genMap[mapLoc.x, mapLoc.y - 1].direction & Direction.Up) != Direction.None)
            {
                constraint.mustHave.Add(Direction.Down, genMap[mapLoc.x, mapLoc.y - 1].directionToDoorPos[Direction.Up]);
            }
            else
            {
                constraint.mustNotHave |= Direction.Down;
            }
        }

        return constraint;
    }

    /// <summary>
    /// Gets a random direction and door pos, adhering to the given constraint
    /// </summary>
    /// <param name="constraint"> The constraint to adhere to </param>
    /// <returns> The direction to door dict </returns>
    Dictionary<Direction, int> CreateRandomDirectionToDoorPosDict(DirectionConstraint constraint)
    {
        Dictionary<Direction, int> returnVal = new Dictionary<Direction, int>();
        Direction direction = (Direction) Random.Range(0, 16);

        foreach (KeyValuePair<Direction, int> directionToDoor in constraint.mustHave)
        {
            direction |= directionToDoor.Key;
            returnVal.Add(directionToDoor.Key, directionToDoor.Value);
        }

        Direction invertedMustNotHave = (Direction) 15 ^ constraint.mustNotHave;
        direction = direction & invertedMustNotHave;

        if ((direction & Direction.Right) != Direction.None && !returnVal.ContainsKey(Direction.Right))
        {
            returnVal.Add(Direction.Right, GetRandomDoorPos(false));
        }

        if ((direction & Direction.Up) != Direction.None && !returnVal.ContainsKey(Direction.Up))
        {
            returnVal.Add(Direction.Up, GetRandomDoorPos(true));
        }

        if ((direction & Direction.Left) != Direction.None && !returnVal.ContainsKey(Direction.Left))
        {
            returnVal.Add(Direction.Left, GetRandomDoorPos(false));
        }

        if ((direction & Direction.Down) != Direction.None && !returnVal.ContainsKey(Direction.Down))
        {
            returnVal.Add(Direction.Down, GetRandomDoorPos(true));
        }

        return returnVal;
    }

    /// <summary>
    /// Gets the overall direction within the dictionary
    /// </summary>
    /// <param name="dictionary"> The dictionary </param>
    /// <returns> The direction </returns>
    Direction DirectionToDoorDictToDirection(Dictionary<Direction, int> dictionary)
    {
        Direction direction = Direction.None;

        foreach (KeyValuePair<Direction, int> directionToDoor in dictionary)
        {
            direction |= directionToDoor.Key;
        }

        return direction;
    }

    /// <summary>
    /// Gets a random position for the door to appear in
    /// </summary>
    /// <param name="topOrBottom"> Whether or not this is getting a door for the top or bottom, or from the left or right </param>
    /// <returns> The random position </returns>
    int GetRandomDoorPos(bool topOrBottom)
    {
        if (topOrBottom)
        {
            return Random.Range(1, parameters.roomSize.x - 1);
        }
        else
        {
            return Random.Range(1, parameters.roomSize.y - 1);
        }
    }

    /// <summary>
    /// Gets whether or not there is only one direction in this direction
    /// </summary>
    /// <param name="direction"> The direction to check </param>
    /// <returns> Whether the direction is a dead end or not </returns>
    bool IsDeadEnd(Direction direction)
    {
        return direction == Direction.Right || direction == Direction.Up || direction == Direction.Left || direction == Direction.Down;
    }

    /// <summary>
    /// Insantiates the given room
    /// </summary>
    /// <param name="cell"> The cell to instantiate in </param>
    /// <param name="room"> The room to instantiate </param>
    /// <returns> The instantiated room </returns>
    GameObject CreateRoom(MapCell cell, GameObject room)
    {
        GameObject newRoom = Instantiate(room, MapLocToWorldLoc(cell.location), Quaternion.identity);
        newRoom.transform.parent = transform;
        newRoom.SetActive(true);
        return newRoom;
    }
}

/// <summary>
/// Stores door directions
/// </summary>
public enum Direction
{
    None = 0,
    Right = 1,
    Up = 2,
    Left = 4,
    Down = 8,
    All = 15
}

/// <summary>
/// Specifies what directions a room must have and which directions a room must not have
/// </summary>
public class DirectionConstraint
{
    public Dictionary<Direction, int> mustHave;
    public Direction mustNotHave = Direction.None;
}
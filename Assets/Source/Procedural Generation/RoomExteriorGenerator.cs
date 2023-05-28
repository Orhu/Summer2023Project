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
    /// Transforms a map location to a world location
    /// </summary>
    /// <param name="mapLocation"> The location to transform </param>
    /// <param name="startLocation"> The start location (aka midpoint of the map) </param>
    /// <param name="roomSize"> The room size </param>
    /// <returns></returns>
    public Vector2 TransformMapToWorld(Vector2Int mapLocation, Vector2Int startLocation, Vector2Int roomSize)
    {
        // +0.5 In order to align with the tilemap's grid
        return new Vector2((mapLocation.x - startLocation.x) * roomSize.x, (mapLocation.y - startLocation.y) * roomSize.y);
    }

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
        newRoom.transform.position = TransformMapToWorld(createdCell.location, startCell.location, roomSize);

        // Add the room component
        newRoom.AddComponent<Room>();
        newRoom.AddComponent<RoomInteriorGenerator>();

        // Add the box collider component
        BoxCollider2D roomBox = newRoom.AddComponent<BoxCollider2D>();
        roomBox.size = roomSize;
        roomBox.isTrigger = true;

        CreateWalls(createdCell, exteriorParameters, roomSize);

        //CreateDoors(newRoom, roomSize);

        //CreateFloor(newRoom, roomSize);
        
    }

    /// <summary>
    /// Initializes a tilemap
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
        int ObstacleLayer = LayerMask.NameToLayer("Obstacle");
        GameObject wallTilemapGameObject = InitializeTilemap(roomCell.room, ObstacleLayer, "Wall Tilemap");
        Tilemap wallTilemap = wallTilemapGameObject.GetComponent<Tilemap>();

        // Add the corners
        TileBase randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(-roomSize.x / 2, -roomSize.y / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(roomSize.x / 2, -roomSize.y / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(-roomSize.x / 2, roomSize.y / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(roomSize.x / 2, roomSize.y / 2, 0), randomCornerTile);

        // Add the top and bottom walls
        for (int i = - roomSize.x / 2 + 1; i <= roomSize.x / 2 - 1; i++)
        {
            if (i >= -1 && i <= 1 && (roomCell.direction & Direction.Down) != Direction.None)
            {
                if (i != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(i, -roomSize.y / 2, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(i, -roomSize.y / 2, 0), randomWallTile);
            }

            if (i >= -1 && i <= 1 && (roomCell.direction & Direction.Up) != Direction.None)
            {
                if (i != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(i, roomSize.y / 2, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(i, roomSize.y / 2, 0), randomWallTile);
            }
        }

        // Add the right and left walls
        for (int j = -roomSize.y / 2; j <= roomSize.y / 2; j++)
        {
            if (j >= -1 && j <= 1 && (roomCell.direction & Direction.Left) != Direction.None)
            {
                if (j != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(-roomSize.x / 2, j, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(-roomSize.x / 2, j, 0), randomWallTile);
            }

            if (j >= -1 && j <= 1 && (roomCell.direction & Direction.Right) != Direction.None)
            {
                if (j != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(roomSize.x / 2, j, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(roomSize.x / 2, j, 0), randomWallTile);
            }
        }

        wallTilemapGameObject.AddComponent<TilemapCollider2D>();
        wallTilemap.CompressBounds();
    }

    /// <summary>
    /// Creates walls, doors, and floors for a boss room (which is not normally sized)
    /// </summary>
    /// <param name="createdCell"> One of the cells that should be part of the boss room </param>
    /// <param name="exteriorParameters"> The parameters for the boss room </param>
    /// <param name="map"> The map </param>
    /// <param name="startCell"> The start cell </param>
    /// <param name="roomSize"> The size of a room </param>
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
        bossRoom.transform.position = TransformMapToWorld(centerCell.location, startCell.location, roomSize);

        int ObstacleLayer = LayerMask.NameToLayer("Obstacle");
        GameObject wallTilemapObject = InitializeTilemap(bossRoom, ObstacleLayer, "Boss Wall Tilemap");
        Tilemap wallTilemap = wallTilemapObject.GetComponent<Tilemap>();

        // Create the corners

        // Add the corners
        TileBase randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(-(3 * roomSize.x) / 2, -(3 * roomSize.y) / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int((3 * roomSize.x) / 2, -(3 * roomSize.y) / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int(-(3 * roomSize.x) / 2, (3 * roomSize.y) / 2, 0), randomCornerTile);

        randomCornerTile = exteriorParameters.wallCornerTiles[Random.Range(0, exteriorParameters.wallCornerTiles.Count)];
        wallTilemap.SetTile(new Vector3Int((3 * roomSize.x) / 2, (3 * roomSize.y) / 2, 0), randomCornerTile);

        // Add the top and bottom walls
        for (int i = -(3 *roomSize.x) / 2 + 1; i <= (3 *roomSize.x) / 2 - 1; i++)
        {
            if (i >= -1 && i <= 1 && (map.map[centerCell.location.x, centerCell.location.y - 1].direction & Direction.Down) != Direction.None)
            {
                if (i != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(i, -(3 * roomSize.y) / 2, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(i, -(3 * roomSize.y) / 2, 0), randomWallTile);
            }

            if (i >= -1 && i <= 1 && (map.map[centerCell.location.x, centerCell.location.y + 1].direction & Direction.Up) != Direction.None)
            {
                if (i != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(i, (3 *roomSize.y) / 2, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(i, (3 * roomSize.y) / 2, 0), randomWallTile);
            }
        }

        // Add the right and left walls
        for (int j = -(3 * roomSize.y) / 2; j <= (3 * roomSize.y) / 2; j++)
        {
            if (j >= -1 && j <= 1 && (map.map[centerCell.location.x - 1, centerCell.location.y].direction & Direction.Left) != Direction.None)
            {
                if (j != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int(-(3 *roomSize.x / 2), j, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int(-(3 * roomSize.x) / 2, j, 0), randomWallTile);
            }

            if (j >= -1 && j <= 1 && (map.map[centerCell.location.x + 1, centerCell.location.y].direction & Direction.Right) != Direction.None)
            {
                if (j != 0)
                {
                    TileBase randomNextToDoorTile = exteriorParameters.nextToDoorTiles[Random.Range(0, exteriorParameters.nextToDoorTiles.Count)];
                    wallTilemap.SetTile(new Vector3Int((3 * roomSize.x) / 2, j, 0), randomNextToDoorTile);
                }
            }
            else
            {
                TileBase randomWallTile = exteriorParameters.wallTiles[Random.Range(0, exteriorParameters.wallTiles.Count)];
                wallTilemap.SetTile(new Vector3Int((3* roomSize.x) / 2, j, 0), randomWallTile);
            }
        }

        wallTilemapObject.AddComponent<TilemapCollider2D>();
        wallTilemap.CompressBounds();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                map.map[centerCell.location.x + i, centerCell.location.y + j].room = bossRoom;
            }
        }
    }
}

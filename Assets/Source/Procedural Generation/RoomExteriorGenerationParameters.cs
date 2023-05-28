using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomExteriorGenerationParameters
{
    // The possible wall tiles to use (will pick randomly from these)
    public List<TileBase> wallTiles;

    // The possible wall corner tiles to use
    public List<TileBase> wallCornerTiles;

    // The possible floor tiles to use
    public List<TileBase> floorTiles;

    // The possible door tiles to use
    public List<DoorTiles> doorTiles;

    // The possible next to door tiles to use
    public List<TileBase> nextToDoorTiles;
}

[System.Serializable]
public class DoorTiles
{
    // The door opened tile to use
    public TileBase doorOpened;

    // The door closed tile
    public TileBase doorClosed;
}
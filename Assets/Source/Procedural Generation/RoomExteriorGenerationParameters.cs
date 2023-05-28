using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The tiles that make up the exterior of the rooms
/// </summary>
[System.Serializable]
public class RoomExteriorGenerationParameters
{
    [Tooltip("The possible wall tiles to use (will pick randomly from these)")]
    [SerializeField] public List<TileBase> wallTiles;

    [Tooltip("The possible wall corner tiles to use")]
    [SerializeField] public List<TileBase> wallCornerTiles;

    [Tooltip("The possible floor tiles to use")]
    [SerializeField] public List<TileBase> floorTiles;

    [Tooltip("The possible door tiles to use")]
    [SerializeField] public List<DoorTiles> doorTiles;

    [Tooltip("The possible next to door tiles to use")]
    [SerializeField] public List<TileBase> nextToDoorTiles;
}

/// <summary>
/// Holds two tiles for a door, an opened one and a closed one
/// </summary>
[System.Serializable]
public class DoorTiles
{
    [Tooltip("The door opened tile to use")]
    [SerializeField] public TileBase doorOpened;

    [Tooltip("The door closed tile")]
    [SerializeField] public TileBase doorClosed;
}
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
    [Tooltip("The possible wall sprites to use (will pick randomly from these)")]
    [SerializeField] public List<Sprite> wallSprites;

    [Tooltip("The possible wall corner sprites to use")]
    [SerializeField] public List<Sprite> wallCornerSprites;

    [Tooltip("The possible floor sprites to use")]
    [SerializeField] public List<Sprite> floorSprites;

    [Tooltip("The possible door sprites to use")]
    [SerializeField] public List<DoorSprites> doorSprites;

    [Tooltip("The possible next to door sprites to use")]
    [SerializeField] public List<Sprite> nextToDoorSprites;
}

/// <summary>
/// Holds two sprites for a door, an opened one and a closed one
/// </summary>
[System.Serializable]
public class DoorSprites
{
    [Tooltip("The door opened tile to use")]
    [SerializeField] public Sprite doorOpened;

    [Tooltip("The door closed tile")]
    [SerializeField] public Sprite doorClosed;
}
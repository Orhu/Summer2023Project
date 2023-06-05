using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The tiles that make up the exterior of the rooms
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewRoomExteriorGenerationParameters", menuName = "Generation/RoomExteriorGenerationParameters", order = 1)]

public class RoomExteriorGenerationParameters : ScriptableObject
{
    [Tooltip("The possible right wall sprites to use (will pick randomly from these)")]
    public List<Sprite> rightWallSprites;

    [Tooltip("The possible top wall sprites to use")]
    public List<Sprite> topWallSprites;

    [Tooltip("The possible left wall sprites to use")]
    public List<Sprite> leftWallSprites;

    [Tooltip("The possible bottom wall sprites to use")]
    public List<Sprite> bottomWallSprites;

    [Tooltip("The possible top right wall corner sprites to use")]
    public List<Sprite> topRightWallCornerSprites;

    [Tooltip("The possible top left wall corner sprites to use")]
    public List<Sprite> topLeftWallCornerSprites;

    [Tooltip("The possible bottom left wall corner sprites to use")]
    public List<Sprite> bottomLeftWallCornerSprites;

    [Tooltip("The possible bottom right wall corner sprites to use")]
    public List<Sprite> bottomRightWallCornerSprites;

    [Tooltip("The possible right door sprites to use")]
    public List<DoorSprites> rightDoorSprites;

    [Tooltip("The possible top door sprites to use")]
    public List<DoorSprites> topDoorSprites;

    [Tooltip("The possible left door sprites to use")]
    public List<DoorSprites> leftDoorSprites;

    [Tooltip("The possible bottom door sprites to use")]
    public List<DoorSprites> bottomDoorSprites;

    [Tooltip("The possible floor sprites to use")]
    public List<Sprite> floorSprites;
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
using UnityEngine;

/// <summary>
/// The names for each exterior param type
/// </summary>
[CreateAssetMenu(fileName = "NewExteriorParamNames", menuName = "Generation/ExteriorParamNames")]
public class ExteriorParamNames : ScriptableObject
{
    [Header("Walls")]
    [Tooltip("The name for the right wall sprites")]
    public string rightWallName;

    [Tooltip("The name for the right wall sprites")]
    public string topWallName;

    [Tooltip("The name for the right wall sprites")]
    public string leftWallName;

    [Tooltip("The name for the right wall sprites")]
    public string bottomWallName;

    [Header("Corners")]
    [Tooltip("The name for the top right corner sprites")]
    public string topRightCornerName;

    [Tooltip("The name for the top left corner sprites")]
    public string topLeftCornerName;

    [Tooltip("The name for the bottom left corner sprites")]
    public string bottomLeftCornerName;

    [Tooltip("The name for the bottom right corner sprites")]
    public string bottomRightCornerName;

    [Header("Doors")]
    [Tooltip("The name for the right door sprites")]
    public string rightDoorName;

    [Tooltip("The name for the top door sprites")]
    public string topDoorName;

    [Tooltip("The name for the left door sprites")]
    public string leftDoorName;

    [Tooltip("The name for the bottom door sprites")]
    public string bottomDoorName;

    [Header("Door Frames")]
    [Tooltip("The name for the right door top door frame sprites")]
    public string rightDoorTopDoorFrameName;

    [Tooltip("The name for the right door bottom door frame sprites")]
    public string rightDoorBottomDoorFrameName;

    [Tooltip("The name for the top door right door frame sprites")]
    public string topDoorRightDoorFrameName;

    [Tooltip("The name for the top door left door frame sprites")]
    public string topDoorLeftDoorFrameName;

    [Tooltip("The name for the left door top door frame sprites")]
    public string leftDoorTopDoorFrameName;

    [Tooltip("The name for the left door bottom door frame sprites")]
    public string leftDoorBottomDoorFrameName;

    [Tooltip("The name for the bottom door right door frame sprites")]
    public string bottomDoorRightDoorFrameName;

    [Tooltip("The name for the bottom door left door frame sprites")]
    public string bottomDoorLeftDoorFrameName;

    [Header("Floors")]
    [Tooltip("The name for the floor sprites")]
    public string floorName;
}

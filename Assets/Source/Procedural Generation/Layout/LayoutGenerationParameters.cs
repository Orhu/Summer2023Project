using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The parameters that affect the layout generation
/// </summary>
[System.Serializable]
public class LayoutGenerationParameters
{
    [Tooltip("The (approximate) number of normal rooms to generate")]
    [SerializeField] public int numNormalRooms;

    [Tooltip("The variance of randomness for the number of normal rooms to generate")]
    [SerializeField] public int numNormalRoomsVariance;

    [Tooltip("The number of special rooms that will appear")]
    [SerializeField] public int numSpecialRooms;

    [Tooltip("The number of doors that is preferred")] [Range(0, 4)]
    [SerializeField] public int preferredNumDoors;

    [Tooltip("How strictly the generation adheres to the preferred number of doors (100 = very strict, 0 = not strict at all)")] [Range(0, 100)]
    [SerializeField] public float strictnessNumDoors;
}
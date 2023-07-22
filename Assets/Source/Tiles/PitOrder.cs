using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps ints to other ints
/// </summary>
[System.Serializable] [CreateAssetMenu(fileName = "NewPitOrder", menuName = "Pits/PitOrder")]
public class PitOrder : ScriptableObject
{
    // The a list of ints that will map one int to another int (values of -1 are unused)
    public List<int> order;
}


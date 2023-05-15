using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Detector to be used by different detector types
/// </summary>
public abstract class Detector : MonoBehaviour
{
    /// <summary>
    /// Detect nearby things, depending on detector type
    /// </summary>
    /// <param name="aiData">Data type to write information to</param>
    public abstract void Detect(AIData aiData);
}
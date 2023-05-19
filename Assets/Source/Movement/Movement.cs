using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all components that move game objects
/// </summary>
public abstract class Movement : MonoBehaviour
{
    // The desired movement direction.
    public virtual Vector2 MovementInput { get; set; }

    // A delegate that is queried before moving to adjust the speed based on outside factors.
    public delegate void ModifySpeed(ref float speed);
    public ModifySpeed requestSpeedModifications;
}

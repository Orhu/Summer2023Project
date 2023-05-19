using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    // The desired movement direction.
    public abstract Vector2 MovementInput { get; set; }

    public delegate void ModifySpeed(ref float speed);
    public ModifySpeed requestSpeedModifications;
}

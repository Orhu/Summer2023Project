using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all components that move game objects
/// </summary>
[RequireComponent(typeof(AnimatorController))]
public abstract class Movement : MonoBehaviour
{
    // The desired movement direction.
    private Vector2 _movementInput;
    public virtual Vector2 movementInput 
    {
        get => _movementInput;
        set
        {
            _movementInput = value;
            animatorComponent.SetBool("moving", value.sqrMagnitude > 0);
            if (value.x == 0) { return; }
            animatorComponent.SetMirror("runLeft", value.x < 0);
        }
    }

    // A delegate that is queried before moving to adjust the speed based on outside factors.
    public delegate void ModifySpeed(ref float speed);
    public ModifySpeed requestSpeedModifications;

    // animator component to make the pretty animations do their thing
    private AnimatorController animatorComponent;

    /// <summary>
    /// Initialize Reference
    /// </summary>
    protected void Awake()
    {
        animatorComponent = GetComponent<AnimatorController>();
    }
}

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
    private Vector2 _movementInput;
    public virtual Vector2 movementInput 
    {
        get => _movementInput;
        set
        {
            _movementInput = value;
            animatorComponent.SetBool("moving", value.sqrMagnitude > 0);
            transform.localScale = new Vector3(1, 1, value.x < 0 ? -1 : 1);
        }
    }

    // A delegate that is queried before moving to adjust the speed based on outside factors.
    public delegate void ModifySpeed(ref float speed);
    public ModifySpeed requestSpeedModifications;

    // animator component to make the pretty animations do their thing
    private Animator animatorComponent;

    protected void Awake()
    {
        animatorComponent = GetComponent<Animator>();
    }
}

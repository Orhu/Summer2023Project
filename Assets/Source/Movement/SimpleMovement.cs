using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A movement component that moves towards the move input, and has acceleration and deceleration.
/// </summary>
public class SimpleMovement : Movement
{
    // TODO: add a [RequireComponent] attribute
    // The rigid body that handles collisions 
    private Rigidbody2D rb2d;

    [SerializeField]
    [Tooltip("The max speed this can accelerate to")]
    private float maxSpeed = 2;
    [SerializeField]
    [Tooltip("The speed at which this accelerates to the desired move direction")]
    private float acceleration = 50;
    [SerializeField]
    [Tooltip("The speed at which this accelerates to zero velocity")]
    private float deacceleration = 100;
    
    // The current speed that this is moving at.
    private float currentSpeed = 0;
    // The last movement input.
    private Vector2 oldMovementInput;

    /// <summary>
    /// Initializes the rigid body reference.
    /// </summary>
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Updates velocity.
    /// </summary>
    private void FixedUpdate()
    {
        if (MovementInput.magnitude > 0 && currentSpeed >= 0)
        {
            oldMovementInput = MovementInput;
            currentSpeed += acceleration * maxSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deacceleration * maxSpeed * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        requestSpeedModifications?.Invoke(ref currentSpeed);
        rb2d.velocity = oldMovementInput * currentSpeed;

    }


}
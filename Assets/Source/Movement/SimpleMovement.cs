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
    [Tooltip("The max speed in tiles/s this can accelerate to")]
    private float maxSpeed = 2;
    [SerializeField]
    [Tooltip("The speed in maxSpeed/s at which this accelerates to the desired move direction")]
    private float acceleration = 50;
    [SerializeField]
    [Tooltip("The speed in maxSpeed/s at which this accelerates to zero velocity")]
    private float deacceleration = 100;

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
        float targetSpeed = maxSpeed;
        requestSpeedModifications?.Invoke(ref targetSpeed);
        Vector2 targetVelocity = MovementInput * targetSpeed;

        Vector2 deltaVelocity = targetVelocity - rb2d.velocity;

        float currentAcceleration = Time.deltaTime * maxSpeed;
        currentAcceleration *= MovementInput.sqrMagnitude == 0 ? deacceleration : acceleration;

        rb2d.velocity += Vector2.ClampMagnitude(deltaVelocity.normalized * currentAcceleration, deltaVelocity.magnitude);
    }


}
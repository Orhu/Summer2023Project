using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A movement component that moves towards the move input, and has acceleration and deceleration.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class SimpleMovement : Movement
{
    // The rigid body that handles collisions 
    private Rigidbody2D rb2d;
    
    [Tooltip("The max speed in tiles/s this can accelerate to")]
    public float maxSpeed = 2;
    
    [Tooltip("The speed in maxSpeed/s at which this accelerates to the desired move direction")]
    public float acceleration = 50;
   
    [Tooltip("The speed in maxSpeed/s at which this accelerates to zero velocity")]
    public float deceleration = 100;
    
    [Tooltip("Is this unit immune to movement-altering grounds (eg ice)?")]
    public bool immuneToGroundEffects = false;
    
    // cache original values in case they are needed
    public float originalMaxSpeed { get; private set; }
    public float originalAcceleration { get; private set; }
    public float originalDeceleration { get; private set; }

    /// <summary>
    /// Initializes the rigid body reference.
    /// </summary>
    private new void Awake()
    {
        base.Awake();
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Caches original values
    /// </summary>
    void Start()
    {
        originalMaxSpeed = maxSpeed;
        originalAcceleration = acceleration;
        originalDeceleration = deceleration;
    }

    /// <summary>
    /// Updates velocity.
    /// </summary>
    private void FixedUpdate()
    {
        float targetSpeed = maxSpeed;
        requestSpeedModifications?.Invoke(ref targetSpeed);
        Vector2 targetVelocity = movementInput * targetSpeed;

        Vector2 deltaVelocity = targetVelocity - rb2d.velocity;

        float currentAcceleration = Time.deltaTime * maxSpeed;
        currentAcceleration *= movementInput.sqrMagnitude == 0 ? deceleration : acceleration;

        rb2d.velocity += Vector2.ClampMagnitude(deltaVelocity.normalized * currentAcceleration, deltaVelocity.magnitude);
    }


}
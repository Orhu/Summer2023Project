using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component supports any trigger that is placed on the ground that affects movement (eg ice paths)
/// </summary>
public class ChangeMovementGround : MonoBehaviour
{
    [Tooltip("What does this ice path set everyone's max speed to? Set to -1 to keep their same speed.")]
    [SerializeField] private float newMaxSpeed;
    
    [Tooltip("What does this ice path set everyone's acceleration to? Set to -1 to keep their same acceleration.")]
    [SerializeField] private float newAcceleration;
    
    [Tooltip("What does this ice path set everyone's deceleration to? Set to -1 to keep their same deceleration.")]
    [SerializeField] private float newDeceleration;

    /// <summary>
    /// If the collider has a simple movement component, set the correct speed information
    /// </summary>
    /// <param name="other"> The collider that enters the collision </param>
    void OnTriggerStay2D(Collider2D other)
    {
        // get component in parent because feet will be the collider we get here
            var simpleMovementComponent = other.GetComponentInParent<SimpleMovement>();
            if (simpleMovementComponent != null && !simpleMovementComponent.immuneToGroundEffects)
            {
                simpleMovementComponent.maxSpeed = newMaxSpeed != -1 ? newMaxSpeed : simpleMovementComponent.maxSpeed;
                simpleMovementComponent.acceleration = newAcceleration != -1 ? newAcceleration : simpleMovementComponent.acceleration;
                simpleMovementComponent.deceleration = newDeceleration != -1 ? newDeceleration : simpleMovementComponent.deceleration;
            }
    }

    /// <summary>
    /// If the collider has a simple movement component, reset the speed information to its original values
    /// </summary>
    /// <param name="other"> The collider that enters the collision </param>
    void OnTriggerExit2D(Collider2D other)
    {
        // get component in parent because feet will be the collider we get here
        var simpleMovementComponent = other.GetComponentInParent<SimpleMovement>();
        if (simpleMovementComponent != null && !simpleMovementComponent.immuneToGroundEffects)
        {
            simpleMovementComponent.maxSpeed = simpleMovementComponent.originalMaxSpeed;
            simpleMovementComponent.acceleration = simpleMovementComponent.originalAcceleration;
            simpleMovementComponent.deceleration = simpleMovementComponent.originalDeceleration;
        }
    }
}

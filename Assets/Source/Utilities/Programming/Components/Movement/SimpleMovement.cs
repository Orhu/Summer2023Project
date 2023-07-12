using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A movement component that moves towards the move input, and has acceleration and deceleration.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class SimpleMovement : Movement
    {
        // The rigid body that handles collisions 
        private Rigidbody2D rigidBody;

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
            rigidBody = GetComponent<Rigidbody2D>();
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
            if (ActiveKnockbacks.Count == 0)
            {
                float targetSpeed = maxSpeed;
                requestSpeedModifications?.Invoke(ref targetSpeed);
                Vector2 targetVelocity = movementInput * targetSpeed;

                Vector2 deltaVelocity = targetVelocity - rigidBody.velocity;

                float currentAcceleration = Time.deltaTime * maxSpeed;
                currentAcceleration *= movementInput.sqrMagnitude == 0 ? deceleration : acceleration;

                rigidBody.velocity += Vector2.ClampMagnitude(deltaVelocity.normalized * currentAcceleration, deltaVelocity.magnitude);
            }
            else
            {
                Vector2 knockbackVelocity = Vector2.zero;
                for (int i = 0; i < ActiveKnockbacks.Count; i++)
                {
                    knockbackVelocity += ActiveKnockbacks[i].direction;
                    if ((ActiveKnockbacks[i].duration -= Time.fixedDeltaTime) > 0) { continue; }

                    ActiveKnockbacks.RemoveAt(i--);
                }

                rigidBody.MovePosition((Vector2)transform.position + knockbackVelocity * Time.fixedDeltaTime);
            }
        }

        #region Knockback
        // The knockback effects currently being applied.
        private List<ActiveKnockback> ActiveKnockbacks = new List<ActiveKnockback>();
        private class ActiveKnockback
        {
            // The direction of the knockback.
            public Vector2 direction;

            // The remaing duration of this knockback.
            public float duration;

            public ActiveKnockback(Vector2 direction, float duration)
            {
                this.direction = direction;
                this.duration = duration;
            }
        }

        /// <summary>
        /// Causes knockback to this moment component.
        /// </summary>
        /// <param name="direction"> The direction to apply the knockback in. Should be normalized. </param>
        /// <param name="info"> The knockback info. </param>
        public override void Knockback(Vector2 direction, KnockbackInfo info)
        {
            if (info.resetMomentum)
            {
                rigidBody.velocity = Vector2.zero;
            }

            if (info.amount == 0) { return; }
            float duration = Mathf.Max(Time.fixedDeltaTime, info.duration);
            ActiveKnockbacks.Add(new ActiveKnockback(direction * knockbackMultiplier * info.amount / duration, duration));
        }
        #endregion
    }
}
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A projectile that orbits in a circle around the spawn location.
    /// </summary>
    public class OrbitProjectile : Projectile
    {
        // The attack that spawned this.
        private OrbitAttack orbitAttack;

        // The spawn info used to spawn this.
        private OrbitSpawnInfo orbitSpawnInfo;

        // The current orbit radius.
        public float radius;

        // The time this has existed for.
        private float timeAlive;

        // The velocity of the center of the orbit
        private Vector2 centerPosition;

        /// <summary>
        /// Handles initial position and rotation.
        /// </summary>
        new void Start()
        {
            orbitAttack = attack as OrbitAttack;
            orbitSpawnInfo = spawnSequence[index] as OrbitSpawnInfo;
            radius = orbitSpawnInfo.radius + Random.Range(orbitAttack.randomRadius / -2f, orbitAttack.randomRadius / 2f);

            // Position
            transform.position = GetSpawnLocation();
            if (!orbitAttack.attachedToSpawnLocation)
            {
                centerPosition = transform.position;
            }

            Vector3 aimDirection = (GetAimTarget(attack.aimMode) - actor.GetActionSourceTransform().position).normalized;
            float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            float startingRotation = orbitSpawnInfo.startingAngle + Random.Range(orbitAttack.randomStartingAngle / -2f, orbitAttack.randomStartingAngle / 2f);

            transform.position += Quaternion.AngleAxis(startingRotation + aimRotation, Vector3.forward) * Vector2.right * radius;
            transform.rotation = Quaternion.AngleAxis(startingRotation + aimRotation + 90 * OrbitSign(), Vector3.forward);

            base.Start();

            if (orbitSpawnInfo.orbitDirection == OrbitSpawnInfo.RotationDirection.Counterclockwise)
            {
                transform.GetChild(0).transform.localScale = Vector3.Scale(transform.GetChild(0).transform.localScale, new Vector3(1, -1, 1));
            }
        }

        /// <summary>
        /// Updates orbit rotation if not homing.
        /// </summary>
        new void FixedUpdate()
        {
            timeAlive += Time.fixedDeltaTime;
            speed += acceleration * Time.fixedDeltaTime;

            transform.rotation = Quaternion.AngleAxis(OrbitSign() * Mathf.Rad2Deg * speed * timeAlive / radius, Vector3.forward);
            

            if (remainingHomingDelay <= 0 && remainingHomingTime > 0 && homingSpeed > 0)
            {
                Vector2 targetVelocity = (GetAimTarget(attack.homingAimMode) - transform.position).normalized * maxSpeed;
                if (targetVelocity.sqrMagnitude > Vector2.kEpsilon)
                {
                    velocity += (targetVelocity - velocity).normalized * homingSpeed * Time.fixedDeltaTime;
                    velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, minSpeed, maxSpeed); 
                }
            }

            centerPosition += velocity * Time.fixedDeltaTime;
            Vector2 offset = centerPosition + (Vector2)transform.up * radius;
            if (orbitAttack.attachedToSpawnLocation)
            {
                offset += (Vector2)GetSpawnLocation();
            }
            rigidBody.MovePosition(offset);
        }

        /// <summary>
        /// Gets the sign of the tangent rotation of the orbit direction.
        /// </summary>
        /// <returns> -1 if clockwise, 1 if counterclockwise. </returns>
        float OrbitSign()
        {
            return (orbitSpawnInfo.orbitDirection == OrbitSpawnInfo.RotationDirection.Clockwise ? -1f : 1f);
        }
    }
}

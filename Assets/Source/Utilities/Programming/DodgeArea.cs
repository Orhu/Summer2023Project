using System.Collections;
using System.Collections.Generic;
using Cardificer;
using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{

    /// <summary>
    /// Represents a dodge area around an enemy that flags the state machine to dodge when an enemy projectile enters the radius
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DodgeArea : MonoBehaviour
    {
        [Tooltip("Tolerance for angle comparison in degrees")]
        [SerializeField] private float tolerance = 30;
        
        // Stores a reference to our state machine
        private BaseStateMachine stateMachine;
        
        /// <summary>
        /// Assigns a reference to the state machine
        /// </summary>
        void Start()
        {
            stateMachine = GetComponentInParent<BaseStateMachine>();
        }

        /// <summary>
        /// Determines if an enemy projectile enters the dodge area, and if it does and it's coming towards the enemy, then flag the state machine to dodge
        /// </summary>
        /// <param name="collider2D"> The collider of the projectile </param>
        void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (!stateMachine.canDodge)
            {
                return; 
            }
            
            var projectile = collider2D.GetComponent<BulletProjectile>();

            if (projectile != null)
            {
                var onTeam = projectile.ignoredObjects.Contains(gameObject);
                if (!onTeam)
                {
                    // Check the angle and movement vector of the collided projectile.
                    Vector2 prjDirection = stateMachine.transform.position - collider2D.transform.position;
                    Vector2 prjForward = collider2D.GetComponent<Rigidbody2D>().velocity;
                    float angleToPrj = Vector2.Angle(prjForward, prjDirection);

                    // Check if the angle is within the tolerance.
                    if (angleToPrj <= tolerance)
                    {
                        stateMachine.needToDodge = true;
                    }
                }
            }
        }
    }
}
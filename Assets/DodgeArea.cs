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
    public class DodgeArea : MonoBehaviour
    {
        [Tooltip("Tolerance for angle comparison in degrees")]
        [SerializeField] private float tolerance = 15;
        
        // Stores a reference to our state machine
        private BaseStateMachine stateMachine;
        
        void Start()
        {
            stateMachine = GetComponentInParent<BaseStateMachine>();
        }

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
                    Vector2 prjDirection = collider2D.GetComponent<Rigidbody2D>().velocity.normalized;
                    Vector2 angleToPrj = (collider2D.transform.position - transform.position).normalized;

                    // Calculate the angle between the projectile's movement vector and the vector between the projectile and this GameObject.
                    float angleBetweenVectors = Vector2.Angle(prjDirection, angleToPrj);

                    // Check if the angle is within the tolerance.
                    if (Mathf.Abs(angleBetweenVectors) <= tolerance)
                    {
                        stateMachine.needToDodge = true;
                    }
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Cardificer;
using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a behavior that places an arrow in the direction the enemy is facing.
    /// </summary>
    [RequireComponent(typeof(BaseStateMachine))]
    public class DirectionalArrow : MonoBehaviour
    {
        [Tooltip("The sprite to use for the arrow")]
        [SerializeField] private Sprite arrowSprite;

        // Store a reference to the BaseStateMachine
        private BaseStateMachine stateMachine;

        // Store a reference to the arrow GameObject
        private GameObject arrowObject;
        
        // Store a reference to the arrow's SpriteRenderer
        private SpriteRenderer arrowSpriteRenderer;

        /// <summary>
        /// Initializes components and the arrow GameObject
        /// </summary>
        private void Start()
        {
            // Get the BaseStateMachine component attached to this game object
            stateMachine = GetComponent<BaseStateMachine>();

            // Create the arrow game object and set its initial position and parent
            arrowObject = new GameObject();
            arrowObject.transform.position = stateMachine.transform.position;
            arrowObject.transform.parent = stateMachine.transform;

            // Add a SpriteRenderer component and set the arrow sprite
            arrowSpriteRenderer = arrowObject.AddComponent<SpriteRenderer>();
            arrowSpriteRenderer.sprite = arrowSprite;

            // Hide the arrow at the beginning
            arrowObject.SetActive(false);
        }

        /// <summary>
        /// Calculates the necessary angle to face in the forward movement direction and points the arrow sprite in that direction
        /// </summary>
        private void FixedUpdate()
        {
            // Get the movement input from the Movement component and normalize it
            // We don't cache movement component because the state machine caches it for us
            Vector2 moveInput = stateMachine.GetComponent<Movement>().movementInput.normalized;

            if (moveInput.x != 0 || moveInput.y != 0)
            {
                // If there is movement input, show the arrow and update its position and rotation
                arrowObject.SetActive(true);
                arrowObject.transform.position = (Vector2)stateMachine.transform.position + moveInput;

                // Calculate the direction and angle of the arrow based on the movement input
                var direction = arrowObject.transform.position - stateMachine.transform.position;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Need to flip because external scripts flip the sprite when moving in a different direction
                // Assumes the arrow sprite is facing right
                arrowSpriteRenderer.flipX = moveInput.x > 0;

                arrowObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                // If there is no movement input, hide the arrow
                arrowObject.SetActive(false);
            }
        }
    }
}
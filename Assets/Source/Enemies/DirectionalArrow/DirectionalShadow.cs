using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Represents a behavior that places an arrow in the direction the enemy is facing.
    /// </summary>
    [RequireComponent(typeof(BaseStateMachine))]
    public class DirectionalShadow: MonoBehaviour
    {
        [Tooltip("Reference to the prefab used for shadow arrow")]
        [SerializeField] private GameObject shadowObject;

        // Store a reference to the BaseStateMachine
        private BaseStateMachine stateMachine;
        
        // Store a reference to the arrow's animator
        private Animator anim;

        private enum ArrowDirection { None, Left, Right, Up, Down } //None == no arrow, but facing right
        //corresponds to "direction" in the animator.
        // None: 0
        // Left: 1
        // Right: 2
        // Up: 3
        // Down: 4

        private ArrowDirection currentDirection = ArrowDirection.None;

        /// <summary>
        /// Initializes components and the arrow GameObject
        /// </summary>
        private void Start()
        {
            // Get the BaseStateMachine component attached to this game object
            stateMachine = GetComponent<BaseStateMachine>();
         
            anim = shadowObject.GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// Calculates the necessary angle to face in the forward movement direction and points the arrow sprite in that direction
        /// </summary>
        private void FixedUpdate()
        {
            // Get the movement input from the Movement component and normalize it
            // We don't cache movement component because the state machine caches it for us
            Vector2 moveInput = stateMachine.GetComponent<Movement>().movementInput.normalized;

            ArrowDirection targetDirection = GetTargetDirection(moveInput);

            //only update visuals if the direction changed
            if (currentDirection != targetDirection) VisualizeTargetDirection(targetDirection);
        }

        //changes shadow to match direction of movement
        private void VisualizeTargetDirection(ArrowDirection targetDir)
        {
            if (targetDir == ArrowDirection.None)
            {
                //Debug.Log("STOPPED case");

                //use the default animation & ensure our x values are not inverted
                anim.SetInteger("direction", 0);
                currentDirection = ArrowDirection.None;
            }

           
            if (targetDir == ArrowDirection.Left)
            {
                //Debug.Log("LEFT case");

                anim.SetInteger("direction", 1);
                currentDirection = ArrowDirection.Left;
                return;
            }

            if (targetDir == ArrowDirection.Right)
            {
                //Debug.Log("RIGHT case");

                anim.SetInteger("direction", 2);
                currentDirection = ArrowDirection.Right;
                return;
            }

            if (targetDir == ArrowDirection.Up)
            {
                //Debug.Log("UP case");

                anim.SetInteger("direction", 3);
                currentDirection = ArrowDirection.Up;
                return;
            }

            if (targetDir == ArrowDirection.Down)
            {
                anim.SetInteger("direction", 4);
                currentDirection = ArrowDirection.Down;
                return;
            }
        }

        //Returns intended direction based on movement input
        private ArrowDirection GetTargetDirection(Vector2 moveInput)
        {
            //ensure the target direction is what direction we are moving in the MOST
            if (moveInput.x == 0f && moveInput.y == 0f) return ArrowDirection.None;

            if (moveInput.x < 0 && Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y)) return ArrowDirection.Left;
            if (moveInput.x > 0 && Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y)) return ArrowDirection.Right;
            if (moveInput.y > 0) return ArrowDirection.Up;
            return ArrowDirection.Down;
        }
    }
}
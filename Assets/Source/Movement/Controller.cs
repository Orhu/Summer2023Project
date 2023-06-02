using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
public class Controller : MonoBehaviour, IActor
{
    [Tooltip("is this agent controllable by inputs?")] 
    [SerializeField] private bool isControllable;

    [Tooltip("Does this agent use enemy brain component?")] 
    [SerializeField] private bool useEnemyLogic;

    // -1 to 1 range representing current movement input, same system as built-in Input.GetAxis"
    [HideInInspector] public Vector2 movementInput;

    // Movement component to allow the agent to move
    private Movement movementComponent;

    // enemy attacker component, if it exists on this agent
    private EnemyAttacker enemyAttacker;
    
    // enemy brain component, if it exists on this agent
    private EnemyBrain enemyBrain;
    
    // represents the inner collider of this unit
    [HideInInspector] public Collider2D feet;
    
    // can this enemy move?
    private bool canMove = true;

    /// <summary>
    /// Initialize components
    /// </summary>
    private void Awake()
    {
        if (useEnemyLogic)
        {
            enemyAttacker = GetComponent<EnemyAttacker>();
            enemyBrain = GetComponent<EnemyBrain>();
        }

        movementComponent = GetComponent<Movement>();
        feet = GetComponentInChildren<CircleCollider2D>();
    }

    /// <summary>
    /// Retrieve inputs where necessary and perform actions as needed
    /// </summary>
    private void Update()
    {
        // if we are controllable, get inputs. otherwise, don't
        if (isControllable)
        {
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
        }

        if (!useEnemyLogic && CanAct)
        {
            int pressedPreview = GetPressedPreviewButton();
            if (pressedPreview > 0)
            {
                Deck.playerDeck.SelectCard(pressedPreview - 1);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Deck.playerDeck.PlayChord();
            }
        }

        movementComponent.MovementInput = movementInput.normalized;
    }

    /// <summary>
    /// Launch an action from this agent
    /// </summary>
    public void PerformAttack()
    {
        var coroutine = enemyAttacker.PerformAttack(this);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// Gets the card preview button being pressed.
    /// </summary>
    /// <returns> The number corresponding to the current button, -1 if none pressed. </returns>
    static int GetPressedPreviewButton()
    {
        for (int i = 1; i <= Deck.playerDeck.handSize; i++)
        {
            if (Input.GetButtonDown("PreviewCard" + i))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Issues a command to move towards the given Vector2. Essentially, converts a Vector2 targetPos into an input vector and sets that as our input
    /// </summary>
    /// <param name="target"> Target to move to </param>
    public void MoveTowards(Vector2 target)
    {
        if (!canMove) return;
        var buffer = 0.1f;
        var myPos = (Vector2)transform.position;
        var targetPos = target;

        var xDiff = myPos.x - targetPos.x;
        var yDiff = myPos.y - targetPos.y;

        // compare the two positions to determine inputs
        if (xDiff > buffer)
        {
            movementInput.x = -1;
        } else if (xDiff < -buffer)
        {
            movementInput.x = 1;
        }
        else
        {
            movementInput.x = 0;
        }
        
        if (yDiff > buffer)
        {
            movementInput.y = -1;
        } else if (yDiff < -buffer)
        {
            movementInput.y = 1;
        }
        else
        {
            movementInput.y = 0;
        }

    }

    #region IActor Implementation

    // Gets whether or not this actor can act.
    IActor.CanActRequest canAct;

    bool CanAct
    {
        get
        {
            bool shouldAct = true;
            canAct?.Invoke(ref shouldAct);
            return shouldAct;
        }
    }

    /// <summary>
    /// Get the transform that the action should be played from.
    /// </summary>
    /// <returns> The actor transform. </returns>
    public Transform GetActionSourceTransform()
    {
        return transform;
    }


    /// <summary>
    /// Get the position that the action should be aimed at.
    /// </summary>
    /// <returns> The mouse position in world space. </returns>
    public Vector3 GetActionAimPosition()
    {
        if (useEnemyLogic)
        {
            var target = enemyBrain.target;
            if (target != null)
            {
                return target.transform.position;
            }
            else
            {
                // TODO probably think of a better fail case than just returning the zero vector
                return Vector2.zero;
            }
        }

        return Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0));
    }


    /// <summary>
    /// Gets the collider of this actor.
    /// </summary>
    /// <returns> The collider. </returns>
    public Collider2D GetCollider()
    {
        return GetComponent<Collider2D>();
    }

    public ref IActor.CanActRequest GetOnRequestCanAct()
    {
        return ref canAct;
    }

    #endregion
}
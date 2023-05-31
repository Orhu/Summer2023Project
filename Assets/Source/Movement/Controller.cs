using System;
using System.Collections.Generic;
using UnityEngine;
using Action = CardSystem.Action;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
public class Controller : MonoBehaviour, IActor
{
    [Tooltip("is this agent controllable by inputs?")] 
    [SerializeField] private bool isControllable;

    [Tooltip("Does this agent use enemy brain components?")] 
    [SerializeField] private bool useEnemyLogic;

    // Movement component to allow the agent to move
    [HideInInspector] private Movement movementComponent;

    // -1 to 1 range representing current movement input, same system as built-in Input.GetAxis"
    [HideInInspector] private Vector2 _movementInput;
    public Vector2 MovementInput
    {
        get => _movementInput;
        set => _movementInput = value;
    }
    
    // enemy attacker component, if it exists on this agent
    private EnemyAttacker enemyAttacker;
    
    // enemy brain component, if it exists on this agent
    private EnemyBrain enemyBrain;

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
    }

    /// <summary>
    /// Retrieve inputs where necessary and perform actions as needed
    /// </summary>
    private void Update()
    {
        // if we are controllable, get inputs. otherwise, don't
        if (isControllable)
        {
            _movementInput.x = Input.GetAxisRaw("Horizontal");
            _movementInput.y = Input.GetAxisRaw("Vertical");
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

        movementComponent.MovementInput = _movementInput.normalized;
    }

    /// <summary>
    /// Launch an action from this agent
    /// </summary>
    public void PerformAttack()
    {
        enemyAttacker.PerformAttack(this);
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
    /// <param name="buffer"> How close to get before I am happy with my position </param>
    public void MoveTowards(Vector2 target, float buffer)
    {
        _movementInput = Vector2.zero;
        
        var myPos = (Vector2)transform.position;
        var targetPos = target;

        var xDiff = myPos.x - targetPos.x;
        var yDiff = myPos.y - targetPos.y;

        // compare the two positions to determine inputs
        if (xDiff > buffer)
        {
            _movementInput.x = -1;
        } else if (xDiff < -buffer)
        {
            _movementInput.x = 1;
        }
        else
        {
            _movementInput.x = 0;
        }
        
        if (yDiff > buffer)
        {
            _movementInput.y = -1;
        } else if (yDiff < -buffer)
        {
            _movementInput.y = 1;
        }
        else
        {
            _movementInput.y = 0;
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
            var targetPos = enemyBrain.GetTargetPosition();
            if (targetPos != null)
            {
                return targetPos.transform.position;
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
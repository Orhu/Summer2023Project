using System;
using UnityEngine;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
public class Controller : MonoBehaviour, IActor
{
    [Tooltip("is this agent controllable by inputs?")] 
    [SerializeField] private bool isControllable;

    [Tooltip("is this agent capable of selecting/using cards?")] 
    [SerializeField] private bool canPlayCards;

    [Tooltip("Does this agent use enemy logic?")] 
    [SerializeField] private bool useEnemyLogic;
    
    
    // "Movement component to allow the agent to move"
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
    
    // enemy AI component, if it exists on this agent
    private EnemyAI enemyAI;

    private void Start()
    {
        if (useEnemyLogic)
        {
            enemyAttacker = GetComponent<EnemyAttacker>();
            enemyAI = GetComponent<EnemyAI>();
        }
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

        if (canPlayCards && CanAct)
        {
            int pressedPreview = getPressedPreviewButton();
            if (pressedPreview > 0)
            {
                Deck.playerDeck.SelectCard(pressedPreview - 1);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Deck.playerDeck.PlayChord();
            }
        }

        movementComponent.MovementInput = _movementInput;
    }

    /// <summary>
    /// Launch an attack
    /// </summary>
    private void PerformAttack()
    {
        if (useEnemyLogic)
            enemyAttacker.PerformAttack(this);
    }

    /// <summary>
    /// Initializes the movement component
    /// </summary>
    private void Awake()
    {
        movementComponent = GetComponent<Movement>();
    }

    /// <summary>
    /// Gets the card preview button being pressed.
    /// </summary>
    /// <returns> The number corresponding to the current button, -1 if none pressed. </returns>
    static int getPressedPreviewButton()
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
            return enemyAI.GetCurrentTargetPos();

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
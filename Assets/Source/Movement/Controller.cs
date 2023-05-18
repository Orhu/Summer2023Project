using System;
using UnityEngine;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
public class Controller : MonoBehaviour, IActor
{
    [Tooltip("agent mover component to allow the agent to move")]
    private SimpleMovement agentMover;
    
    [Tooltip("movement input")]
    private Vector2 movementInput;
    [Tooltip("movement input")]
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }
    
    [Tooltip("is this agent controllable by inputs?")]
    [SerializeField] private bool isControllable;
    
    [Tooltip("is this agent capable of selecting/using cards?")]
    [SerializeField] private bool canPlayCards;

    /// <summary>
    /// Retrieve inputs where necessary and perform actions as needed
    /// </summary>
    private void Update()
    {
        // if we are controllable, get inputs. otherwise, don't
        if (isControllable)
        {
            movementInput.x = Input.GetAxis("Horizontal");
            movementInput.y = Input.GetAxis("Vertical");
        }

        if (canPlayCards && CanAct)
        {
            int pressedPreview = getPressedPreviewButton();
            if (pressedPreview > 0)
            {
                Deck.playerDeck.TogglePreviewCard(pressedPreview - 1);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Deck.playerDeck.PlayPreviewedCard();
            }
        }
        
        agentMover.MovementInput = movementInput;
    }

    /// <summary>
    /// Launch an attack
    /// </summary>
    private void PerformAttack()
    {
        // TODO attack
    }

    private void Awake()
    {
        agentMover = GetComponent<SimpleMovement>();
    }
    
    /// <summary>
    /// Gets the card preview button being pressed.
    /// </summary>
    /// <returns> The number corresponding to the current button, -1 if none pressed. </returns>
    static int getPressedPreviewButton()
    {
        for (int i = 1; i <= Deck.playerDeck.handSize; i ++)
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
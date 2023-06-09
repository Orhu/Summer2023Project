using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
[RequireComponent(typeof(Movement), typeof(AnimatorController))]
public class PlayerController : MonoBehaviour, IActor
{

    // Movement component to allow the agent to move
    private Movement movementComponent;

    // animator component to make the pretty animations do their thing
    private AnimatorController animatorComponent;

    /// <summary>
    /// Initialize components
    /// </summary>
    private void Awake()
    { 
        movementComponent = GetComponent<Movement>();
        animatorComponent = GetComponent<AnimatorController>();
    }

    /// <summary>
    /// Lost last position.
    /// </summary>
    private void Start()
    {
        if (!SaveManager.autosaveExists) { return; }
        transform.position = SaveManager.savedPlayerPosition;
        GetComponent<Health>().currentHealth = SaveManager.savedPlayerHealth;
    }

    /// <summary>
    /// Retrieve inputs where necessary and perform actions as needed
    /// </summary>
    private void Update()
    {
        movementComponent.movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (canAct)
        {
            int pressedPreview = GetPressedPreviewButton();
            if (pressedPreview > 0)
            {
                Deck.playerDeck.SelectCard(pressedPreview - 1);
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (Deck.playerDeck.PlayChord())
                {
                    animatorComponent.SetTrigger("cast");
                }
            }
            animatorComponent.SetMirror("castLeft", GetActionAimPosition().x - transform.position.x < 0);
        }
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

    #region IActor Implementation

    // Gets whether or not this actor can act.
    IActor.CanActRequest _canAct;
    bool canAct
    {
        get
        {
            bool shouldAct = true;
            _canAct?.Invoke(ref shouldAct);
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


    /// <summary>
    /// Gets the delegate that will fetch whether this actor can act.
    /// </summary>
    /// <returns> A delegate with a out parameter, that allows any subscribed objects to determine whether or not this actor can act. </returns>
    public ref IActor.CanActRequest GetOnRequestCanAct()
    {
        return ref _canAct;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player character, that handles input, movement, and animation.
/// </summary>
public class Player : MonoBehaviour, ICardPlayer
{
    // Global player singleton.
    public static Player _instance;
    // The move speed of the player.
    public float speed = 10.0f;
    
    // The direction the player is tying to move.
    private Vector2 moveDirection = Vector2.zero;
    // The rigid body using for collision detection.
    private Rigidbody2D rigidBody;

    /// <summary>
    /// Initializes singleton and rigid body.
    /// </summary>
    private void Awake()
    {
        _instance = this;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Updates input.
    /// </summary>
    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");

        int pressedPreview = getPressedPreviewButton();
        if (pressedPreview > 0)
        {
            DeckManager.playerDeck.TogglePreviewCard(pressedPreview - 1);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            DeckManager.playerDeck.PlayPreveiwedCards();
        }
    }

    /// <summary>
    /// Updates position.
    /// </summary>
    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Gets the card preview button being pressed.
    /// </summary>
    /// <returns> The number corresponding to the current button, -1 if none pressed. </returns>
    private static int getPressedPreviewButton()
    {
        for (int i = 1; i <= DeckManager.playerDeck.handSize; i ++)
        {
            if (Input.GetButtonDown("PreviewCard" + i))
            {
                return i;
            }
        }
        return -1;
    }

    #region ICardPlayerImplementation
    /// <summary>
    /// Get the transform that the action should be played from.
    /// </summary>
    /// <returns> The player transform. </returns>
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
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    /// <summary>
    /// Gets the collider of this player.
    /// </summary>
    /// <returns> The collider. </returns>
    public Collider GetCollider()
    {
        return GetComponent<Collider>();
    }
    #endregion
}

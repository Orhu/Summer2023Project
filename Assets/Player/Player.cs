using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICardPlayer
{
    public static Player _instance;
    public float speed = 10.0f;
    
    private Vector2 moveDirection = Vector2.zero;
    private Rigidbody2D rigidBody;

    private void Awake()
    {
        _instance = this;
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }


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
    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

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

    public Vector3 getActionSourcePosition()
    {
        return transform.position;
    }
}

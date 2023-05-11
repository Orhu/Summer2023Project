using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;


    private Vector2 moveDirection = Vector2.zero;
    private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}

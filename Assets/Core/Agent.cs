using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    
    private AgentMover agentMover;
    private Vector2 movementInput;
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }

    private void Update()
    {
        // if we are the Player, get inputs. otherwise, don't
        if (gameObject.CompareTag("Player"))
        {
            movementInput.x = Input.GetAxis("Horizontal");
            movementInput.y = Input.GetAxis("Vertical");
        }
        
        agentMover.MovementInput = movementInput;
    }

    private void PerformAttack()
    {
        // TODO attack
    }

    private void Awake()
    {
        agentMover = GetComponent<AgentMover>();
    }



}
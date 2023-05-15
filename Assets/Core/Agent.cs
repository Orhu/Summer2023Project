using System;
using UnityEngine;

/// <summary>
/// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
/// </summary>
public class Agent : MonoBehaviour
{
    // agent mover component to allow the agent to move
    private AgentMover agentMover;
    
    // movement input
    private Vector2 movementInput;
    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }

    /// <summary>
    /// Retrieve inputs where necessary and perform actions as needed
    /// </summary>
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

    /// <summary>
    /// Launch an attack
    /// </summary>
    private void PerformAttack()
    {
        // TODO attack
    }

    private void Awake()
    {
        agentMover = GetComponent<AgentMover>();
    }



}
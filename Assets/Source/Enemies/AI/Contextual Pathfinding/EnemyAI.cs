using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main AI class for enemies with contextual steering behavior
/// </summary>
public class EnemyAI : MonoBehaviour
{
    // list of steering behaviors to consider
    [SerializeField] private List<SteeringBehavior> steeringBehaviors;

    // list of detectors to use
    [SerializeField] private List<Detector> detectors;

    // AI data container
    [SerializeField] private AIData aiData;

    // detection delay is how often the detection occurs,
    // update delay is how often the AI updates itself,
    // attack delay is how often the AI can attempt an attack
    [SerializeField] private float detectionDelay = 0.05f, aiUpdateDelay = 0.06f, attackDelay = 1f;

    // at what range can the enemy attempt to attack?
    [SerializeField] private float attackDistance = 0.5f;

    // inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent OnAttackPressed;
    public UnityEvent<Vector2> OnMovementInput;

    // tracks movement input as determined by the AI
    private Vector2 movementInput;

    // context solver to determine movement direction
    [SerializeField] private ContextSolver movementDirectionSolver;

    // tracks whether the enemy is current following something or not
    bool following = false;

    GameObject player;

    /// <summary>
    /// Begins the AI
    /// </summary>
    private void Start()
    {
        // detecting player and obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);


        // delete this code
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Loops through each detector and attempts to detect, writing their results to aiData
    /// </summary>
    private void PerformDetection()
    {
        // loop through all detectors and detect anything they find, importing the data into aiData
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    /// <summary>
    /// Updates the AI
    /// </summary>
    private void Update()
    {
        // enemy AI movement based on target availability
        // we have a current target...
        if (aiData.currentTarget != null)
        {
            // TODO if we want to look at target, look at aiData.currentTarget.position here
            if (following == false)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        // other targets available but we are targeting nothing, time to acquire a new target...
        else if (aiData.GetTargetsCount() > 0)
        {
            // target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }

        // moving the Agent
        OnMovementInput?.Invoke((player.transform.position - transform.position).normalized);
    }

    /// <summary>
    /// Based on the distance to the current target, lose them, chase, or attack
    /// </summary>
    private IEnumerator ChaseAndAttack()
    {
        // we have no target, stop chasing/attacking...
        if (aiData.currentTarget == null)
        {
            // stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        // we have a target...
        else
        {
            // distance between current target and this unit
            float distance = Vector2.Distance(aiData.currentTarget.position, transform.position);

            // we are in attack range...
            if (distance < attackDistance)
            {
                // attack logic
                movementInput = Vector2.zero;
                OnAttackPressed?.Invoke();
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseAndAttack());
            }
            else
                // we are not in attack range...
            {
                // chase logic
                movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }
        }
    }

    /// <summary>
    /// Gets the position of the current target
    /// </summary>
    /// <returns> The position </returns>
    public Vector2 GetCurrentTargetPos()
    {
        if (aiData.currentTarget != null)
            return aiData.currentTarget.position;

        return Vector2.zero;
    }
}
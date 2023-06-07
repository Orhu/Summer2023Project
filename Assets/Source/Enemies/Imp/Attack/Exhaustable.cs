using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is used to exhaust an enemy. Just call the "ExhaustMe" function in a unity event or somewhere else
/// </summary>
public class Exhaustable : MonoBehaviour
{
    // my state machine
    private BaseStateMachine stateMachine;

    /// <summary>
    /// assign the state machine component
    /// </summary>
    void Awake()
    {
        stateMachine = GetComponent<BaseStateMachine>();
    }

    /// <summary>
    /// Apply exhaustion to me
    /// </summary>
    /// <param name="exhaustDuration"> The duration of the exhaust </param>
    public void ExhaustMe(float exhaustDuration)
    {
        stateMachine.exhausted = true;
        StartCoroutine(BeginExhaustCooldown(exhaustDuration));
    }

    /// <summary>
    /// Begin counting the exhaust cooldown
    /// </summary>
    /// <param name="exhaustDuration"> The duration of the exhaust </param>
    /// <returns></returns>
    IEnumerator BeginExhaustCooldown(float exhaustDuration)
    {
        yield return new WaitForSeconds(exhaustDuration);
        stateMachine.exhausted = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaustable : MonoBehaviour
{
    private BaseStateMachine stateMachine;

    void Awake()
    {
        stateMachine = GetComponent<BaseStateMachine>();
    }

    public void ExhaustMe(float exhaustDuration)
    {
        stateMachine.exhausted = true;
        StartCoroutine(BeginExhaustCooldown(exhaustDuration));
    }

    IEnumerator BeginExhaustCooldown(float exhaustDuration)
    {
        yield return new WaitForSeconds(exhaustDuration);
        stateMachine.exhausted = false;
    }
}
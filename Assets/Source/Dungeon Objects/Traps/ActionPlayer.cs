using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays any action on a regular interval.
/// </summary>
public class ActionPlayer : MonoBehaviour, IActor
{
    [Tooltip("The action to play.")]
    public Action action;
    [Tooltip("The time between playing the action.")]
    public float playRate = 1f;

    void Start()
    {
        StartCoroutine(PlayAction());
    }

    private IEnumerator PlayAction()
    {
        while(true)
        {
            yield return new WaitForSeconds(playRate);
            if (CanAct)
            {
                action.Play(this, 1, new List<ActionModifier>());
            }
        }
    }

    #region IActor Implementation
    public Transform GetActionSourceTransform()
    {
        return transform;
    }

    public Vector3 GetActionAimPosition()
    {
        return transform.position + transform.right;
    }

    public Collider2D GetCollider()
    {
        return null;
    }

    bool CanAct 
    { 
        get 
        {
            bool shouldAct = true;
            canAct?.Invoke(ref shouldAct);
            return shouldAct;
        } 
    }
    IActor.CanActRequest canAct;
    public ref IActor.CanActRequest GetOnRequestCanAct() { return ref canAct; }
    #endregion
}

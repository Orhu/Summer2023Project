using System.Collections;
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

    /// <summary>
    /// Initializes timer.
    /// </summary>
    void Start()
    {
        StartCoroutine(PlayAction());
    }
    
    /// <summary>
    /// Plays the action repeatedly.
    /// </summary>
    /// <returns> The time to wait for the next action. </returns>
    private IEnumerator PlayAction()
    {
        while(true)
        {
            yield return new WaitForSeconds(playRate);
            if (CanAct)
            {
                action.Play(this);
            }
        }
    }

    #region IActor Implementation
    /// <summary>
    /// Get the transform that the action should be played from.
    /// </summary>
    /// <returns> The transform. </returns>
    public Transform GetActionSourceTransform()
    {
        return transform;
    }

    /// <summary>
    /// Get the position that the action should be aimed at.
    /// </summary>
    /// <returns> The position in world space. </returns>
    public Vector3 GetActionAimPosition()
    {
        return transform.position + transform.right;
    }

    /// <summary>
    /// Gets the collider of this actor.
    /// </summary>
    /// <returns> The collider. </returns>
    public Collider2D GetCollider()
    {
        return GetComponent<Collider2D>();
    }

    // Whether or not this can act.
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

    /// <summary>
    /// Gets the delegate that will fetch whether this actor can act.
    /// </summary>
    /// <returns> A delegate with a out parameter, that allows any subscribed objects to determine whether or not this actor can act. </returns>
    public ref IActor.CanActRequest GetOnRequestCanAct() { return ref canAct; }
    #endregion
}

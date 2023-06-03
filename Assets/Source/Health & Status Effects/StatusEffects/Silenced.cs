using UnityEngine;

/// <summary>
/// A status effect that prevents an actor from acting.
/// </summary>
[CreateAssetMenu(fileName = "NewSilenced", menuName = "Status Effects/Silenced")]
public class Silenced : StatusEffect
{
    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    public override StatusEffect CreateCopy(GameObject gameObject)
    {
        Silenced instance = (Silenced)base.CreateCopy(gameObject);

        gameObject.GetComponent<Controller>().GetOnRequestCanAct() += instance.PreventAction;

        return instance;
    }

    /// <summary>
    /// Stacks this effect onto another status effect.
    /// </summary>
    /// <param name="other"> The other particle effect to stack this onto. </param>
    /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
    public override bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.remainingDuration = Mathf.Max(duration, other.remainingDuration);
        return true;
    }

    /// <summary>
    /// Responds to a actors can act request, and prevents action.
    /// </summary>
    /// <param name="CanAct"> The can act variable to be set to false. </param>
    private void PreventAction(ref bool CanAct)
    {
        CanAct = false;
    }

    /// <summary>
    /// Cleans up binding.
    /// </summary>
    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Controller>().GetOnRequestCanAct() -= PreventAction;
    }
}

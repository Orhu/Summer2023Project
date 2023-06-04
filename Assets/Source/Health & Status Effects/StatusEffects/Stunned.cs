using UnityEngine;

/// <summary>
/// A status effect that prevents an actor from acting.
/// </summary>
[CreateAssetMenu(fileName = "NewStunned", menuName = "Status Effects/Stunned")]
public class Stunned : StatusEffect
{
    [Tooltip("The max duration of this effect.")]
    [SerializeField] private float maxDuration = 15f;


    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    public override StatusEffect CreateCopy(GameObject gameObject)
    {
        Stunned instance = (Stunned)base.CreateCopy(gameObject);

        gameObject.GetComponent<Controller>().GetOnRequestCanAct() += instance.PreventAction;
        gameObject.GetComponent<Movement>().requestSpeedModifications += instance.PreventMovement;

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

        other.remainingDuration = Mathf.Min(duration + other.remainingDuration, maxDuration);
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
    /// Responds to a movement components speed modification request, and sets the speed to 0.
    /// </summary>
    /// <param name="speed"> The speed variable to be modified. </param>
    private void PreventMovement(ref float speed)
    {
        speed = 0;
    }

    /// <summary>
    /// Cleans up binding.
    /// </summary>
    private new void OnDestroy()
    {
        base.OnDestroy();
        
        if(gameObject == null) { return; }

        gameObject.GetComponent<Controller>().GetOnRequestCanAct() -= PreventAction;
        gameObject.GetComponent<Movement>().requestSpeedModifications -= PreventMovement;
    }
}

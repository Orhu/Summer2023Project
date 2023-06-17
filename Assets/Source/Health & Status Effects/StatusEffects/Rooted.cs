using UnityEngine;

/// <summary>
/// A status effect that prevents movement entirely.
/// </summary>
[CreateAssetMenu(fileName = "NewRooted", menuName = "Status Effects/Rooted")]
public class Rooted : StatusEffect
{
    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    public override StatusEffect CreateCopy(GameObject gameObject)
    {
        Rooted instance = (Rooted)base.CreateCopy(gameObject);

        if (gameObject.GetComponent<Movement>() is Movement movment)
        {
            movment.requestSpeedModifications += instance.PreventMovement;
        }

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

        if (gameObject == null) { return; }

        if (gameObject.GetComponent<Movement>() is Movement movment)
        {
            movment.requestSpeedModifications -= PreventMovement;
        }        
    }
}

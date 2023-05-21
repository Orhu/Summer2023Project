using UnityEngine;

/// <summary>
/// A status effect that prevents movement entirely.
/// </summary>
[CreateAssetMenu(fileName = "Root", menuName = "Status Effects [Don't Create]/Root")]
public class Root : StatusEffect
{
    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Root instance = (Root)base.Instantiate(gameObject);

        gameObject.GetComponent<Movement>().requestSpeedModifications += instance.PreventMovement;

        return instance;
    }


    /// <summary>
    /// Stacks this effect onto another status effect.
    /// </summary>
    /// <param name="other"> The other particle effect to stack this onto. </param>
    /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
    internal override bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.Duration += Duration;
        return true;
    }

    /// <summary>
    /// Responds to a movement components speed modification request, and sets the speed to 0.
    /// </summary>
    /// <param name="speed"> The speed variable to be modified. </param>
    void PreventMovement(ref float speed)
    {
        speed = 0;
    }

    /// <summary>
    /// Cleans up binding.
    /// </summary>
    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Movement>().requestSpeedModifications -= PreventMovement;
    }
}

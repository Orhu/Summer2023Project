using UnityEngine;

/// <summary>
/// A status effect that prevents health from receiving attacks.
/// </summary>
[CreateAssetMenu(fileName = "NewExhausted", menuName = "Status Effects/Exhausted")]
public class Exhausted : StatusEffect
{
    [Tooltip("The amount incoming damage is multiplied by.")]
    public float damageMultiplier = 2f;


    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    public override StatusEffect CreateCopy(GameObject gameObject)
    {
        Exhausted instance = (Exhausted)base.CreateCopy(gameObject);

        gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.MutiplyDamage;
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
        return true;
    }

    /// <summary>
    /// Responds to a health's incoming damage modification request, and prevents the attack from passing.
    /// </summary>
    /// <param name="attack"> The attack to prevent. </param>
    private void MutiplyDamage(ref DamageData attack)
    {
        attack.damage = (int)(attack.damage * damageMultiplier);
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

        gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= MutiplyDamage;
        gameObject.GetComponent<Movement>().requestSpeedModifications -= PreventMovement;
    }
}

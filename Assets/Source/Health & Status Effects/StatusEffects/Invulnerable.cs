using UnityEngine;

/// <summary>
/// A status effect that prevents health from receiving attacks.
/// </summary>
[CreateAssetMenu(fileName = "Invulnerable", menuName = "Status Effects/Invulnerable")]
public class Invulnerable : StatusEffect
{
    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    public override StatusEffect Instantiate(GameObject gameObject)
    {
        Invulnerable instance = (Invulnerable)base.Instantiate(gameObject);

        gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.PreventAttack;

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

        other.Duration += Duration;
        return true;
    }

    /// <summary>
    /// Responds to a health's incoming damage modification request, and prevents the attack from passing.
    /// </summary>
    /// <param name="attack"> The attack to prevent. </param>
    void PreventAttack(ref DamageData attack)
    {
        DamageData prevousAttack = attack;
        attack = new DamageData(0, attack.damageType, prevousAttack.causer);
    }

    /// <summary>
    /// Cleans up binding.
    /// </summary>
    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= PreventAttack;
    }
}

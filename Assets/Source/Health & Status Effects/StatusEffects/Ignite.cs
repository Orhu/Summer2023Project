using UnityEngine;

/// <summary>
/// A status effect the does damage over time proportional to stacks, and who's duration is reset on each stack.
/// </summary>
[CreateAssetMenu(fileName = "Ignite", menuName = "Status Effects/Ignite")]
public class Ignite : StatusEffect
{
    [SerializeField]
    [Tooltip("Damage per second per stack.")]
    float dps = 1f;

    // The time until the next damage tick is applied
    float timeToDamage;
    // The number of times this status effect has been applied.
    int stacks = 1;
    public override int Stacks
    {
        protected set
        {
            dps *= (float)value / stacks;
            timeToDamage = 1f / dps;
            stacks = value;
        }
        get { return stacks; }
    }

    /// <summary>
    /// Initialize timeToDamage
    /// </summary>
    private void Awake()
    {
        timeToDamage = 1f / dps;
    }


    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Ignite instance = (Ignite)base.Instantiate(gameObject);

        instance.Stacks = Stacks;
        instance.Duration = Duration;
        instance.dps = dps;

        return instance;
    }

    /// <summary>
    /// Causes damage over time.
    /// </summary>
    internal override void Update()
    {
        base.Update();
        timeToDamage -= Time.deltaTime;
        if (timeToDamage <= 0)
        {
            gameObject.GetComponent<Health>().ReceiveAttack(new AttackData(1, AttackData.DamageType.Special, this));
            timeToDamage += 1f / dps;
        }
    }

    /// <summary>
    /// Stacks this effect onto another status effect.
    /// </summary>
    /// <param name="other"> The other particle effect to stack this onto. </param>
    /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
    internal override bool Stack(StatusEffect other)
    {
        if (base.Stack(other))
        {
            other.Duration = Duration;
            return true;
        }
        return false;
    }
}

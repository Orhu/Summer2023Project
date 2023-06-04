using UnityEngine;

/// <summary>
/// A status effect the does damage over time proportional to stacks, and who's duration is reset on each stack.
/// </summary>
[CreateAssetMenu(fileName = "NewPoisoned", menuName = "Status Effects/Poisoned")]
public class Poisoned : StatusEffect
{
    [Tooltip("The damage a single stack of poisoned will deal.")]
    [SerializeField] private int damage = 1;

    [Tooltip("The amount the damage is multiplied by every time this stacks.")]
    [SerializeField] private int perStackDamageMultiplier = 2;

    [Tooltip("Damage per second per stack.")]
    [SerializeField] private float perStackAdditionalDuration = 1f;

    [Tooltip("The time in seconds between dealing damage.")]
    [SerializeField] private float tickInterval = 2f;

    // The time until the next damage tick is applied
    private float timeToDamage;

    // The number of times this status effect has been applied.
    private int _stacks = 1;
    public override int stacks
    {
        protected set
        {
            remainingDuration = duration + value * perStackAdditionalDuration;
            damage *= perStackDamageMultiplier;
            _stacks = value;
        }
        get { return _stacks; }
    }

    /// <summary>
    /// Initialize timeToDamage
    /// </summary>
    private void Awake()
    {
        timeToDamage = tickInterval;
    }

    /// <summary>
    /// Causes damage over time.
    /// </summary>
    public override void Update()
    {
        base.Update();
        timeToDamage -= Time.deltaTime;
        if (timeToDamage <= 0)
        {
            gameObject.GetComponent<Health>().ReceiveAttack(new DamageData(damage, DamageData.DamageType.Special, this));
            timeToDamage += tickInterval;
        }
    }
}

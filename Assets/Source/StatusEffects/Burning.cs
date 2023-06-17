using UnityEngine;

/// <summary>
/// A status effect the does damage over time proportional to stacks, and who's duration is reset on each stack.
/// </summary>
[CreateAssetMenu(fileName = "NewBurning", menuName = "Status Effects/Burning")]
public class Burning : StatusEffect
{
    [Tooltip("Damage per second per stack.")]
    [SerializeField] private float dps = 2f;

    [Tooltip("The max duration of this effect.")]
    [SerializeField] private float maxDuration = 4f;

    // The time until the next damage tick is applied
    private float timeToDamage;

    // The number of times this status effect has been applied.
    private int _stacks = 1;
    public override int stacks
    {
        protected set
        {
            remainingDuration = Mathf.Min(remainingDuration + duration, 4f);
            _stacks = value;
        }
        get { return _stacks; }
    }

    /// <summary>
    /// Initialize timeToDamage
    /// </summary>
    private void Awake()
    {
        timeToDamage = 1f / dps;
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
            gameObject.GetComponent<Health>().ReceiveAttack(new DamageData(1, DamageData.DamageType.Special, this, true), true);
            timeToDamage += 1f / dps;
        }
    }

    /// <summary>
    /// Stacks this effect onto another status effect.
    /// </summary>
    /// <param name="other"> The other particle effect to stack this onto. </param>
    /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
    public override bool Stack(StatusEffect other)
    {
        if (base.Stack(other))
        {
            other.remainingDuration = Mathf.Min(other.remainingDuration + duration, maxDuration);
            return true;
        }
        return false;
    }
}

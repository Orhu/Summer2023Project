using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A status effect that permanently stays on targets until they are cleansed, purified, or healed by a healing item. Does damage over time until the affected entity dies. Does not stack
    /// </summary>
    [CreateAssetMenu(fileName = "NewToxic", menuName = "Status Effects/Toxic")]
    public class Toxic : StatusEffect
    {
        [Tooltip("The damage this instance of the toxic status will apply.")]
        [SerializeField] private int damage = 1;

        [Tooltip("The amount the damage is multiplied by every time this stacks.")]
        [SerializeField] private int perStackDamageMultiplier = 1; // essentially no stacks

        [Tooltip("Duration of the status effect")]
        [SerializeField] private float perStackAdditionalDuration = 999999f; // may want to formally support this as a non-exhausting status effect, if possible

        [Tooltip("The time in seconds between dealing damage.")]
        [SerializeField] private float tickInterval = 1f;

        [Tooltip("The amount to speed up the tick rate each stack.")]
        [SerializeField] private float tickIntervalDecreaseRate = 0.5f;

        // The time until the next damage tick is applied
        private float timeToDamage;

        // The number of times this status effect has been applied. Note that since this status effect does not stack, stack behavior is just to reset duration back to the maximum.
        private int _stacks = 1;
        public override int stacks
        {
            protected set
            {
                remainingDuration = perStackAdditionalDuration; // reset to max duration on stack
                damage *= perStackDamageMultiplier;
                tickInterval *= tickIntervalDecreaseRate;
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
                gameObject.GetComponent<Health>().ReceiveAttack(new DamageData(damage, DamageData.DamageType.Special, null, false), true);
                timeToDamage += tickInterval;
            }
        }

        // TODO: need functionality to suport removal of this status when the affected entity picks up a healing item (should only ever be something the player does).
    }
}
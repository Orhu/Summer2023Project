using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A status effect that reduces cooldowns while applied.
    /// </summary>
    [CreateAssetMenu(menuName = "Status Effects/Focused")]
    public class Focused : StatusEffect
    {
        [Tooltip("The amount cooldown durations will be multiplied by.")]
        [SerializeField] private float cooldownMultiplier = 0.5f;

        [Tooltip("Duration of the status effect")]
        [SerializeField] private float perStackAdditionalDuration = 2; 

        // The number of times this status effect has been applied. 
        private int _stacks = 1;
        public override int stacks
        {
            protected set
            {
                remainingDuration += perStackAdditionalDuration;
                _stacks = value;
            }
            get { return _stacks; }
        }

        /// <summary>
        /// Causes cooldowns reduction.
        /// </summary>
        public override void Update()
        {
            base.Update();
            gameObject.GetComponent<Deck>()?.SubtractFromCooldowns((1/cooldownMultiplier) * Time.deltaTime);
        }

        public override StatusEffectType StatusType()
        {
            return StatusEffectType.Focused;
        }
    }
}
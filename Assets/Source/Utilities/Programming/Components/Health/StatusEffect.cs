using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// The base for any status effect. Has a duration and VFX and can stack with other status effects.
    /// </summary>
    public abstract class StatusEffect : ScriptableObject
    {
        [Tooltip("The Duration this status effect will be applied for")]
        [Min(0.0166666667f)]
        public float duration = 1f;

        // The time remaining for this status effect.
        private float _remainingDuration;
        public float remainingDuration
        {
            get { return _remainingDuration; }
            set
            {
                _remainingDuration = Mathf.Max(value, 0);
                if (_remainingDuration == 0)
                {
                    Destroy(this);
                }
            }
        }

        [Tooltip("The game object spawned on the effected game object as a visual indicator")]
        [SerializeField] protected GameObject visualEffect;


        // The number of times this status effect has been applied.
        public virtual int stacks { get; protected set; } = 1;

        // The game object this is applied to.
        protected GameObject gameObject;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect. </param>
        /// <returns> The status effect that was created. </returns>
        public virtual StatusEffect CreateCopy(GameObject gameObject)
        {
            if (gameObject == null || !gameObject.scene.isLoaded) { return null; }
            StatusEffect instance = Instantiate(this);

            instance.remainingDuration = duration;
            instance.gameObject = gameObject;

            if (visualEffect != null)
            {
                instance.visualEffect = Instantiate(visualEffect);
                instance.visualEffect.transform.parent = gameObject.transform;
                instance.visualEffect.transform.localPosition = Vector3.zero;
            }

            return instance;
        }

        /// <summary>
        /// Stacks this effect onto another status effect.
        /// </summary>
        /// <param name="other"> The other particle effect to stack this onto. </param>
        /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
        public virtual bool Stack(StatusEffect other)
        {
            if (other.GetType() != GetType())
            {
                return false;
            }

            other.stacks += stacks;
            return true;
        }

        public abstract StatusEffectType StatusType();

        /// <summary>
        /// Called every tick and updates the duration.
        /// </summary>
        public virtual void Update()
        {
            remainingDuration -= Time.deltaTime;
        }

        /// <summary>
        /// Cleans up the VFX.
        /// </summary>
        protected void OnDestroy()
        {
            Destroy(visualEffect);
        }
    }
}
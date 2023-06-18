using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A status effect that prevents movement entirely.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSlowed", menuName = "Status Effects/Slowed")]
    public class Slowed : StatusEffect
    {
        [Tooltip("The percent to reduce the movement speed by per stack")]
        [Range(0f, 1f)]
        [SerializeField] private float slowAmount = 0.25f;

        /// <summary>
        /// Creates a new status effect that is a copy of the caller.
        /// </summary>
        /// <param name="gameObject"> The object to apply the status effect.</param>
        /// <returns> The status effect that was created. </returns>
        public override StatusEffect CreateCopy(GameObject gameObject)
        {
            Slowed instance = (Slowed)base.CreateCopy(gameObject);

            gameObject.GetComponent<Movement>().requestSpeedModifications += instance.SlowMovement;

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
        private void SlowMovement(ref float speed)
        {
            speed *= Mathf.Pow(slowAmount, stacks);
        }

        /// <summary>
        /// Cleans up binding.
        /// </summary>
        private new void OnDestroy()
        {
            if (gameObject != null)
            {
                gameObject.GetComponent<Movement>().requestSpeedModifications -= SlowMovement;
            }
            base.OnDestroy();
        }
    }
}
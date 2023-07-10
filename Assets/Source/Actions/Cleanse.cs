using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action that removes all status effects from the actor.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCleanse", menuName = "Cards/Actions/Cleanse")]
    public class Cleanse : Action
    {
        [Tooltip("The delay before it is applied")] [Min(0f)]
        [SerializeField] private float delay = 0f;

        /// <summary>
        /// Plays this action and causes all its effects.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            if (delay <= 0)
            {
                actor.GetActionSourceTransform().GetComponent<Health>().Cleanse();
            }
            else
            {
                actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(DelayedCleanse(actor));
            }
        }

        /// <summary>
        /// Removes all status effects.
        /// </summary>
        private IEnumerator DelayedCleanse(IActor actor)
        {
            yield return new WaitForSeconds(delay);
            actor.GetActionSourceTransform().GetComponent<Health>().Cleanse();
        }
    }
}
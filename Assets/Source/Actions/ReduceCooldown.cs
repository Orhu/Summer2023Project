using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action that spawns an thing at the aim location.
    /// </summary>
    [CreateAssetMenu(menuName = "Cards/Actions/Reduce Cooldowns")]
    public class ReduceCooldowns : Action
    {
        [Tooltip("The amount in seconds to subtract from all current cooldowns in seconds")] [Min(0f)]
        [SerializeField] private float cooldownReduction = 0f;

        /// <summary>
        /// Plays this action and causes all its effects.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            actor.GetActionSourceTransform().GetComponent<Deck>().SubtractFromCooldowns(cooldownReduction);
        }
    }
}
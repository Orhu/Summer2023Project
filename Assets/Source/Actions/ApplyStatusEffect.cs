using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action that applied a status effect to the actor that played it.
    /// </summary>
    [CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Cards/Actions/ApplyStatusEffect")]
    public class ApplyStatusEffect : Action
    {
        [Tooltip("The status effect to apply")]
        [SerializeField] private List<StatusEffect> statusEffects;

        [Tooltip("The delay before it is applied")] [Min(0f)]
        [SerializeField] private float delay = 0f;

        [Tooltip("Whether or not to create a damage number prefab on trigger")]
        [SerializeField] private bool dontShowDamageNumber = false;

        /// <summary>
        /// Plays this action and causes all its effects.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            if (delay <= 0)
            {
                actor.GetActionSourceTransform().GetComponent<Health>().ReceiveAttack(new DamageData(statusEffects, actor.GetActionSourceTransform().gameObject, false, dontShowDamageNumber));
            }
            else
            {
                actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(DelayedApply(actor));
            }

            AudioManager.instance.PlaySoundOnActor(actionSound, actor);

        }

        /// <summary>
        /// Applies the status effect.
        /// </summary>
        private IEnumerator DelayedApply(IActor actor)
        {
            yield return new WaitForSeconds(delay);
            actor.GetActionSourceTransform().GetComponent<Health>().ReceiveAttack(new DamageData(statusEffects, actor.GetActionSourceTransform().gameObject, false, dontShowDamageNumber));
        }
    }
}
using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action that applied a status effect to the actor that played it.
/// </summary>
[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Cards/Actions/ApplyStatusEffect")]
public class ApplyStatusEffect : Action
{
    [Tooltip("The status effect to apply")][SerializeField]
    private List<StatusEffect> statusEffects;

    /// <summary>
    /// Plays this action and causes all its effects.
    /// </summary>
    /// <param name="actor"> The actor that will be playing this action. </param>
    /// <param name="ignoredObjects"> The objects this action will ignore. </param>
    public override void Play(IActor actor, List<GameObject> ignoredObjects)
    {
        actor.GetActionSourceTransform().GetComponent<Health>().ReceiveAttack(new DamageData(statusEffects, actor.GetActionSourceTransform().gameObject));
    }
}

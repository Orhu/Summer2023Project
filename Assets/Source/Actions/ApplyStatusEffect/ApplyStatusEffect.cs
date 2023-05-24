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
    [SerializeField]
    [Tooltip("The status effect to apply")]
    private List<StatusEffect> statusEffects;

    /// <summary>
    /// Gets the formated description of this card.
    /// </summary>
    /// <returns> The description with any Serialized Field names that appear in [] replaced with their actual value.</returns>
    public override void Play(IActor actor, List<GameObject> ignoredObjects)
    {
        actor.GetActionSourceTransform().GetComponent<Health>().ReceiveAttack(new AttackData(0, statusEffects, actor.GetActionSourceTransform().gameObject));
    }
}

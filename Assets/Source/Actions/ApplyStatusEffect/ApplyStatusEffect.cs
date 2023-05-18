using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewApplyStatusEffect", menuName = "Cards/Actions/ApplyStatusEffect")]
public class ApplyStatusEffect : Action
{
    [SerializeField]
    private List<StatusEffect> statusEffects;

    public override void Play(IActor actor, int numStacks, List<ActionModifier> modifiers)
    {
        actor.GetActionSourceTransform().GetComponent<Health>().ReceiveAttack(new Attack(0, statusEffects, actor.GetActionSourceTransform().gameObject) * numStacks);
    }


    public override void AddStacksToPreview(IActor actor, int numStacks) { }

    public override void ApplyModifiersToPreview(IActor actor, List<ActionModifier> actionModifiers) { }

    public override void CancelPreview(IActor actor) { }

    public override void Preview(IActor actor) { }

    public override void RemoveModifiersFromPreview(IActor actor, List<ActionModifier> actionModifiers) { }
}

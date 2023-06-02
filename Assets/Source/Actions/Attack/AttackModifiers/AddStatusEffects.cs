using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes an attack deal additional status effects.
/// </summary>
[CreateAssetMenu(fileName = "NewAddStatusEffects", menuName = "Cards/AttackModifers/Add[Stat]/AddStatusEffects")]
public class AddStatusEffects : AttackModifier
{
    [Tooltip("The additional status effects to apply")]
    [SerializeField] private List<StatusEffect> statusEffects;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.attackData = value.attackData + statusEffects;
        }
    }
}

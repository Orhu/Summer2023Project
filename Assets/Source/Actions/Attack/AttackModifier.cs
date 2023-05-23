using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewAttackModifer", menuName = "Cards/ActionModifers/AttackModifer")]
public class AttackModifier : ScriptableObject
{
    [Tooltip("The damage to add to the attack")]
    public int damageToAdd = 0;
    [Tooltip("The status effects to add to the attack")]
    public List<StatusEffect> statusEffectToAdd;
    [Tooltip("The number of times the attack will be multiplied by before adding the effects of this modifier")]
    [Min(0)]
    public int attackMultiplier = 1;

    /// <summary>
    /// Modifies the given attack.
    /// </summary>
    /// <param name="attack"> The attack to modify </param>
    public void ModifyAttack(ref AttackData attack)
    {
        attack = ((attack * attackMultiplier) + damageToAdd) + statusEffectToAdd;
    }
}

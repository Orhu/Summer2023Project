using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base for anything modifier that changes attacks.
/// </summary>
public abstract class AttackModifier : ActionModifier
{
    /// <summary>
    /// Modifies the given attack.
    /// </summary>
    /// <param name="attack"> The attack to modify </param>
    public abstract void ModifyAttack(Attack attack);

    /// <summary>
    /// Unmodifies the given attack.
    /// </summary>
    /// <param name="attack"> The attack to unmodify </param>
    public abstract void UnmodifyAttack(Attack attack);
}

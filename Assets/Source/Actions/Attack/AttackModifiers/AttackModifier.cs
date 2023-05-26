using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>

namespace CardSystem.Effects
{
    public abstract class AttackModifier : ScriptableObject
    {
        // The projectile this modifies
        public abstract Projectile ModifiedProjectile { set; protected get; }
    }
}

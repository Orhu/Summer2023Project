using UnityEngine;

/// <summary>
/// An the base for all modifier that effect projectiles
/// </summary>
public abstract class AttackModifier : ScriptableObject
{
    // The projectile this modifies
    public abstract Projectile modifiedProjectile { set; }
}

/// <summary>
/// A base for modifiers that can only be used as a duplicate modifier
/// </summary>
public abstract class DuplicateAttackModifier : AttackModifier {}

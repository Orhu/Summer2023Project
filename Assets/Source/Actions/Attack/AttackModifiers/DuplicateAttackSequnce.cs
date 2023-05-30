using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewDuplicateAttackSequence", menuName = "Cards/AttackModifers/DuplicateAttackSequence")]
public class DuplicateAttackSequence : AttackModifier
{
    // The projectile this modifies
    public override Projectile<ProjectileSpawnInfo> modifiedProjectile
    {
        set
        {

        }
    }
}

using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewPhantasmal", menuName = "Cards/AttackModifers/Phantasmal")]
public class Phantasmal : AttackModifier
{
    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.gameObject.layer = LayerMask.NameToLayer("PhantasmalProjectile");
        }
    }
}

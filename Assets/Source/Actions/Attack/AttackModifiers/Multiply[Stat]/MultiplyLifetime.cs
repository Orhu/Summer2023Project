using UnityEngine;

/// <summary>
/// Makes an attack's projectile last longer.
/// </summary>
[CreateAssetMenu(fileName = "NewMultiplyLifetime", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplyLifetime")]
public class MultiplyLifetime : AttackModifier
{
    [Tooltip("The amount to multiply the lifetime by")]
    [SerializeField] private float lifetimeFactor = 1f;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.remainingLifetime *= lifetimeFactor;
        }
    }
}

using UnityEngine;

/// <summary>
/// Makes an attack's projectile last longer.
/// </summary>
[CreateAssetMenu(fileName = "NewMultiplySpeed", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplySpeed")]
public class MultiplySpeed : AttackModifier
{
    [Tooltip("The additional initial speed to add in tiles/s.")]
    [SerializeField] private float initialSpeedFactor = 1f;

    [Tooltip("The additional acceleration to add in tiles/s^2.")]
    [SerializeField] private float accelerationFactor = 1f;

    [Tooltip("The additional max speed to add in tiles/s.")]
    [SerializeField] private float maxSpeedFactor = 1f;

    [Tooltip("The additional min speed to add in tiles/s.")]
    [SerializeField] private float minSpeedFactor = 1f;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.speed *= initialSpeedFactor;
            value.acceleration *= accelerationFactor;
            value.maxSpeed *= maxSpeedFactor;
            value.minSpeed *= minSpeedFactor;
        }
    }
}
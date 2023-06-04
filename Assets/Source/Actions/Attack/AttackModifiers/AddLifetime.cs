using UnityEngine;

/// <summary>
/// Makes an attack's projectile last longer.
/// </summary>
[CreateAssetMenu(fileName = "NewAddLifetime", menuName = "Cards/AttackModifers/Add[Stat]/AddLifetime")]
public class AddLifetime : AttackModifier
{
    [Tooltip("The additional lifetime in seconds")]
    [SerializeField] private float lifetime;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.remainingLifetime += lifetime;
        }
    }
}

using UnityEngine;

/// <summary>
/// Adds to scale to a projectile.
/// </summary>
[CreateAssetMenu(fileName = "NewAddScale", menuName = "Cards/AttackModifers/Add[Stat]/AddScale")]
public class AddScale : AttackModifier
{
    [Tooltip("The homing speed to add.")]
    [SerializeField] private float scale;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            value.transform.localScale = new Vector3(value.transform.localScale.x + scale, value.transform.localScale.y + scale, value.transform.localScale.z + scale);
        }
    }
}

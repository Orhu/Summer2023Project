using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewKnockback", menuName = "Cards/AttackModifers/Knockback")]
public class Knockback : AttackModifier
{
    private enum PushDirection 
    {
        AwayFromProjectile,
        InProjectileForwardDirection,
        AwayFromSpawner,
        InSpawnerForwardDirection
    }

    [Tooltip("The force in tiles/s added to hit objects.")]
    [SerializeField] private KnockbackInfo knockback;

    [Tooltip("The force in tiles/s added to hit objects.")]
    [SerializeField] private PushDirection pushDirection;

    // The projectile this modifies
    private GameObject knockbackSource = null;
    public override Projectile modifiedProjectile
    {
        set
        {
            switch (pushDirection)
            {
                case PushDirection.AwayFromProjectile:
                    knockbackSource = value.gameObject;
                    break;

                case PushDirection.InProjectileForwardDirection:
                    knockbackSource = value.gameObject;
                    break;

                case PushDirection.AwayFromSpawner:
                    knockbackSource = value.actor.GetActionSourceTransform().gameObject;
                    break;

                case PushDirection.InSpawnerForwardDirection:
                    knockbackSource = value.actor.GetActionSourceTransform().gameObject;
                    break;
            }
            value.onOverlap += ApplyKnockback;
        }
    }

    /// <summary>
    /// Applies knockback to the hit collider.
    /// </summary>
    /// <param name="collision"> The collider that was hit. </param>
    private void ApplyKnockback(Collider2D collision)
    {
        if (collision.GetComponent<Movement>() is Movement movementComponent)
        {
            Vector2 direction;
            if (pushDirection == PushDirection.InProjectileForwardDirection || pushDirection == PushDirection.InSpawnerForwardDirection)
            {
                direction = knockbackSource.transform.right;
            }
            else
            {
                direction = (collision.transform.position - knockbackSource.transform.position).normalized;
            }
            movementComponent.Knockback(direction, knockback);
        }
    }
}

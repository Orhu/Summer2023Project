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
    [SerializeField] private float knockbackVelocity = 4;
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
            value.onHit += ApplyKnockback;
        }
    }

    private void ApplyKnockback(Collider2D collider)
    {
        Vector2 impulse;
        if (pushDirection == PushDirection.InProjectileForwardDirection || pushDirection == PushDirection.InSpawnerForwardDirection)
        {
            impulse = knockbackSource.transform.right;
        }
        else
        {
            impulse = (collider.transform.position - knockbackSource.transform.position).normalized;
        }
        impulse *= knockbackVelocity;

        collider.GetComponent<Rigidbody2D>().velocity += impulse;
    }
}

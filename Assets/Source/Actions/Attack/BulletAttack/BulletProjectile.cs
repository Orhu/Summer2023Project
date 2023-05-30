using UnityEngine;
using static Attack;

/// <summary>
/// A projectile that moves in a straight line.
/// </summary>
public class BulletProjectile : Projectile
{
    // The bullet attack that spawned this.
    BulletAttack bulletAttack;
    // The index in the spawn sequence that this was created from.
    public int bulletIndex;

    /// <summary>
    /// Initializes position and rotation.
    /// </summary>
    new void Start()
    {
        bulletAttack = attack as BulletAttack;

        // Position
        switch (attack.spawnLocation)
        {
            case SpawnLocation.Actor:
                transform.position = actor.GetActionSourceTransform().position;
                break;
            case SpawnLocation.AimPosition:
                transform.position = actor.GetActionAimPosition();
                break;
        }
        float random = Random.Range(0f, Mathf.PI * 2f);
        transform.position += (Vector3)Random.insideUnitCircle * bulletAttack.randomOffset;

        // Rotation
        Vector3 aimDirection = (GetAimTarget(attack.aimMode) - actor.GetActionSourceTransform().position).normalized;
        float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        float randomAngle = Random.Range(bulletAttack.randomAngle / -2f, bulletAttack.randomAngle / 2f);
        transform.rotation = Quaternion.AngleAxis(aimRotation + randomAngle + bulletAttack.spawnSequence[bulletIndex].angle, Vector3.forward);

        // Position offset
        transform.position += Quaternion.AngleAxis(aimRotation, Vector3.forward) * bulletAttack.spawnSequence[bulletIndex].offset;


        base.Start();
    }
}

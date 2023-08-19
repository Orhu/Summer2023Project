using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// A projectile that moves in a straight line.
    /// </summary>
    public class BulletProjectile : Projectile
    {
        // The bullet attack that spawned this.
        BulletAttack bulletAttack;

        /// <summary>
        /// Initializes position and rotation.
        /// </summary>
        new void Start()
        {
            bulletAttack = attack as BulletAttack;
            BulletSpawnInfo bulletSpawnInfo = spawnSequence[index] as BulletSpawnInfo;

            // Position
            transform.position = GetSpawnLocation() ?? Vector2.zero;
            Vector3 aimDirection = (GetAimTarget(attack.aimMode) - transform.position).normalized;
            transform.position += (Vector3)Random.insideUnitCircle * bulletAttack.randomOffset;

            // Rotation
            float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            float randomAngle = Random.Range(bulletAttack.randomAngle / -2f, bulletAttack.randomAngle / 2f);
            transform.rotation = Quaternion.AngleAxis(aimRotation + randomAngle + bulletSpawnInfo.angle, Vector3.forward);

            // Position offset
            transform.position += Quaternion.AngleAxis(aimRotation, Vector3.forward) * bulletSpawnInfo.offset;

            base.Start();
            velocity = speed * transform.right;
        }
    }
}
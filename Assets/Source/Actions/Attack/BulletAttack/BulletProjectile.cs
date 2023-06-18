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
            transform.position = GetSpawnLocation();
            float random = Random.Range(0f, Mathf.PI * 2f);
            transform.position += (Vector3)Random.insideUnitCircle * bulletAttack.randomOffset;

            // Rotation
            Vector3 aimDirection = (GetAimTarget(attack.aimMode) - GetSpawnLocation()).normalized;
            float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            float randomAngle = Random.Range(bulletAttack.randomAngle / -2f, bulletAttack.randomAngle / 2f);
            transform.rotation = Quaternion.AngleAxis(aimRotation + randomAngle + bulletSpawnInfo.angle, Vector3.forward);

            // Position offset
            transform.position += Quaternion.AngleAxis(aimRotation, Vector3.forward) * bulletSpawnInfo.offset;


            base.Start();
        }
    }
}
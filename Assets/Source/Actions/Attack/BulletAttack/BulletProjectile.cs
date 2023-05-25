using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSystem.Effects.Attack;

public class BulletProjectile : Projectile
{
    BulletAttack bulletAttack;
    // The index in the spawn sequence that this was created from.
    internal int bulletIndex;

    // Start is called before the first frame update
    new void Start()
    {
        bulletAttack = attack as BulletAttack;


        Vector3 direction;
        if (attack.isAimed)
        {
            direction = (actor.GetActionAimPosition() - actor.GetActionSourceTransform().position).normalized;
        }
        else
        {
            direction = (Target.transform.position - actor.GetActionSourceTransform().position).normalized;
        }
        float aimRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float randomAngle = Random.Range(bulletAttack.randomAngle / -2f, bulletAttack.randomAngle / 2f);
        transform.rotation = Quaternion.AngleAxis(aimRotation + randomAngle + bulletAttack.spawnSequence[bulletIndex].angle, Vector3.forward);


        switch (attack.spawnLocation)
        {
            case SpawnLocation.Actor:
                transform.position = actor.GetActionSourceTransform().position;
                break;
            case SpawnLocation.AimPosition:
                transform.position = actor.GetActionAimPosition();
                break;
        }
        
        transform.position += Quaternion.AngleAxis(aimRotation, Vector3.forward) * bulletAttack.spawnSequence[bulletIndex].offset;

        float random = Random.Range(0f, Mathf.PI * 2f);
        transform.position += (Vector3)Random.insideUnitCircle * bulletAttack.randomOffset;

        base.Start();
    }
}

using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSystem.Effects.Attack;

public class BulletProjectile : Projectile
{

    // Start is called before the first frame update
    new void Start()
    {
        switch (attack.spawnLocation)
        {
            case SpawnLocation.Actor:
                transform.position = actor.GetActionSourceTransform().position;
                break;
            case SpawnLocation.AimPosition:
                transform.position = actor.GetActionAimPosition();
                break;
        }

        Vector3 direction;
        if (attack.isAimed)
        {
            direction = (actor.GetActionAimPosition() - actor.GetActionSourceTransform().position).normalized;
        }
        else
        {
            direction = (Target.transform.position - actor.GetActionSourceTransform().position).normalized;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        base.Start();
    }
}

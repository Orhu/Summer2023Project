using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardSystem.Effects.Attack;

public class OrbitProjectile : Projectile
{
    OrbitAttack orbitAttack;
    // The index in the spawn sequence that this was created from.
    internal int orbitIndex;

    // Start is called before the first frame update
    new void Start()
    {
        orbitAttack = attack as OrbitAttack;

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

        OrbitAttack.OrbitSpawnInfo orbitSpawnInfo = orbitAttack.spawnSequence[orbitIndex];

        Vector3 aimDirection = (GetAimTarget(attack.aimMode) - actor.GetActionSourceTransform().position).normalized;
        float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float startingRotation = orbitSpawnInfo.startingAngle + Random.Range(orbitAttack.randomStartingAngle / -2f, orbitAttack.randomStartingAngle / 2f);

        transform.position += Quaternion.AngleAxis(startingRotation + aimRotation, Vector3.forward) * Vector2.right * (orbitSpawnInfo.radius + Random.Range(orbitAttack.randomRadius / -2f, orbitAttack.randomRadius / 2f));
        transform.rotation = Quaternion.AngleAxis(startingRotation + aimRotation + 90 * OrbitSign(), Vector3.forward);

        base.Start();
    }

    new void FixedUpdate()
    {
        //transform.rotation *= Quaternion.AngleAxis(OrbitSign() * Mathf.Rad2Deg * speed * Time.fixedDeltaTime / orbitAttack.spawnSequence[orbitIndex].radius, Vector3.forward);
        //base.FixedUpdate();
    }

    float OrbitSign()
    {
        return (orbitAttack.spawnSequence[orbitIndex].orbitDirection == OrbitAttack.RotationDirection.Clockwise ? -1f : 1f);
    }
}

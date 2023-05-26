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

    float radius;
    Vector3 lastSpawnLocationPosition;
    float previousHomingSign = 0f;

    // Start is called before the first frame update
    new void Start()
    {
        orbitAttack = attack as OrbitAttack;
        OrbitAttack.OrbitSpawnInfo orbitSpawnInfo = orbitAttack.spawnSequence[orbitIndex];
        radius = orbitSpawnInfo.radius + Random.Range(orbitAttack.randomRadius / -2f, orbitAttack.randomRadius / 2f);

        // Position
        transform.position = GetSpawnLocation();
        lastSpawnLocationPosition = transform.position;

        Vector3 aimDirection = (GetAimTarget(attack.aimMode) - actor.GetActionSourceTransform().position).normalized;
        float aimRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float startingRotation = orbitSpawnInfo.startingAngle + Random.Range(orbitAttack.randomStartingAngle / -2f, orbitAttack.randomStartingAngle / 2f);

        transform.position += Quaternion.AngleAxis(startingRotation + aimRotation, Vector3.forward) * Vector2.right * radius;
        transform.rotation = Quaternion.AngleAxis(startingRotation + aimRotation + 90 * OrbitSign(), Vector3.forward);

        base.Start();

        if (orbitSpawnInfo.orbitDirection == OrbitAttack.RotationDirection.Counterclockwise)
        {
            transform.GetChild(0).transform.localScale = Vector3.Scale(transform.GetChild(0).transform.localScale, new Vector3(1,-1,1));
        }
    }

    new void FixedUpdate()
    {
        if (remainingHomingTime <= 0 && attack.homingSpeed <= 0)
        {
            transform.rotation *= Quaternion.AngleAxis(OrbitSign() * Mathf.Rad2Deg * speed * Time.fixedDeltaTime / radius, Vector3.forward);
        }

        base.FixedUpdate();

        if (orbitAttack.attachedToSpawnLocaiton)
        {
            rigidBody.MovePosition(transform.position + GetSpawnLocation() - lastSpawnLocationPosition + (Vector3)rigidBody.velocity * Time.fixedDeltaTime);
            lastSpawnLocationPosition = GetSpawnLocation();
        }
    }

    float OrbitSign()
    {
        return (orbitAttack.spawnSequence[orbitIndex].orbitDirection == OrbitAttack.RotationDirection.Clockwise ? -1f : 1f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>
[CreateAssetMenu(fileName = "NewDuplicateAttackSequence", menuName = "Cards/AttackModifers/DuplicateAttackSequence")]
public class DuplicateAttackSequence : AttackModifier
{
    [Tooltip("The time in seconds to delay the duplicate attack sequence by.")]
    public float duplicateDelay = 1; 

    // The initial length of the attack sequence.
    int sequenceLength;
    // The initial length of the attack sequence.
    List<ProjectileSpawnInfo> spawnSequence;

    // The projectile this modifies
    public override Projectile modifiedProjectile
    {
        set
        {
            if (value.index == 0)
            {
                spawnSequence = value.spawnSequence;
                sequenceLength = spawnSequence.Count;
                value.StartCoroutine(UpadateSpawnSequnce());
            }
        }
    }

    private IEnumerator UpadateSpawnSequnce()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < sequenceLength; i++)
        {
            spawnSequence.Add(spawnSequence[i].Instantiate());
        }

        spawnSequence[spawnSequence.Count - sequenceLength].delay += duplicateDelay;
    }
}

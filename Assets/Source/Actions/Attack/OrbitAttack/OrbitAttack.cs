using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitAttack : Attack
{
    [Tooltip("The sequence of when and where to spawn bullets")]
    [SerializeField] private List<OrbitSpawnInfo> _spawnSequence = new List<OrbitSpawnInfo>() { new OrbitSpawnInfo() };
    public override List<ProjectileSpawnInfo> spawnSequence
    {
        get
        {
            return _spawnSequence
                .Select(x => x as ProjectileSpawnInfo)
                .ToList();
        }
        set
        {
            _spawnSequence = value
                .Select(x => x as OrbitSpawnInfo)
                .ToList();
        }
    }

    [Tooltip("The amount the starting angle will vary by")]
    public float randomStartingAngle = 0f;

    [Tooltip("The amount the radius will vary by")]
    public float randomRadius = 0f;

    [Tooltip("Whether or not the projectiles will follow the spawn location")]
    public bool attachedToSpawnLocation = true;

    #region Previewing
    public override void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers)
    {
        throw new System.NotImplementedException();
    }

    public override void CancelPreview(IActor actor)
    {
        throw new System.NotImplementedException();
    }

    public override void Preview(IActor actor)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}


/// <summary>
/// The information about a single bullet spawning event.
/// </summary>
[System.Serializable]
public class OrbitSpawnInfo : ProjectileSpawnInfo
{
    /// <summary>
    /// The direction a projectile orbits in.
    /// </summary>
    public enum RotationDirection
    {
        Clockwise,
        Counterclockwise
    }


    [Tooltip("The angle on the orbit that projectiles will start at relative to the aim direction")]
    public float startingAngle = 0f;

    [Tooltip("The radius of the orbit")]
    public float radius = 1f;

    [Tooltip("The direction this projectile will orbit in")]
    public RotationDirection orbitDirection;



    /// <summary>
    /// Creates a duplicate of this.
    /// </summary>
    /// <returns> The created duplicate. </returns>
    public override ProjectileSpawnInfo Instantiate()
    {
        OrbitSpawnInfo newInfo = new OrbitSpawnInfo();
        newInfo.delay = delay;
        newInfo.startingAngle = startingAngle;
        newInfo.radius = radius;
        newInfo.orbitDirection = orbitDirection;

        return newInfo;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An attack that creates a projectile that orbits around the spawn location.
/// </summary>
public class OrbitAttack : Attack
{
    [Tooltip("The sequence of when and where to spawn bullets")]
    [SerializeField] private List<OrbitSpawnInfo> _spawnSequence = new List<OrbitSpawnInfo>() { new OrbitSpawnInfo() };
    public override List<ProjectileSpawnInfo> spawnSequence
    {
        get
        {
            return _spawnSequence.Cast<ProjectileSpawnInfo>().ToList();
        }
        set
        {
            _spawnSequence = value.Cast<OrbitSpawnInfo>().ToList();
        }
    }

    [Tooltip("The amount the starting angle will vary by")]
    public float randomStartingAngle = 0f;

    [Tooltip("The amount the radius will vary by")]
    public float randomRadius = 0f;

    [Tooltip("Whether or not the projectiles will follow the spawn location")]
    public bool attachedToSpawnLocation = true;

    #region Previewing
    /// <summary>
    /// A scriptable object data about an attack that can be used by cards and enemies.
    /// </summary>
    /// <param name="actor"> The actor that will be playing this action. </param>
    public override void Preview(IActor actor)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Applies modifiers to a preview.
    /// </summary>
    /// <param name="actor"> The actor previewing. </param>
    /// <param name="actionModifiers"> The modifiers to apply </param>
    public override void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Removes modifiers from a preview.
    /// </summary>
    /// <param name="actor"> The actor previewing. </param>
    /// <param name="actionModifiers"> The modifiers to remove </param>
    public override void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Stops rendering a preview of what this action will do.
    /// </summary>
    /// <param name="actor"> The actor that will no longer be playing this action. </param>
    public override void CancelPreview(IActor actor)
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletAttack : Attack
{
    [Tooltip("The sequence of when and where to spawn bullets")]
    [SerializeField] private List<BulletSpawnInfo> _spawnSequence = new List<BulletSpawnInfo>() { new BulletSpawnInfo() };
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
                .Select(x => x as BulletSpawnInfo)
                .ToList();
        }
    }

    [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
    public float randomAngle = 0f;

    [Tooltip("The radius of the circle to randomly pick a point to spawn projectiles within.")]
    public float randomOffset = 0f;

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
public class BulletSpawnInfo : ProjectileSpawnInfo
{
    [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
    public float angle = 0;

    [Tooltip("The offset from the spawn location to spawn this bullet at")]
    public Vector2 offset = Vector2.zero;



    /// <summary>
    /// Creates a duplicate of this.
    /// </summary>
    /// <returns> The created duplicate. </returns>
    public override ProjectileSpawnInfo Instantiate()
    {
        BulletSpawnInfo newInfo = new BulletSpawnInfo();
        newInfo.delay = delay;
        newInfo.angle = angle;
        newInfo.offset = offset;

        return newInfo;
    }
}
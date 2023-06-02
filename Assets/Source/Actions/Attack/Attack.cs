using Skaillz.EditInline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object data about an attack that can be used by cards and enemies.
/// </summary>
public abstract class Attack : Action
{
    [Header("Hits")]

    [Tooltip("The damage, damage type, status effects, and knockback this projectile will deal.")]
    public DamageData attack;
        
    [Tooltip("The number of objects this can hit before being destroyed.")] [Min(1)]
    public int hitCount = 1;

    [Tooltip("Whether or not this projectile should apply damage on hit.")]
    public bool applyDamageOnHit = true;
                
    [Tooltip("The modifiers that are always applied to this projectile")] [EditInline]
    public List<AttackModifier> modifiers;

    [Tooltip("The radius of the projectile.")] [EditInline]
    public ProjectileShape shape;



    [Header("Movement Info")]

    [Tooltip("The lifetime of projectiles spawned by this.")]
    public float lifetime = 10f;

    [Tooltip("The speed projectile will start traveling at. In tiles/second")]
    public float initialSpeed = 10f;

    [Tooltip("The acceleration this projectile will experience. In tiles/second^2")]
    public float acceleration = -1f;

    [Tooltip("The minimum speed projectile will travel at. In tiles/second")]
    public float minSpeed = 5f;

    [Tooltip("The maximum speed projectile will travel at. In tiles/second")]
    public float maxSpeed = 10f;



    [Header("Homing")]
        
    [Tooltip("The speed in degrees/s that projectiles will rotate towards the closest enemy")] [Min(0)]
    public float homingSpeed = 0;

    [Tooltip("The duration that this will home for.")] [Min(0)]
    public float homingTime = 0;

    [Tooltip("What the homing will rotate the projectile towards")]
    public AimMode homingAimMode;



    [Header("Visuals")]

    [Tooltip("The game object used to render the projectiles.")]
    public GameObject visualObject;

    [Tooltip("Whether or not the visuals should be detached before the object is destroyed so that they can handle their own lifetime.")]
    public bool detachVisualsBeforeDestroy = false;



    [Header("Spawning")]

    [Tooltip("The location to spawn the projectiles at.")]
    public SpawnLocation spawnLocation;

    [Tooltip("Whether or not the player needs to aim. If false it will be aimed at the closet enemy")]
    public AimMode aimMode;

    [Tooltip("The sequence of when and where to spawn projectiles")]
    public abstract List<ProjectileSpawnInfo> spawnSequence { set; get; }

    // The projectile to spawn
    public Projectile projectilePrefab;
    // The previewer prefab to use.
    public AttackPreviewer previewerPrefab;



    #region Previewing
    /// <summary>
    /// A scriptable object data about an attack that can be used by cards and enemies.
    /// </summary>
    /// <param name="actor"> The actor that will be playing this action. </param>
    public abstract void Preview(IActor actor);

    /// <summary>
    /// Applies modifiers to a preview.
    /// </summary>
    /// <param name="actor"> The actor previewing. </param>
    /// <param name="actionModifiers"> The modifiers to apply </param>
    public abstract void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers);

    /// <summary>
    /// Removes modifiers from a preview.
    /// </summary>
    /// <param name="actor"> The actor previewing. </param>
    /// <param name="actionModifiers"> The modifiers to remove </param>
    public abstract void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers);

    /// <summary>
    /// Stops rendering a preview of what this action will do.
    /// </summary>
    /// <param name="actor"> The actor that will no longer be playing this action. </param>
    public abstract void CancelPreview(IActor actor);
    #endregion

    #region Playing

    /// <summary>
    /// Plays this action and causes all its effects. Also cancels any relevant previews.
    /// </summary>
    /// <param name="actor"> The actor that will be playing this action. </param>
    /// <param name="modifiers"> The modifiers to be applied to this attack. </param>
    /// <param name="causer"> The causer of damage dealt by this attack. </param>
    /// <param name="ignoredObjects"> The objects this action will ignore. </param>
    public virtual void Play(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects = null)
    {
        actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(PlaySpawnSequence(actor, modifiers, causer, ignoredObjects));
    }
    public void Play(IActor actor, GameObject causer, List<GameObject> ignoredObjects = null)
    {
        Play(actor, new List<AttackModifier>(), causer, ignoredObjects);
    }
    public void Play(IActor actor, List<AttackModifier> modifiers, List<GameObject> ignoredObjects = null)
    {
        Play(actor, modifiers, actor.GetActionSourceTransform().gameObject, ignoredObjects);
    }
    public sealed override void Play(IActor actor, List<GameObject> ignoredObjects = null)
    {
        Play(actor, new List<AttackModifier>(), actor.GetActionSourceTransform().gameObject, ignoredObjects);
    }

    /// <summary>
    /// Spawns all of the projectiles in spawnSequence and creates delays appropriately.
    /// </summary>
    /// <param name="actor"> The actor that is playing this action. </param>
    /// <param name="modifiers"> The modifiers that are applied to this attack. </param>
    /// <param name="causer"> The causer of damage dealt by this attack. </param>
    /// <param name="ignoredObjects"> The objects this action will ignore. </param>
    protected IEnumerator PlaySpawnSequence(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects)
    {
        List<ProjectileSpawnInfo> spawnSequence = new List<ProjectileSpawnInfo>(this.spawnSequence);
        for (int i = 0; i < spawnSequence.Count; i++)
        {
            if (spawnSequence[i].delay > 0)
            {
                yield return new WaitForSeconds(spawnSequence[i].delay);
            }
            SpawnProjectile(actor, modifiers, causer, ignoredObjects, i, spawnSequence);

            // Wait to ensure the sequence doesn't miss new additions
            if (i + 1 >= spawnSequence.Count)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// Spawns a single projectile from the spawn sequence.
    /// </summary>
    /// <param name="actor"> The actor spawning the projectile. </param>
    /// <param name="modifiers"> The modifiers that are applied to this attack. </param>
    /// <param name="causer"> The causer of damage dealt by this attack. </param>
    /// <param name="ignoredObjects"> The objects this action will ignore. </param>
    /// <param name="index"> The index in the spawn sequence that the projectile was spawned from. </param>
    /// <param name="spawnSequence"> The spawn sequence instance that is being used. </param>
    /// <returns> The projectile that was spawned. </returns>
    protected virtual Projectile SpawnProjectile(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects, int index, List<ProjectileSpawnInfo> spawnSequence)
    {
        Projectile projectile = Instantiate(projectilePrefab.gameObject).GetComponent<Projectile>();
        projectile.attack = this;
        projectile.actor = actor;
        projectile.modifiers = new List<AttackModifier>(this.modifiers);
        projectile.modifiers.AddRange(modifiers);
        projectile.causer = causer;
        projectile.ignoredObjects = ignoredObjects;
        projectile.index = index;
        projectile.spawnSequence = spawnSequence;
        return projectile;
    }
    #endregion
}

/// <summary>
/// The different location where a projectile could spawn at.
/// </summary>
public enum SpawnLocation
{
    Actor,
    AimPosition
}

/// <summary>
/// The different things a projectile could be aimed at.
/// </summary>
public enum AimMode
{
    AtMouse,
    AtClosestEnemy,
    AtRandomEnemy
}

/// <summary>
/// The information about a single bullet spawning event.
/// </summary>
[System.Serializable]
public abstract class ProjectileSpawnInfo
{
    [Tooltip("The time to wait after the previous bullet to spawn this one")]
    public float delay = 0;

    /// <summary>
    /// Creates a duplicate of this.
    /// </summary>
    /// <returns> The created duplicate. </returns>
    public abstract ProjectileSpawnInfo Instantiate();
}
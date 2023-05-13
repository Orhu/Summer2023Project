using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Previews the path that a projectile will take.
/// </summary>
public class ProjectilePreviewer : MonoBehaviour
{
    // The owner of the preview
    internal IActor actor;
    // The spawner that this is previewing.
    internal SpawnProjectile spawner;
    // The number of stacks of the projectile to preview.
    int numStacks = 1;
    internal int NumStacks
    {
        set 
        {
            transform.localScale *= (float)value / numStacks;
            numStacks = value;
        }
        get { return numStacks; }
    }

    // The sprite of the preview.
    SpriteRenderer sprite;

    /// <summary>
    /// Updates sprite color and size.
    /// </summary>
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = spawner.previewColor;
        transform.localScale = new Vector3(spawner.range * numStacks, spawner.size * 2 * numStacks, 0);
    }

    /// <summary>
    /// Updates rotation.
    /// </summary
    void Update()
    {
        Vector3 diff = actor.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}

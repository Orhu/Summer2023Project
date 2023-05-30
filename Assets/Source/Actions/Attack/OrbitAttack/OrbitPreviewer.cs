using UnityEngine;

/// <summary>
/// Previews the path that a projectile will take.
/// </summary>
public class OrbitPreviewer : AttackPreviewer
{

    // The sprite of the preview.
    SpriteRenderer sprite;

    /// <summary>
    /// Updates sprite color and size.
    /// </summary>
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        //transform.localScale = new Vector3(spawner.range * (spawner.stackRange ? numStacks : 1), spawner.size * 2 * (spawner.stackSize ? numStacks : 1), 0);
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

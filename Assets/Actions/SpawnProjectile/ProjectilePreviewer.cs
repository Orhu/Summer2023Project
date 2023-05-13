using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePreviewer : MonoBehaviour
{
    internal IActor actor;
    internal SpawnProjectile spawner;
    int count = 1;
    internal int Count
    {
        set 
        {
            transform.localScale *= (float)value / count;
            count = value;
        }
        get { return count; }
    }
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = spawner.previewColor;
        transform.localScale = new Vector3(spawner.range * count, spawner.size * 2 * count, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = actor.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}

using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePreviewer : MonoBehaviour
{
    internal ICardPlayer player;
    internal SpawnProjectile spawner;
    private int count = 1;
    internal int Count
    {
        set 
        { 
            count = value;
            sprite.color = new Color(spawner.previewColor.r, spawner.previewColor.g, spawner.previewColor.b, spawner.previewColor.a * value);
        }
        get { return count; }
    }
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = spawner.previewColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.GetActionAimPosition());
    }
}

using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // The spawner of the projectile.
    internal SpawnProjectile spawner;
    // The actor of the projectile.
    internal IActor actor;
    // The attack this will cause when it hits
    internal Attack attack;

    Rigidbody2D rigidBody;
    float distanceTraveled;
    internal int count;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.radius = spawner.size * count;
        Physics2D.IgnoreCollision(collider, actor.GetCollider());

        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = spawner.sprite;
        sprite.transform.localScale = new Vector3(count, count, count);

        Vector3 diff = actor.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
        transform.position += transform.right * spawner.size * count;
    }

    void FixedUpdate()
    {
        distanceTraveled += Time.fixedDeltaTime * spawner.speed * count;
        rigidBody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * spawner.speed * count);
        if (distanceTraveled > spawner.range * count)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.Attack(attack);
        }
        Destroy(gameObject);
    }
}

using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // The spawner of the projectile.
    internal SpawnProjectile spawner;
    // The player of the projectile.
    internal ICardPlayer player;

    private Rigidbody rigidbody;
    private float distanceTraveled;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.radius = spawner.size;
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetCollider());
        GetComponent<SpriteRenderer>().sprite = spawner.sprite;
    }

    private void FixedUpdate()
    {
        distanceTraveled += Time.fixedDeltaTime * spawner.speed;
        rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * spawner.speed);
        if (distanceTraveled > spawner.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Health health = collision.gameObject.GetComponent<Health>();
        //if (health != null)
        //{
        //    health.damage(spawner.damage);
        //}
        Destroy(gameObject);
    }
}
